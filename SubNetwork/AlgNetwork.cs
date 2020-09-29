using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class AlgNetwork
    {
        public int countn = 0;
        public int counte = 0;
        public string line;
        public bool wezly = false;
        public bool lacza = false;
        public List<AlgNode> nodes = new List<AlgNode>();
        public List<AlgLink> edges = new List<AlgLink>();
        public void Clearr() 
        {
            edges.Clear();
            nodes.Clear();
            wezly = false;
            lacza = false;
            counte = 0;
            countn = 0;
        }

        public double[,] Convertd() 
        {
            double[,] converted = new double[countn, countn];

            for (int i = 0; i < countn; i++)
            {
                for (int q = 0; q < countn; q++)
                {
                    converted[i, q] = 9999999;
                }
            }
            for (int i = 0; i < countn; i++)
            {
                for (int z = 0; z < counte; z++)
                {
                    if (edges[z].NodeFirst == i + 1)
                    {
                        int p = edges[z].NodeSecond;
                        converted[i, p - 1] = edges[z].Weight;
                        converted[p - 1, i] = edges[z].Weight;
                    }
                }
            }
            return converted;
        }
    }
}
