using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class AlgNode
    {
        public int IdNode { get; set; }

        public int ClientNumber { get; set; }

        public AlgNode NodeFrom { get; set; }

        public float Label { get; set; }

        public bool IsVisited { get; set; }

        public List<AlgLink> LinksList = new List<AlgLink>();

        public string IPNode { get; set; }

        //public SNPP SNPPool { get; set; }

        public AlgNode()
        {
            IdNode = 0;
            NodeFrom = null;
            Label = 0;
            IsVisited = false;
            IPNode = "";
            LinksList = new List<AlgLink>();
           // SNPPool = new SNPP(new SNP(IPAddress.Any));
        }

        public AlgNode(int id)
        {
            IdNode = id;
            NodeFrom = null;
            Label = 0;
            IsVisited = false;
            IPNode = "";
            LinksList = new List<AlgLink>();
           // SNPPool = new SNPP(new SNP(IPAddress.Any));
        }

        public AlgNode(int id, string ip) 
        {
            IdNode = id;
            IPNode = ip;
            //SNPPool = new SNPP(new SNP(IPAddress.Parse(ip)));
            NodeFrom = null;
            Label = 0;
            IsVisited = false;
            LinksList = new List<AlgLink>();
        }
    }
}
