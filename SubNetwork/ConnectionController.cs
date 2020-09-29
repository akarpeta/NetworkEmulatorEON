using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;
namespace SubNetwork
{
   
    public class ConnectionController
    {
        public IPAddress  CCiPAddress { get; set; }

        public IPAddress RCiPAddress { get; set; }

        public int numberDomainCC { get; set; }
 
        public string TypeEdge { get; set; }

        public List<LRM> LinksRM { get; set; }

        public IPAddress NCCiPAddress;

        public SubnetworkLogs myWindow;

        //ile odpowiedzi oczkujemy od lrm
        public int numberlrm { get; set; }

        public string color = "LightSkyBlue";

        //0 w jednej domenie 1 w 2 domenach
        public int FlagDomain;

        public bool tmp1 = true;
        //ip drugiej domeny
        public string other_cc { get;set; }

        public string Source { get; set; }
        public string Destination { get; set; }
        public string dCapacity { get; set; }
        public string[] path { get; set; }

        public int Modulation { get; set; }
        public int SlotNumber { get; set; }

        public int enm = 0;
        public SlotWindow slotWindow = new SlotWindow(0, 0);

        public ConnectionController(int numberDomain, SubnetworkLogs tmp)
        {
            myWindow = tmp;
            numberDomainCC = numberDomain;
            XmlReader reader = new XmlReader("CC"+numberDomain+".xml");
            CCiPAddress = IPAddress.Parse(reader.GetAttributeValue("config","IP_ADDRESS"));
            RCiPAddress = IPAddress.Parse(reader.GetAttributeValue("config","IP_ADDRESS_RC"));
            NCCiPAddress = IPAddress.Parse(reader.GetAttributeValue("config", "IP_ADDRESS_NCC"));
            other_cc = reader.GetAttributeValue("config","OTHER_CC_ADDRESS");
            LinksRM = new List<LRM>();
            int numberoflrm = reader.GetNumberOfItems("LRM");
   
            for(int a=0; a<numberoflrm; a++)
            {
                LRM lRM = new LRM(CCiPAddress, RCiPAddress,myWindow);
                lRM.IdLRM = Int32.Parse(reader.GetAttributeValue(a, "LRM", "ID"));
                lRM.IPAddressLRM = reader.GetAttributeValue(a, "LRM", "IP_ADDRESS");
                LinksRM.Add(lRM);
                Task.Run(() => lRM.ReceiveMessage());
            }
        }


    private void SendingMessage(IPAddress ipaddress, string message)
        {
            byte[] data = new byte[64];

            UdpClient newsock = new UdpClient();
            IPEndPoint sender = new IPEndPoint(ipaddress, 11000);

            try
            {
                data = Encoding.ASCII.GetBytes(message);
                newsock.Send(data, data.Length, sender);
            }
            catch
            {

            }
                newsock.Close();
        }
        private void SendingMessage(string ipaddress, string message)
        {
            byte[] data = new byte[64];

            UdpClient newsock = new UdpClient();
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ipaddress), 11000);

            try
            {
                data = Encoding.ASCII.GetBytes(message);
                newsock.Send(data, data.Length, sender);
            }
            catch
            {

            }
            newsock.Close();
        }


        public void ReceivedMessage()
        {
         
            byte[] bytes = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(CCiPAddress, 11000);
            UdpClient newsock = new UdpClient(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    bytes = newsock.Receive(ref sender);
                    var message = Encoding.ASCII.GetString(bytes);
                    var action = message.Split(null)[0];
                    var tmp = message.Replace($"{action} ", "");
                    var data = tmp.Split(' ');

                    Task.Run(() => Actions(action, data));
                }
            }
            catch (Exception)
            {

            }
        }


        private void Actions(string action ,string[] data)
        {
           
            switch (action)
            {
                case MessageNames.CONNECTION_REQUEST:
                    ConnectionRequest(data);
                    break;
                case MessageNames.PEER_COORDINATION:
                    PeerCoordinationReqRsp(data);
                    break;
                case MessageNames.ROUTE_TABLE_QUERY_RSP:
                    RouteTableQueryResponse(data);
                    break;
                case MessageNames.CALL_CONFIRMED:
                    NewLog($"{CCiPAddress} Zestawiono polaczenie TMP1: {tmp1}", myWindow);
                    break;
                case MessageNames.CONNECTION_TEARDOWN_CONFIRMED:
                    break;
                case MessageNames.CONNECTION_TEARDOWN_PEER:
                    break;
                case MessageNames.KILL_LINK:
                    KillLinkRequest(data);
                    break;
                case MessageNames.LINK_ALLOCATION_RESPONSE:
                    NewLog($"Otrzymano wiadomosc od Node/LRM o dodaniu rekordu/zaalokowaniu zasobow",myWindow);
                    GetResponses(XmlSerialization.GetStringToNormal(data));
                    break;
                case MessageNames.PEER_COORDINATION_RESPONSE:
                    SendCallConfirmed();
                    break;


            }
        }

        private void PeerCoordinationReqRsp(string[] data)
        {
            //TODO: FirstSlot
            tmp1 =false;
            TypeEdge = "START";
            Modulation = Int32.Parse(data[3]);
            SlotNumber = Int32.Parse(data[4]);
            NewLog($"{CCiPAddress}Modulacj: {Modulation} slot number: {SlotNumber}", myWindow);
            RouteTableQuery(data[0], data[1], data[2], TypeEdge, Modulation , SlotNumber);
        }

    
        private void GetResponses(string msg)
        {
            enm++;
            SlotWindow receivedWindow = XmlSerialization.DeserializeObject<SlotWindow>(msg);
            if(slotWindow.FirstSlot == 0 && slotWindow.NumberofSlots == 0)
            {
                slotWindow = receivedWindow;
            }
            else if(!slotWindow.Equals(receivedWindow))
            {
                //fail
            }
            NewLog($"GET RESPONSES {enm}", myWindow);

            //if(AreAllResponses())
            if (enm==path.Length)
            {
                //wyslij call confirmed lub peercoordination
                if (FlagDomain == 0)
                {
                    SendCallConfirmed();

                } else if (FlagDomain == 1 && tmp1 == true)
                {
                    PeerCoordinationReq();
                }
                else
                {
                    NewLog($"PeerCoordinationResponse", myWindow);
                    tmp1 = true;
                    //SendPeerCoordinationResponse to other CC
                    //string data = MessageNames.PEER_COORDINATION_RESPONSE;
                    //SendingMessage(other_cc,data);
                    SendCallConfirmed();
                }
                slotWindow = new SlotWindow(0, 0);
            }
        }

      

        private void SendCallConfirmed()
        {
            try
            {
                string message = MessageNames.CALL_CONFIRMED
                    + " "
                    + XmlSerialization.SerializeObject(slotWindow);
                SendingMessage(NCCiPAddress, message);
                NewLog($"{CCiPAddress} Zestawiono polaczenie sendcallconfirmed", myWindow);
            }
            catch(Exception ex)
            {
                NewLog($"SendCallConfirmed Eception {ex}", myWindow);
            }
        }

        /*source 
         destination 
         dCapacity
         Convert.ToInt32(FlagdDomain)
         modulation
        slotnumber
        path.Length
         data;
    */
        private void RouteTableQueryResponse(string[] data)
        {
            NewLog($"{CCiPAddress} Get RouteTableQueryResponse", myWindow);
            //zajmij szczeliny
            //Wiadomosc do LRM
            string source = data[0];
            string dest = data[1];
            string dCapacity = data[2];
            string header = source + " " + dest + " " + dCapacity;
            FlagDomain = Int32.Parse(data[3]);
            Modulation = Int32.Parse(data[4]);
            SlotNumber = Int32.Parse(data[5]);
            int length = Int32.Parse(data[6]);
            path = new string[length];
            for (int a = 0; a < length; a++)
            {
                path[a] = data[a + 7];
            }
            //SendLinkAllocationRequest(header); ;
            for (int i = 0; i < path.Length; i++)
            {
                NewLog($"{CCiPAddress} Path: {path[i]}", myWindow);
            }

            string message = MessageNames.ALLOCATE;
            SendingMessage(RCiPAddress, message);

        }

        private void SendLinkAllocationRequest(string header)
        {
           
            numberlrm = path.Length;
            for (int a=0; a<path.Length;a++)
            {
                string message = null;
                message = MessageNames.LINK_CONNECTION_REQUEST+" "+header;
                LRM act = LinksRM.Find(LRM => LRM.IdLRM == Int32.Parse(path[a]));
                NewLog($"{CCiPAddress} Send LinkAllocation do lrm adres{act.IPAddressLRM}", myWindow);
                SendingMessage(act.IPAddressLRM, message);
            }

          
        }

        private void PeerCoordinationReq()
        {
            
            NewLog($"{CCiPAddress}PeerCoordination to CC", myWindow);
            //wyznacz route table query wiedzac jakie sa zajete szczeliny
            //TO DO Send to RC RouteTableQuery zrob sciezke w drugiej domenie 
            //RouteTableQuery();
            //string startNodeAddress, string endNodeAddress, string demandedCapacity,int defaultrouter,string type
            string message = null;
            message = MessageNames.PEER_COORDINATION + " " + Source + " " + Destination +" "+dCapacity +" " + Modulation + " " + SlotNumber + " " + "START";
            SendingMessage(other_cc,message);

        }

        public void ConnectionRequest(string[] data)
        {
           
            Source = data[0];
            Destination = data[1];
            dCapacity = data[2];
            int domainflag = Int32.Parse(data[3]);
           
            if(domainflag==1)
            {
                //polaczenie w jednej domenie
                //routetablequery
                //wezel poczatkowy i koncowy sa wezlami dostepowymi z transponderami 
                RouteTableQuery(Source, Destination, dCapacity);
                NewLog($"{CCiPAddress} Get Connection Request from NCC", myWindow);

            }
            else
            {
                //Edge to koniec ściezki
                //polaczenie miedzy domenami
                NewLog($"{CCiPAddress} Get Connection Request from NCC", myWindow);

                TypeEdge = "END";
                RouteTableQuery(Source, Destination, dCapacity, TypeEdge);
            }
        }

        public void KillLinkRequest(string[] data)
        {
            //KillLinkToLRM
        }

        //jedna domena
        public void RouteTableQuery(string startNodeAddress, string endNodeAddress, string demandedCapacity)
        {
            string message = null;
            message = MessageNames.ROUTE_TABLE_QUERY + " " + startNodeAddress + " " + endNodeAddress + " " + demandedCapacity;
            SendingMessage(RCiPAddress, message);

        }

       

        //dwie domeny i jestesmy w drugiej peercoordination
        public void RouteTableQuery(string startNodeAddress, string endNodeAddress, string demandedCapacity, string type, int modulation, int slotnumber)
        {
            string message = null;
            message = MessageNames.ROUTE_TABLE_QUERY + " " + startNodeAddress + " " + endNodeAddress + " " + demandedCapacity+" "+type+" "+modulation+ " "+slotnumber;
            SendingMessage(RCiPAddress, message);

        }

        //dwiedomeny i jestesmy w 1
        public void RouteTableQuery(string startNodeAddress, string endNodeAddress, string demandedCapacity,string type)
        {
            string message = null;
            message = MessageNames.ROUTE_TABLE_QUERY + " " + startNodeAddress + " " + endNodeAddress + " " + demandedCapacity +" "+type;
            SendingMessage(RCiPAddress, message);
           
        }



        //private void SendMessage(IPAddress ipAddress,string message)
        //{
        //    byte[] data = new byte[64];
        //    UdpClient udpClient = new UdpClient();
        //    IPEndPoint sender = new IPEndPoint(ipAddress, 11000);

        //    try
        //    {
        //        data = Encoding.ASCII.GetBytes(message);
        //        udpClient.Send(data, data.Length, sender);

        //    }
        //    catch
        //    {

        //    }
        //    udpClient.Close();
        //}


        public void NewLog(string info, SubnetworkLogs tmp)
        {
            myWindow.Dispatcher.Invoke(() => myWindow.NewLog(info, tmp, color));

        }
    }

}
