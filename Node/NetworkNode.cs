using System.Collections.Generic;
using System.Net;
using Utility;

namespace Node
{
    public class NetworkNode
    {
        public List<NetworkNode> ListNode = new List<NetworkNode>();

        public IPAddress CloudIP { get; set; }

        public ushort CloudPort { get; set; }

        public IPAddress ManagementIP { get; set; }
        
        public IPAddress ListenIPAddress { get; set; }

        public IPAddress IPAddress { get; set; }

        public IPAddress CCIPAddress { get; set; }
        public ushort ManagementPort { get; set; }

        public string NodeName { get; set; }

        public IPAddress RCIPAddres { get; set; }

        public int ID { get; set; }

        public BehaviourOfPacket PacketLogic { get; set; }

        public TablesInNode RoutingTables { get; set; }

        public MySocket NodeSocket { get; set; }

        public NetworkNode networknode;

        public int HostPort = -1;

        public int EdgePort = -1;

        public List<RouterMapRecord> routerMapRecords = new List<RouterMapRecord>();

        public NetworkNodeWindow Window { get; set; }

        public NetworkNode(NetworkNodeWindow myWindow)
        {
            Window = myWindow;
            PacketLogic = new BehaviourOfPacket(myWindow);
            RoutingTables = new TablesInNode(myWindow);
        }
    }
}
