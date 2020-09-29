using Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class NetworkPath
    {
        public List<SNPP> SNPPList { get; set; }

        public AlgPath PathAlg { get; set; }

        public NetworkPath()
        {
            SNPPList = new List<SNPP>();
            PathAlg = new AlgPath();
        }

        public NetworkPath(AlgPath tmp)
        {
            SNPPList = new List<SNPP>();
            PathAlg = new AlgPath();
            PathAlg = tmp;
            //tmp.FindNodes(tmp.NodeOne);
            RefreshSNPP();
        }

        public void RefreshSNPP()
        {
            SNPPList = new List<SNPP>();
            //foreach(AlgNode n in PathAlg.)
            //{
            //    SNPP tmpSNPP = new SNPP();
            //    SNP tmpSNP = new SNP(IPAddress.Parse(n.IPNode));
            //    tmpSNPP.SNPList.Add(tmpSNP);
            //    SNPPList.Add(tmpSNPP);
            //}
        }
    }
}
