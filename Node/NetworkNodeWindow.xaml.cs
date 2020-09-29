using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Utility;

namespace Node
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NetworkNodeWindow : Window
    {     
        public MySocket mySocket { get; set; }
        public NetworkNode networknode;
        public int NodeNumber { get; set; }

        public NetworkNodeWindow(int n)
        {
            InitializeComponent();
            logBox.Document.Blocks.Remove(logBox.Document.Blocks.FirstBlock);
            NodeNumber = n;
            Title = "Router " + NodeNumber.ToString();
            networknode = new NetworkNode(this);
            Task.Run(() => MyStart(NodeNumber));
        }

        MyPacket RoutePacket(MyPacket packet)
        {
            try
            {
                NewLog($"RoutePacket {packet.slotWindow.FirstSlot} {packet.slotWindow.NumberofSlots}", this);
               
                Predicate<RouterMapRecord> goodRecord = r => ((r.slotWindow.FirstSlot == packet.slotWindow.FirstSlot)
                    && (r.slotWindow.NumberofSlots == packet.slotWindow.NumberofSlots));
                RouterMapRecord record = networknode.routerMapRecords.Find(goodRecord);
                NewLog($"RoutePacket destinationID:::: {record.DestinationID}", this);
                if (null == record)
                {
                    NewLog($"RoutePacket record is null", this);
                }
                if (record.DestinationID >= 0) //wysylamy do routera
                {
                    packet.Port = -record.DestinationID;
                }
                else if(networknode.HostPort>=0) // wysylamy do hosta
                {
                    packet.Port = networknode.HostPort;
                }else //wysylam do 2 domeny
                {
                    packet.Port = networknode.EdgePort;
                }
            }
            catch(Exception ex)
            {
                NewLog($"RoutePacket {ex}", this);
                foreach(var it in networknode.routerMapRecords)
                {
                    NewLog($" {it.slotWindow.FirstSlot} {it.slotWindow.NumberofSlots} {it.DestinationID}", this);
                }
            }
            return packet;
        }

        private void MyStart(int nodeNr)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                NewLog("Tworzenie routera", this);
            }));

            try
            {   
                XmlReader xmlReader = new XmlReader("ConfigurationNode.xml");
                networknode.ID = nodeNr;
                nodeNr--;
                networknode.CloudIP = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr, "router", "CLOUD_IP_ADDRESS"));
                networknode.CloudPort = Convert.ToUInt16(xmlReader.GetAttributeValue(nodeNr, "router", "CLOUD_PORT"));
                networknode.ManagementIP = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr, "router", "MANAGEMENT_IP_ADDRESS"));
                networknode.ManagementPort = Convert.ToUInt16(xmlReader.GetAttributeValue(nodeNr, "router", "MANAGEMENT_PORT"));
                networknode.NodeName = xmlReader.GetAttributeValue(nodeNr, "router", "NAME");
                networknode.ListenIPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr, "router", "LISTEN_IP_ADDRESS"));
                networknode.IPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr, "router", "IP_ADDRESS"));
                networknode.RCIPAddres = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr,"router","IP_ADDRESS_RC"));
                networknode.CCIPAddress = IPAddress.Parse(xmlReader.GetAttributeValue(nodeNr, "router", "IP_ADDRESS_CC"));
                string HostPortStr = xmlReader.GetOptionalAttributeValue(nodeNr, "router", "HOST_PORT");
                networknode.HostPort = (HostPortStr == null) ? -1 : int.Parse(HostPortStr);
                networknode.ListNode.Add(networknode);

                string EdgePortStr = xmlReader.GetOptionalAttributeValue(nodeNr, "router", "EDGE_PORT");
                networknode.EdgePort = (EdgePortStr == null) ? -1 : int.Parse(EdgePortStr);
                NewLog($"{networknode.HostPort}", this);
                NewLog($"edge port{networknode.EdgePort}", this);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    NewLog($"Exception {e}", this);
                }));
                NewLog($"Exception {e}", this);
                return;
            }
            //networknode.RoutingTables.LoadTablesFromConfig(networknode, this);
            networknode.RoutingTables.StartManagementAgent();

         
            ConnectToCloud();



            //Task.WhenAll(task1, task2);

            Task.Run(async () =>
            {
                while (true)
                {
                    if (!mySocket.Connected || mySocket == null)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            NewLog("Zerwane połączenie z chmurą", this);
                        }));
                       
                        break;
                    }
                    else
                    {
                        mySocket.Send(Encoding.ASCII.GetBytes("C"));
                        await Task.Delay(5000);
                    }
                }
            });

            while (true)
            {
                while (!mySocket.Connected || mySocket == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        NewLog("Ponawianie połączenia z chmurą", this);
                    }));
                
                    mySocket = new MySocket(networknode.CloudIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    mySocket.Connect(new IPEndPoint(networknode.CloudIP, networknode.CloudPort));
                    mySocket.Send(Encoding.ASCII.GetBytes($"HELLO {networknode.NodeName}"));

                    Dispatcher.Invoke(new Action(() =>
                    {
                        NewLog("Ponownie połączony z chmurą", this);
                    }));
                  
                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            if (!mySocket.Connected || mySocket == null)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    NewLog("Zerwane połączenie z chmurą", this);
                                }));
                              
                                break;
                            }
                            else
                            {
                                mySocket.Send(Encoding.ASCII.GetBytes("C"));
                                await Task.Delay(5000);
                            }
                        }
                    });
                }
                         
                try
                {
                  
                    MyPacket receivedPacket = mySocket.Receive();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        NewLog($"Otrzymano pakiet nr {receivedPacket.ID} wiadomość {receivedPacket.Payload} z portu {receivedPacket.Port}", this);
                    }));
                    
                    Task.Run(() =>
                    { 
                        MyPacket routedPacket = RoutePacket(receivedPacket);
                        //routedPacket = networknode.PacketLogic.RoutePacket(receivedPacket, networknode.RoutingTables, this);
                        if (routedPacket == null)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                NewLog("Pakiet nie został obsłużony poprawnie", this);
                            }));
                            return;
                        }

                        try
                        {
                            mySocket.Send(routedPacket);
                        }
                        catch (Exception e)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                NewLog($"Pakiet nr {routedPacket.ID} nie został wysłany poprawnie: {e.Message}", this);
                            }));
                            
                        }
                    });
                }

                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.TimedOut)
                    {
                        if (e.SocketErrorCode == SocketError.Shutdown || e.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                NewLog("Zerwane połączenie z chmurą", this);
                            }));
                          
                            continue;
                        }

                        else
                        {
                            NewLog($"{e.Source}: {e.SocketErrorCode}", this);
                        }
                    }
                }
            }
        }

        private void ConnectToCloud()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                NewLog("Próba połączenia z chmurą kablową", this);
            }));
            NewLog("Próba połączenia z chmurą kablową", this);

            try
            {
                MySocket socket = new MySocket(networknode.CloudIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(new IPEndPoint(networknode.CloudIP, networknode.CloudPort));

                socket.Send(Encoding.ASCII.GetBytes($"HELLO {networknode.NodeName}"));
                Dispatcher.Invoke(new Action(() =>
                {
                    NewLog("Uzyskano połączenie z chmurą kablową", this);
                }));

                Task.Run(() => Listen());

                mySocket = socket;

                Task.Run(async () =>
                {
                    while (true)
                    {
                        var tmp = mySocket != null && mySocket.Connected;
                        if (tmp)
                        {
                            mySocket.Send(Encoding.ASCII.GetBytes("C"));

                            await Task.Delay(5000);
                        }
                        else
                        {
                            
                                NewLog("Zerwane połączenie z chmurą", this);
                          

                            break;
                        }
                    }
                });
            }
            catch (Exception)
            {
                
                    NewLog("Nieudane połączenie z chmurą", this);
             
            }
        }

        //obsluga wiadomosci udp
        public void Listen()
        {
            NewLog($"Node Listen", this);
            try
            {
                byte[] messagebyte = new byte[64];
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint ipep = new IPEndPoint(networknode.ListenIPAddress, 11000);
                UdpClient newsock = new UdpClient(ipep);
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
           

                bool FlagListening = true;

                while (FlagListening == true)
                {
                    
                    messagebyte = newsock.Receive(ref sender);
                    if (messagebyte.Length > 0)
                    {
                        var message = Encoding.ASCII.GetString(messagebyte);
                        var action = message.Split(null)[0];
                        var tmp = message.Replace($"{action} ", "");
                        var data = tmp.Split(' ');
                        switch (action)
                        {
                            case MessageNames.ADD_RECORD:
                               HandelRouterMapRecord(data);
                                break;
                            case MessageNames.DELETE_RECORD:
                                break;
                            case MessageNames.HANDLEAVAIBLESLOTREQUEST:
                                HandleAvaibleSlotsRequest();
                                break;
                        }

                        //TO DO: zapisac do tablic path[]
                        
                        messagebyte = null;
                    }
                }
            }
            catch
            {
                NewLog($"CAAAATCH", this);
            }
        }

        //dodajemy record do slot table- snp
        //firstslot numberofslots destinationid iprc
        public void HandelRouterMapRecord(string[] msg)
        {
            string resultstring = XmlSerialization.GetStringToNormal(msg);
            RouterMapRecord mapRecord = XmlSerialization.DeserializeObject<RouterMapRecord>(resultstring);
            if(AddRouterMapRecord(mapRecord))
            {
                string message = message = MessageNames.LINK_ALLOCATION_RESPONSE
                    + " "
                    + XmlSerialization.SerializeObject(mapRecord.slotWindow);
                NewLog("Dodano rekord", this);
                SendMessage(networknode.CCIPAddress,message);
                NewLog($"Wyslano wiadomosc dodano rekord do {networknode.CCIPAddress} CC", this);
            }
            else
            {
                NewLog("Nie udało się dodać rekordu", this);
            }
            //wyslij wiadomosc do RC
            //SendMessage(iprc,message);
        }

        public bool AddRouterMapRecord(RouterMapRecord newRecord)
        {
            foreach (RouterMapRecord item in networknode.routerMapRecords)
            {
                if(item.slotWindow.Collide(newRecord.slotWindow))
                {

                    NewLog($"Nie udalo sie dodac {newRecord.slotWindow.FirstSlot} {newRecord.slotWindow.NumberofSlots}", this);
                    return false;
                }
            }

            networknode.routerMapRecords.Add(newRecord);
            foreach(var item in networknode.routerMapRecords)
            {
                NewLog($"RouterMapRecord: {item.slotWindow.FirstSlot} {item.slotWindow.NumberofSlots} {item.DestinationID}", this);
            }
            
            return true;

        }

        //sprawdza dostepnosc okien 
        public void HandleAvaibleSlotsRequest()
        {
            NewLog($"{NodeNumber} Handle Avaible Slot Request", this);
            NewLog($"{NodeNumber} adres rc response: {networknode.RCIPAddres}", this);
            //lista dostepnych slotow
            var listofslots = new System.Collections.Generic.List<int>();

            for(int i=0; i<10; i++)
            {
                listofslots.Add(i);
            }

            foreach(RouterMapRecord item in networknode.routerMapRecords)
            {
                for(int i=item.slotWindow.FirstSlot; i<(item.slotWindow.FirstSlot+item.slotWindow.NumberofSlots);i++)
                {
                    listofslots.Remove(i);
                }
            }

            MessageAvaibleSlotsInNode response = new MessageAvaibleSlotsInNode();
            response.ID = networknode.ID;
            response.AvaibleSlots = listofslots;

            string messageContent = MessageNames.HANDLEAVAIBLESLOT_RESPONSE + " ";
            messageContent += XmlSerialization.SerializeObject(response);
            //wysylamy wiadomosc do RC 
            SendMessage(networknode.RCIPAddres, messageContent);
        }

        public void SendMessage(IPAddress ipaddress, string message)          //wysylanie wiadomosci
        {
            byte[] data = new byte[64];
            UdpClient newsock = new UdpClient();
            IPEndPoint sender = new IPEndPoint(ipaddress, 11000);

            try
            {
                data = Encoding.ASCII.GetBytes(message);
                newsock.Send(data, data.Length, sender);
                newsock.Close();
            }
            catch (Exception)
            {
                newsock.Close();
            }
        }

        public void NewLog(string info, NetworkNodeWindow tmp)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                 string timeNow = DateTime.Now.ToString("h:mm:ss") + "." + DateTime.Now.Millisecond.ToString();
            string restLog = info;
            string fullLog = timeNow + " " + restLog;
            tmp.logBox.Document.Blocks.Add(new Paragraph(new Run(fullLog)));
            tmp.logBox.ScrollToEnd();
            }));
        }

    }
}
