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
    //klasa dla latwiejszego operowania recordami SNP
    public class MessageSNP
    {
        public int PortIn { get; set; }

        public int PortOut { get; set; }

        public int FirstSlotID { get; set; }

        public int ModNumber { get; set; }

        public bool IsUsed { get; set; }

        public MessageSNP(int porti, int porto)
        {
            PortIn = porti;
            PortOut = porto;
            IsUsed = false;
            ModNumber = -1;
            FirstSlotID = -1;
        }

        public MessageSNP(int porti, int porto, int mod, int slo)
        {
            PortIn = porti;
            PortOut = porto;
            ModNumber = mod;
            FirstSlotID = slo;
            IsUsed = true;
        }

    }

    public class RC
    {
        public Domain DomainRC { get; set; }

        public List<Domain> DomainList { get; set; }

        public IPAddress IPRC { get; set; }

        public int IdRC { get; set; }

        public IPAddress IPCC { get; set; }

        public AlgTopology Topo { get; set; }

        public int PortRC { get; set; }

        public volatile IPEndPoint ipEndP;

        public volatile UdpClient socketMy;

        //node id ip address
        public Dictionary<int, string> nodeDict = new Dictionary<int, string>();
        //record
        public List<string> SNPLinkNode = new List<string>();
        //Szuka routera brzegowego po IP hosta
        public Dictionary<IPAddress, int> HostNodes = new Dictionary<IPAddress, int>();

        public IPAddress other_RC { get; set; }

        //id routera brzegowego do polaczenia z 2 domena
        public int EdgeRouter { get; set; }

        public IPAddress destinationIP;

        //path result dla jednej domeny
        public int[] path { get; set; }

        //odpowiedzi path_node record
        public List<int>[] Responses;

        public int Modulation { get; set; }
        public int SlotNumber { get; set; }

        //0 w jednej domenie 1 w 2 domenach
        public bool FlagdDomain;


        public SubnetworkLogs myWindow;

        public string color = "LightSalmon";      
        public RC(int nr, SubnetworkLogs tmp)
        {
            

            myWindow = tmp;
            IdRC = nr;
            XmlReader xmlReader = new XmlReader("RC" + IdRC + ".xml");
            IPRC = IPAddress.Parse(xmlReader.GetAttributeValue("dRC", "IP_ADDRESS"));
            IPCC = IPAddress.Parse(xmlReader.GetAttributeValue("dRC", "IP_ADDRESS_CC"));
            other_RC =IPAddress.Parse(xmlReader.GetAttributeValue("other_RC", "IP_ADDRESS_OTHER_RC"));
            EdgeRouter = Int32.Parse(xmlReader.GetAttributeValue("dRC", "EDGE"));

            int numberofhosts = xmlReader.GetNumberOfItems("host");
            for(int a=0; a<numberofhosts;a++)
            {
                IPAddress ip = IPAddress.Parse(xmlReader.GetAttributeValue(a,"host", "IP_ADDRESS"));
                int id = Int32.Parse(xmlReader.GetAttributeValue(a, "host", "ROUTER"));
                HostNodes.Add(ip, id);
            }

            Topo = new AlgTopology();
            DomainRC = new Domain();
            DomainList = new List<Domain>();
            PortRC = 11000;     //????????????????
            GetNodes("RC" + IdRC + ".xml");           //czytanie nodesow do topo z xml
            GetLink("RC" + IdRC + ".xml");          //czytanie linkow do topo z xml  
                                                    //GetRCSNPP("RC" + IdRC + ".xml");    //czytanie par ip;rc do listy z xml   
            ipEndP = new IPEndPoint(IPRC, PortRC);
            socketMy = new UdpClient(ipEndP);
        }

        public void GetLink(string path)
        {
            XmlReader xmlReader = new XmlReader(path);
            int numberoflinks = xmlReader.GetNumberOfItems("connect");
            this.Topo.AlgNetwork.counte = numberoflinks;
            for (int a = 0; a < numberoflinks; a++)
            {
                var node_name1 = Int32.Parse(xmlReader.GetAttributeValue(a, "connect", "NODE1"));
                var node_name2 = Int32.Parse(xmlReader.GetAttributeValue(a, "connect", "NODE2"));
                var path_id = Int32.Parse(xmlReader.GetAttributeValue(a, "connect", "PATH_ID"));
                var distance = Double.Parse(xmlReader.GetAttributeValue(a, "connect", "DROGA"));
                var port1 = UInt16.Parse(xmlReader.GetAttributeValue(a, "connect", "PORTIN"));
                var port2 = UInt16.Parse(xmlReader.GetAttributeValue(a, "connect", "PORTOUT"));

                AlgNode no = this.Topo.AlgNetwork.nodes.Find(x => x.IdNode == node_name1);
                AlgNode nd = this.Topo.AlgNetwork.nodes.Find(x => x.IdNode == node_name2);
                AlgLink algLink = new AlgLink(path_id, no, nd, distance);
                this.Topo.AlgNetwork.edges.Add(algLink);
            }

            int numberofconnectr = xmlReader.GetNumberOfItems("connect_r");
            for (int a = 0; a < numberofconnectr; a++)
            {
                var node_name1 = Int32.Parse(xmlReader.GetAttributeValue(a, "connect_r", "NODE1"));
                var node_name2 = Int32.Parse(xmlReader.GetAttributeValue(a, "connect_r", "NODE2"));
                var port1 = UInt16.Parse(xmlReader.GetAttributeValue(a, "connect_r", "PORTIN"));
                var port2 = UInt16.Parse(xmlReader.GetAttributeValue(a, "connect_r", "PORTOUT"));

                //SNPLinkNode.Add(node_name1 + ":" + node_name2, port1 + ":" + port2);
                //SNPLinkNode.Add(node_name2 + ":" + node_name1, port2 + ":" + port1);

                SNPLinkNode.Add(node_name1 + ":" + port1+":"+node_name2 + ":" + port2);
                SNPLinkNode.Add(node_name2 + ":" + port2 + ":" + node_name1 + ":" + port1);
            }
        }

      

        public void GetNodes(string path)
        {
            XmlReader xmlReader = new XmlReader(path);
            int numberofnodes = xmlReader.GetNumberOfItems("router");

            this.Topo.AlgNetwork.countn = numberofnodes;

            for (int a = 0; a < numberofnodes; a++)
            {
                var node_id = Int32.Parse(xmlReader.GetAttributeValue(a, "router", "ID"));
                var node_ip_address = xmlReader.GetAttributeValue(a, "router", "IP_ADDRESS");

                AlgNode node = new AlgNode(node_id, node_ip_address);
                this.Topo.AlgNetwork.nodes.Add(node);
                nodeDict.Add(node_id, node_ip_address);
            }

        }
        public void Run()
        {
            Task.Run(() =>
            {

              
                byte[] bytes = new byte[64];
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    while (true)
                    {
                        bytes = socketMy.Receive(ref sender);
                        string receivedMesage = Encoding.ASCII.GetString(bytes);
                        var action = receivedMesage.Split(null)[0];
                        var tmp = receivedMesage.Replace($"{action} ", "");
                        var data = tmp.Split(' ');
                        Task.Run(() => ActionSelect(action, data));
                    }
                }
                catch (Exception E)
                {
                    NewLog("RoutingController.Run(): " + E.Message,myWindow);
                }
            });
        }

        public void ActionSelect(string act, string[] msg)
        {
            NewLog($"ACTION: {act}", myWindow);
           
            switch (act)
            {
                case MessageNames.ROUTE_TABLE_QUERY:
                    RouteTableQueryReq(msg);
                    break;
                case MessageNames.ROUTE_PATH:
                    RoutePathReq(msg);
                    break;
                case MessageNames.LOCAL_TOPOLOGY:
                    LocalTopologyReq(msg);
                    break;
                case MessageNames.NETWORK_TOPOLOGY:
                    NetworkTopologyReq(msg);
                    break;
                case MessageNames.NETWORK_TOPOLOGY_RESPONSE:
                    NetworkTopologyRsp(msg);
                    break;
                case MessageNames.KILL_LINK:
                    KillLinkReq(msg);
                    break;
                case MessageNames.ALLOCATE:
                    //LinkAllocationFromLRM(msg);
                    HandleAllocation();
                    break;
                case MessageNames.HANDLEAVAIBLESLOT_RESPONSE:
                    HandleAvaibleSlotsInNode(msg);
                    break;
            }
        }

        private int FindIndexOfNodeInPath(int nodeId)
        {
            for(int i = 0; i < path.Length; i++)
            {
                if (path[i] == nodeId)
                    return i;
            }
            return -1;
        }
        
        private bool AreAllResponsesReceivevd()
        {
            foreach(var item in Responses)
            {
                //TODO: TEST
                if (item == null)
                    return false;
            }
            return true;
        }

        private bool FindSlotWindow(List<int> slots, ref SlotWindow outSlotWindow)
        {
            int consecutive = 1;
            for (int i = 0; i < slots.Count; i++)
            {
                bool areConsecutive = (i > 0) && (slots[i] == (slots[i - 1] + 1));
                if (areConsecutive)
                {
                    consecutive++;
                }
                else
                {
                    consecutive = 1;
                }
                if (consecutive == SlotNumber)
                {
                    outSlotWindow.NumberofSlots = SlotNumber;
                    outSlotWindow.FirstSlot = slots[i + 1 - consecutive];
                    return true;
                }
            }
            return false;
        }

        private void SentNewRouterRecord(SlotWindow slotWindow)
        {
            try
            {
                NewLog($"SentNewRouterRecord path.Length {path.Length}", myWindow);
                for (int i = 0; i < path.Length; i++)
                {
                    NewLog($"SentNewRouterRecord {i}", myWindow);
                    int nextNode = -1;
                    if (i < (path.Length - 1))
                    {
                        nextNode = path[i + 1];
                    }
                    RouterMapRecord newRecord = new RouterMapRecord(slotWindow, nextNode);
                    string str = XmlSerialization.SerializeObject(newRecord);
                    string message = MessageNames.ADD_RECORD + " " + str;
                    string ipaddress = nodeDict[path[i]];
                    SendMessage(ipaddress, message);

                }
            }
            catch (Exception E)
            {
                NewLog("SentNewRouterRecord : " + E.Message, myWindow);
            }
        }

        //idroutera
        private void HandleAvaibleSlotsInNode(string[] msg)
        {
            string resultstring = XmlSerialization.GetStringToNormal(msg);

            MessageAvaibleSlotsInNode messageAvaibleSlots = XmlSerialization.DeserializeObject<MessageAvaibleSlotsInNode>(resultstring);
            int index = FindIndexOfNodeInPath(messageAvaibleSlots.ID);
            Responses[index] = messageAvaibleSlots.AvaibleSlots;
            NewLog($"HandleAvaibleSlotsNode {index}", myWindow);

            if (AreAllResponsesReceivevd() && (Responses.Length > 0))
            {
                NewLog($"Areallresponsereceived", myWindow);
                NewLog($"Responses {Responses.Length}", myWindow);

                List<int> unionOfSlots = Responses[0];
                for (int i = 1; i < Responses.Length; i++)
                {
                    for (int j = 0; j < unionOfSlots.Count; )
                    {
                        if (Responses[i].Contains(unionOfSlots[j]))
                        {
                            j++;
                        }
                        else
                        {
                            unionOfSlots.RemoveAt(j);
                        }
                    }
                }

                string unionStr = "";
                foreach(int i in unionOfSlots)
                {
                    unionStr += " " + i;
                }
                NewLog($"SlotNumber: {SlotNumber}", myWindow);
                NewLog($"Union[{unionOfSlots.Count}]: {unionStr}", myWindow);
                Responses = null;

                SlotWindow slotWindow = new SlotWindow();

                if(FindSlotWindow(unionOfSlots, ref slotWindow))
                {
                    NewLog($"Findslotwindow {slotWindow.FirstSlot} {slotWindow.NumberofSlots}", myWindow);
                    SentNewRouterRecord(slotWindow);

                }
                else
                {
                    //Zaloguj brak dostępnych slotow

                    NewLog($"Nie znaleziono", myWindow);

                }
            }
        }

        //pierwsze zapytania do routera
        //wysylamy wiadomosci do kazdego routera na sciezce
        public void HandleAllocation()
        {
            NewLog("Handle Allocation",myWindow);

            Responses = new List<int>[path.Length];

            string message = null;
            message = MessageNames.HANDLEAVAIBLESLOTREQUEST;
            foreach(int id in path)
            {
                string ipaddress = nodeDict[id];
                SendMessage(ipaddress,message);
            }
        }

        public void NetworkTopologyRsp(string[] msg)
        {
            NewLog($"{IPRC} Get NetworkTopology Response from {other_RC}", myWindow);
            int startNode = HostNodes[IPAddress.Parse(msg[0])];
            int endNode = EdgeRouter;
            string dCapacity = msg[2];
            int distance = Int32.Parse(msg[3]);
            CreatePath(startNode, endNode, dCapacity, distance,IdRC);
            //NewLog($"{IPRC} Start AlgDijkstry {path.Length} {path[0]} {path[1]} {path[2]}", myWindow);
            double length = CreatePath(startNode, endNode, dCapacity, 0, IdRC);
            Modulation = GetModulation(length+distance);
            SlotNumber = GetSlotNumber(Int32.Parse(dCapacity), Modulation);

            //zaalokuj sciezki potem wypiszemy logi
            //LinkAllocationRequest(path);
            //HandleAllocation();
            // Send message to CC: zrobiona sciezka zrob peercoordination
            //source dest path[] dCapacity
            NewLog($"{IPRC} Dlugosc: {length + distance} Modulacja: {Modulation} Slot {SlotNumber}", myWindow);
            RouteTableQueryReqRsp(msg[0], msg[1], msg[2],path,Modulation, SlotNumber);
            
        }

        //Obsługa RouteTableQuery
        public void RouteTableQueryReq(string[] msg)
        {
            NewLog($"{IPRC} Get RouteTableQuery from CC", myWindow);

            destinationIP = IPAddress.Parse(msg[1]);

            int startNode;
            int endNode;
            string dCapacity = msg[2];
            if (msg.Length == 3)
            {
                FlagdDomain = false;

                //dijkstry jedna domena

                startNode = HostNodes[IPAddress.Parse(msg[0])];
                //nodeDict[msg[0]];
                endNode = HostNodes[IPAddress.Parse(msg[1])];

                //Proba wyznaczenia ściezki
                double distance = CreatePath(startNode, endNode,dCapacity,0, IdRC);
                Modulation = GetModulation(distance);
                SlotNumber = GetSlotNumber(Int32.Parse(dCapacity), Modulation);
               
                NewLog($"{IPRC} Get modulation: {Modulation} slots: {SlotNumber} {msg[0]},{msg[1]},{msg[2]}", myWindow);
                
                RouteTableQueryReqRsp(msg[0],msg[1],msg[2],path,Modulation,SlotNumber);
                
            }
            else if (msg.Length == 4)
            {
                FlagdDomain = true;
                //dijkstry dwie domeny
              
                   
                    //komunikujemy się z drugim RC zeby wyznaczyc efektywnosc modulacje
                    //start end dCapacity iprc(gdzie odeslac potem) edge 
   
                    SendNetworkTopologyReq(msg[0], msg[1], msg[2], IPRC);
                    //wyznacz modulacje 
                    //wyznacz szczeliny
                    //wyslij informacje do CC zeby zajal szczeliny
                    //RouteTableQueryResponse
                    //SendMessage(CCiP,message)
            }else if(msg.Length == 6)
            {
                FlagdDomain = true;
                //from edge to end
                startNode = EdgeRouter;
                endNode = HostNodes[IPAddress.Parse(msg[1])];
                Modulation = Int32.Parse(msg[4]);
                SlotNumber = Int32.Parse(msg[5]);
                CreatePath(startNode, endNode, dCapacity, 0, IdRC);
                NewLog($"{IPRC} modulation: {Modulation} {SlotNumber}", myWindow);
                RouteTableQueryReqRsp(msg[0], msg[1], msg[2], path, Modulation, SlotNumber);
            }
        }

        //source dest path[] 
        private void RouteTableQueryReqRsp(string source, string destination, string dCapacity,int[] path,int modulation,int slotnumber)
        {
          
            //sciezka id nodów
            string data = null;
            for (int a = 0; a < path.Length; a++)
            {
                data += path[a].ToString() + " ";
            }
            string message = null;
       
            message = MessageNames.ROUTE_TABLE_QUERY_RSP+" "+source + " " + destination +" "+dCapacity+" "+ Convert.ToInt32(FlagdDomain)+" "+modulation+" "+slotnumber+" "+path.Length+" "+data;
           
            SendMessage(IPCC,message);
        }

        //Wyznacza siezke i wysyla do CC
        public double CreatePath(int startNodeid, int endNodeid, string dCapacity, int distance, int IdRC)
        {
            double length;

           
            if (IdRC==101)
            {
               
                int pocz = startNodeid - 1;
                int kon = endNodeid - 1;
                var di = new AlgDijkstra(Topo.AlgNetwork.Convertd());
                path = di.makePath(pocz, kon);
                //double path2 = di.makePathDist(pocz, kon);

                for(int a=0; a<path.Length; a++)
                {
                    path[a] = path[a] + 1;
                }
               length = di.DistanceFinal;
            }
            else
            {
               
                for (int a=0; a < Topo.AlgNetwork.nodes.Count;a++)
                {
                    Topo.AlgNetwork.nodes[a].IdNode = Topo.AlgNetwork.nodes[a].IdNode - 5;
                }

                for(int a=0; a<Topo.AlgNetwork.edges.Count;a++)
                {
                    Topo.AlgNetwork.edges[a].NodeFirst = Topo.AlgNetwork.edges[a].NodeFirst - 5;
                    Topo.AlgNetwork.edges[a].NodeSecond = Topo.AlgNetwork.edges[a].NodeSecond - 5;
                }

                int pocz = startNodeid - 6;
                int kon = endNodeid - 6;
                var di = new AlgDijkstra(Topo.AlgNetwork.Convertd());
                path = di.makePath(pocz, kon);

                for (int a = 0; a < path.Length; a++)
                {
                    path[a] = path[a] + 6;
                }
                length = di.DistanceFinal;

                for (int a = 0; a < Topo.AlgNetwork.nodes.Count; a++)
                {
                    Topo.AlgNetwork.nodes[a].IdNode = Topo.AlgNetwork.nodes[a].IdNode + 5;
                }

                for (int a = 0; a < Topo.AlgNetwork.edges.Count; a++)
                {
                    Topo.AlgNetwork.edges[a].NodeFirst = Topo.AlgNetwork.edges[a].NodeFirst + 5;
                    Topo.AlgNetwork.edges[a].NodeSecond = Topo.AlgNetwork.edges[a].NodeSecond + 5;
                }
               
            }
           
            for(int a=0; a<path.Length;a++)
            {
                //NewLog($"Create path length {path[a]}", myWindow);
            }
            return length;
        }

        //Wiadomosc do RC o zwrocenie odleglosci w jego domenie
        public void SendNetworkTopologyReq(string source, string destination, string dCapacity,IPAddress address)
        {
            NewLog($"{IPRC} SendNetworkTopologyReq to {other_RC} [{source} {destination} {dCapacity}]", myWindow);
            string message = null;
            message =MessageNames.NETWORK_TOPOLOGY+" "+source + " " + destination + " " + dCapacity+" "+ address.ToString();
            SendMessage(other_RC,message);
        }

        
        public void NetworkTopologyReq(string[] message)
        {
            NewLog($"{IPRC} Get NetworkTopologyReq", myWindow);
            double distance = 0;
            //algorytm dijsktry zwraca odleglosc w km sciezki
            int destin = HostNodes[IPAddress.Parse(message[1])];
            string dCapacity = message[2];
            string addrestosend = message[3];

            distance = CreatePath(EdgeRouter, destin, dCapacity,0,IdRC);
            NewLog($"{IPRC} Send NetworkTopologyReq Response length:{distance}", myWindow);
            string data1 = null;
            data1 = MessageNames.NETWORK_TOPOLOGY_RESPONSE + " "+message[0] + " "+ message[1] + " "+ message[2] + " "+distance;
            SendMessage(addrestosend, data1);
        }

        private void RoutePathReq(string[] msg)
        {
            throw new NotImplementedException();
        }

        private void LocalTopologyReq(string[] msg)
        {
            throw new NotImplementedException();
        }

      
        private void KillLinkReq(string[] msg)
        {
            throw new NotImplementedException();
        }

        private void GetPath(string[] msg)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string ipaddress, string message)          //wysylanie wiadomosci
        {
            byte[] data = new byte[64];
            UdpClient newsock = new UdpClient();
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ipaddress), 11000);

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

        public int GetModulation(double dist)
        {
            if (dist > 0 && dist < 100)
            {
                return 6;
            }
            if (dist > 100 && dist < 200)
            {
                return 5;
            }
            if (dist > 200 && dist < 300)
            {
                return 4;
            }
            if (dist > 300 && dist < 400)
            {
                return 3;
            }
            if (dist > 400 && dist < 600)
            {
                return 2;
            }
            return 1;
        }

        public int GetSlotNumber(int bitrate, int mod)
        {
            int a = bitrate / 2;
            int b = (int)(a / mod);
            int c = b * 2;
            double d = (double)(c + 5);
            double e = d / 12.5;
            int f = (int)Math.Ceiling(e);
            return f;
        }

        

        public void NewLog(string info, SubnetworkLogs tmp)
        {
            myWindow.Dispatcher.Invoke(() => myWindow.NewLog(info, tmp, color));

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  
        //list z adresami ip potrzebna do komunikacji z nodem zeby dostac slot tablice
        List<string> ipNaSztywno = new List<string>();
        //ipNaSztywno.Add(192168...)

        List<AlgLink> dijEdges = new List<AlgLink>();

        public void LinkAllocationRequest(int[] dijpath)
        {
            for (int i = 0; i < dijpath.Length - 1; i++)
            {
                bool tmp = true;
                for (int i1 = 0; i1 < this.Topo.AlgNetwork.edges.Count; i1++)
                {
                    if(tmp == true)
                    {
                        if (this.Topo.AlgNetwork.edges[i1].NodeFirst == dijpath[i] && this.Topo.AlgNetwork.edges[i1].NodeSecond == dijpath[i + 1])
                        {
                            dijEdges.Add(this.Topo.AlgNetwork.edges[i1]);
                            tmp = false;
                            break;
                        }
                        if (this.Topo.AlgNetwork.edges[i1].NodeSecond == dijpath[i] && this.Topo.AlgNetwork.edges[i1].NodeFirst == dijpath[i + 1])
                        {
                            dijEdges.Add(this.Topo.AlgNetwork.edges[i1]);
                            tmp = false;
                            break;
                        }

                    }
                }
            }
            //NewLog($"{IPRC} znalezlismy 2 linki id: {dijEdges.Count} {dijEdges[0].IdLink} {dijEdges[1].IdLink}", myWindow);


        }








    }
}
