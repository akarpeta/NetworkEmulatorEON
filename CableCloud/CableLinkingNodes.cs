using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    public class CableLinkingNodes
    {
        public string Node1 { get; set; }

        public string Port1 { get; set; }

        public string Node2 { get; set; }

        public string Port2 { get; set; }

        public string IsWorking { get; set; }

        public CableLinkingNodes(string n1, string p1, string n2, string p2, string working)
        {
            Node1 = n1;
            Port1 = p1;
            Node2 = n2;
            Port2 = p2;
            IsWorking = working;
        }
    }
}

