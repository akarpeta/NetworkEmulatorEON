using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class AlgTopology
    {
        public AlgNetwork AlgNetwork;
        
        public List<AlgLink> potentialLinks;

        public AlgTopology()
        {
            AlgNetwork = new AlgNetwork();
            potentialLinks = new List<AlgLink>();

        }


          
    }
}
