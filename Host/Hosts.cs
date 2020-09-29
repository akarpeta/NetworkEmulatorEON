using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Net;
using System.Net.Sockets;



namespace Host
{
    

    public class Hosts
    {
        public MyPacket myPacket { get; set; }

        public MySocket mySocket { get; set; }

        public HostLogs window { get; set; }

        public string HostName { get; set; }

        public IPAddress IPAddress { get; set; }

        public ushort OutPort { get; set; }

        public IPAddress CloudIPAddress { get; set; }

        public ushort CloudPort { get; set; }

        public bool FlagListening = true;

        public List<Connections> connections = new List<Connections>();

        //public List<DistantHosts> DistantHosts { get; set; }


        public Hosts(int n, HostLogs tj)
        {

            window = tj;
            NewLog("Konstruktor host", window, "LightBlue");
            ReadConfig(n);
            Task.Run(() => MyConnect(window)); ;
        }

        private void MyConnect(HostLogs window)
        {
            try
            {
                mySocket = new MySocket(CloudIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mySocket.Connect(new IPEndPoint(CloudIPAddress, CloudPort));
                mySocket.Send(Encoding.ASCII.GetBytes("HELLO " + HostName));

                Task.Run(() => { MyListen(window); });
            }
            catch (Exception)
            {

            }


        }

        private void MyListen(HostLogs window)
        {

            while (FlagListening == true)
            {
                while (!mySocket.Connected || mySocket == null)
                {
                    MyConnect(window);
                }

                try
                {
                    myPacket = mySocket.Receive();

                    if (myPacket != null)
                    {
                        NewLog($"Otrzymano pakiet {myPacket.Payload} {myPacket.Port}", window, "LightBlue");
                    }
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Shutdown || e.SocketErrorCode == SocketError.ConnectionReset)
                    {

                        NewLog("Połączenie z chmurą zerwane", window, "LightBlue");

                        continue;
                    }

                    else
                    {

                        NewLog("Nie można połączyć się z chmurą", window, "LightBlue");

                    }
                }
            }
        }

        public void ReadConfig(int numberhost)
        {

            XmlReader xmlReader = new XmlReader("ConfigurationHost.xml");
            numberhost--;
            CloudIPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(numberhost, "host", "CLOUD_IP_ADDRESS"));
            CloudPort = Convert.ToUInt16(xmlReader.GetAttributeValue(numberhost, "host", "CLOUD_PORT"));
            HostName = xmlReader.GetAttributeValue(numberhost, "host", "NAME");
            IPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(numberhost, "host", "IP_ADDRESS"));
            OutPort = Convert.ToUInt16(xmlReader.GetAttributeValue(numberhost, "host", "OUT_PORT"));
            //DistantHosts = xmlReader.GetItemsForSelectedDistantHost(hostsConfiguration.HostName, "HOST_NAME", "IP_ADDRESS");

        }


        //Wysyla wiadomość gdy zestawimy polaczenie
        public void SendMessage(IPAddress Destination, string dCapacity, string payload)
        {
            try
            {
                NewLog($"SEND MESSAGE MYPACKET {Destination}", window, "Pink");
                Predicate<Connections> isCurrentConnection = c => c.Destination == Destination.ToString();
                NewLog($"SEND MESSAGE MYPACKET {Destination}", window, "Pink");
                Connections connection = connections.Find(isCurrentConnection);
                NewLog($"SEND MESSAGE MYPACKET {Destination}", window, "Pink");
                MyPacket packet = new MyPacket(payload, OutPort, Destination.ToString(), IPAddress.ToString(), connection.slotWindow);
                //packet.Port = OutPort;
                //packet.Payload = payload;
                //packet.DestinationAddress = IPAddress.Parse(Destination);
                //packet.SourceAddress = IPAddress;
                NewLog($"STWORZONO PAKIET", window, "Pink");
                mySocket.Send(packet);
                NewLog($"Wysłano pakiet nr {packet.ID} wiadomość {packet.Payload}  z portu {packet.Port}", window, "Pink");
            }
            catch(Exception e)
            {
                NewLog($"SendMessage EXCEPTION: {e}", window, "Pink");
                foreach(var c in connections)
                {
                    NewLog($"connections {c.Destination} {c.slotWindow.FirstSlot} {c.slotWindow.NumberofSlots}", window, "Pink");
                }
            }
            

            //await Task.Delay(TimeSpan.FromMilliseconds(10000));
        }

        public void NewLog(string info, HostLogs tmp, string color)
        {

            window.Dispatcher.Invoke(() => window.NewLog(info, tmp, color));

        }
    }
}
