using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utility;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Concurrent;

namespace CableCloud
{
    class StateObject
    {
        public Socket workSocket { get; set; } = null;
        public const int BufferSize = 1024;
        public byte[] Buffer { get; set; } = new byte[BufferSize];
        public StringBuilder sb { get; set; } = new StringBuilder();
    }

    public class Cloud //: INotifyPropertyChanged
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private IPAddress CloudIPAddress;
        private int CloudPort;
        IPEndPoint CloudEP;

        public IPAddress CloudIPAddressListen = IPAddress.Parse("127.0.0.2");
        //parametr z dokumentacji
        private const int Backlog = 100;
        private Socket AsyncServer;

        public MainWindow myWindow;
        public Dictionary<string, string> IsCableWorkingDictionary { get; set; }
        public Dictionary<string, string> NodeAddressDictionary { get; set; }
        public Dictionary<string, string> NodesConnectedDictionary { get; set; }

        private ConcurrentDictionary<Socket, string> SocketToNodeName;
        private ConcurrentDictionary<string, Socket> NodeNameToSocket;

        private List<CableLinkingNodes> connectedPairs;
        public List<CableLinkingNodes> ConnectedPairs
        {
            get
            {
                return connectedPairs;
            }
            set
            {
                connectedPairs = value;
                NotifyPropertyChanged("ConnectedPairs");
            }
        }

        //logi, wstepnie sa ok
        private string logText = DateTime.Now.ToString("h:mm:ss") + "." + DateTime.Now.Millisecond.ToString() + " Start chmury kablowej";
        public string LogText
        {
            get
            {
                return logText;
            }
            set
            {
                logText = value;
                NotifyPropertyChanged(nameof(LogText));
            }
        }

        public Cloud(MainWindow tmp)
        {
            myWindow = tmp; 
            CableCloudConfig();
            SocketToNodeName = new ConcurrentDictionary<Socket, string>();
            NodeNameToSocket = new ConcurrentDictionary<string, Socket>();
            var task1 = Task.Run(() => StartAsyncListener());
            var task2 = Task.Run(() => Listen());
            Task.WhenAll(task1, task2);
        }

        //obsluga wiadomosci udp
        public void Listen()
        {
            NewLog($"Cloud Listen", myWindow, "LightCyan");
            try
            {

                byte[] messagebyte = new byte[64];


                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint ipep = new IPEndPoint(CloudIPAddressListen, 11000);
                UdpClient newsock = new UdpClient(ipep);
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                bool FlagListening = true;

                while (FlagListening == true)
                {

                    messagebyte = newsock.Receive(ref sender);
                    if (messagebyte.Length > 0)
                    {
                        var message = Encoding.ASCII.GetString(messagebyte);
                        if (message.Length > 0)
                        {
                            //TO DO: obsluga wiadomosci z udp
                            NewLog($"Payload {message}", myWindow, "Pink");
                        }
                        messagebyte = null;
                    }
                }
            }
            catch
            {

            }
        }

       
        public void CableCloudConfig()
        {
            XmlReader xmlReader = new XmlReader("ConfigurationCloud.xml");
            CloudIPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(0, "cloud", "CLOUD_IP_ADDRESS"));
            CloudPort = Convert.ToInt32(xmlReader.GetAttributeValue(0, "cloud", "CLOUD_PORT"));
            CloudEP = new IPEndPoint(CloudIPAddress, CloudPort);

            NodeAddressDictionary = new Dictionary<string, string>();

            int numberofhost = xmlReader.GetNumberOfItems("host");
            for (int a = 0; a < numberofhost; a++)
            {
                var name = xmlReader.GetAttributeValue(a, "host", "NAME");
                var ip = xmlReader.GetAttributeValue(a, "host", "IP_ADDRESS");
                NodeAddressDictionary.Add(name, ip);
            }

            int numberofrouter = xmlReader.GetNumberOfItems("router");
            for (int a = 0; a < numberofrouter; a++)
            {
                var name = xmlReader.GetAttributeValue(a, "router", "NAME");
                var ip = xmlReader.GetAttributeValue(a, "router", "IP_ADDRESS");
                NodeAddressDictionary.Add(name, ip);
            }

            NodesConnectedDictionary = new Dictionary<string, string>();
            IsCableWorkingDictionary = new Dictionary<string, string>();
            ConnectedPairs = new List<CableLinkingNodes>();

            int numberofconnection = xmlReader.GetNumberOfItems("connect");
            for (int a = 0; a < numberofconnection; a++)
            {
                var node_name1 = xmlReader.GetAttributeValue(a, "connect", "NODE1");
                var node_name2 = xmlReader.GetAttributeValue(a, "connect", "NODE2");
                var port1 = xmlReader.GetAttributeValue(a, "connect", "PORT1");
                var port2 = xmlReader.GetAttributeValue(a, "connect", "PORT2");
                //var Isworking = xmlReader.GetAttributeValue(a, "connect", "ISWORKING");

                NodesConnectedDictionary.Add(node_name1 + ":" + port1, node_name2 + ":" + port2);
                NodesConnectedDictionary.Add(node_name2 + ":" + port2, node_name1 + ":" + port1);

                var connection1 = node_name1 + ":" + port1;
                var connection2 = node_name2 + ":" + port2;
                //IsCableWorkingDictionary.Add(connection1 + "-" + connection2, Isworking);
                //IsCableWorkingDictionary.Add(connection2 + "-" + connection1, Isworking);

                ConnectedPairs.Add(new CableLinkingNodes(node_name1, port1, node_name2, port2, "WORKING"));
            }
        }
        //metoda wywolywana w konstruktorze clouda
        public void StartAsyncListener()
        {
            
            AsyncServer = new Socket(CloudIPAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (!AsyncServer.IsBound)
                {
                    AsyncServer.Bind(CloudEP);
                    AsyncServer.Listen(Backlog);
                }
                else
                {
                    //chyba nic
                }
                while (true)
                {
                    allDone.Reset();
                    AsyncServer.BeginAccept(new AsyncCallback(AcceptCallback), AsyncServer);
                    allDone.WaitOne();
                }
            }
            catch (Exception)
            {
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            
            allDone.Set();
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject
            {
                workSocket = handler
            };
            //buffer ma size 1024
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
        //w tym siedza m.in. Keepalive
        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead;
            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            //jezeli endreceive nie mial czego zakonczyc
            catch (Exception)
            {

                var nodeName = SocketToNodeName[handler];
                SocketToNodeName.TryRemove(handler, out string outString);
                NodeNameToSocket.TryRemove(nodeName, out Socket outSocket);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                return;
            }

            state.sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
            var content = state.sb.ToString().Split(' ');
            if (content[0].Equals("HELLO"))
            {
                //C jest wiadomoscia typu Keepalive
                int index = content[1].IndexOf("C");
                if (index > 0)
                {
                    content[1] = content[1].Substring(0, index);
                }
                while (true)
                {
                    var success = SocketToNodeName.TryAdd(handler, content[1]);
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                while (true)
                {
                    var success = NodeNameToSocket.TryAdd(content[1], handler);
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                NewLog($"Połączono z {content[1]}", myWindow,"LightCyan");
            }
            else if (content[0].Equals("C"))
            {
                //do nothing
            }
            else
            {
                ProcessPacket(state, handler, ar);
            }
            state.sb.Clear();
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
        //chyba ok
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
            }
            catch (Exception)
            {
            }
        }
        //wykorzystywana w ProcessPacket, jest ok
        private void SendPacket(Socket handler, byte[] byteData)
        {
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private int HackishPortTranslation(int value, string srcNodeName)
        {
            NewLog($"HackishPortTranslation: {value}", myWindow, "LightCyan");
            if (value >= 0)
                return value;
            NewLog($"HackishPortTranslation Value: {value}", myWindow, "LightCyan");
            int nodeID = -value;
            NewLog($"HackishPortTranslation nodeID: {nodeID}", myWindow, "LightCyan");
            string destNodeName = "router" + nodeID;
            try
            {
                foreach (var it in connectedPairs)
                {
                    if (it.Node1 == srcNodeName && it.Node2 == destNodeName)
                    {
                        NewLog($"HackishPortTranslation OK1 {it.Port1} {srcNodeName} {destNodeName}", myWindow, "LightCyan");
                        return Int32.Parse(it.Port1);
                    }
                    if (it.Node2 == srcNodeName && it.Node1 == destNodeName)
                    {
                        NewLog($"HackishPortTranslation OK2 {it.Port2} {srcNodeName} {destNodeName}", myWindow, "LightCyan");
                        return Int32.Parse(it.Port2);
                    }
                }
            }
            catch (Exception ex)
            {
                NewLog($"HackishPortTranslation {ex}", myWindow, "LightCyan");
            }
            NewLog($"HackishPortTranslation {destNodeName}", myWindow, "Red");
            throw new Exception("HackishPortTranslation: No port found");
        }
        //czy tu powinno byc cos wiecej? chyba jakas lambda czy cos powinna byc w logach 
        private void ProcessPacket(StateObject state, Socket handler, IAsyncResult ar)
        {
            NewLog($"Process packet", myWindow, "LightCyan");
            try
            {
                MyPacket receivedpacket = MyPacket.BytesToPacket(state.Buffer);

                string myNodeName = SocketToNodeName[handler];
            
                string myNodeIP = NodeAddressDictionary[myNodeName];
                NewLog($"Process packet: {myNodeIP} {myNodeName} {receivedpacket.DestinationAddress}", myWindow, "LightCyan");
                int actualPort = HackishPortTranslation(receivedpacket.Port, myNodeName);
                string myNodePort = Convert.ToString(actualPort);
                NewLog($"ProcessPacket {actualPort} {myNodePort}", myWindow, "LightCyan");


                //dopisuję, wypisze czestotliwosc
                string myFreq = Convert.ToString(receivedpacket.Frequency);
                NewLog($"Otrzymano pakiet z {myNodeIP} port {myNodePort} częstotliwość {myFreq}", myWindow, "LightCyan");

                foreach (KeyValuePair<string, string> entry in NodeAddressDictionary)
                {
                   // NewLog($"iauhsduh vuhv  {entry.Value} ", myWindow, "Green");
                }
              
                string myNodeNext = NextCableNode(myNodeName, myNodePort);
           
                string myPortNext = NextCablePort(myNodeName, myNodePort);
           
                string myIPNext = NodeAddressDictionary[myNodeNext];
            


                receivedpacket.Port = int.Parse(myPortNext);
                    Socket tmpsocket = NodeNameToSocket[myNodeNext];
                    SendPacket(tmpsocket, receivedpacket.PacketToBytes());
                    NewLog($"Wysłano pakiet do {myIPNext} port {myPortNext} częstotliwość {myFreq}", myWindow, "LightCyan");
            }
            catch (Exception ex)
            {
                NewLog($"Połączenie nieudane {ex}", myWindow, "LightCyan");
            }
        }

        public string NextCableNode(string node, string port)
        {
            var parts = NodesConnectedDictionary[node + ":" + port].Split(':');
            return parts[0];
        }

        public string NextCablePort(string node, string port)
        {
            var parts = NodesConnectedDictionary[node + ":" + port].Split(':');
            return parts[1];
        }

        public string GetWorkingCable(string n1, string p1, string n2, string p2)
        {
            var connectedNodes = n1 + ":" + p1 + "-" + n2 + ":" + p2;
            return IsCableWorkingDictionary[connectedNodes];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public void NewLog(string info, MainWindow tmp, string color)
        {

             myWindow.Dispatcher.Invoke(() => myWindow.NewLog(info, tmp, color));

        }
    }
}
