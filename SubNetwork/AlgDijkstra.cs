using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
 
      //JAK KORZYSTAC
      //AlgNetwork network = new AlgNetwork();
      //xml na wypelnienie tablic: nodes, edges
      //int poczDij = 1;
      //int konDij = 4;
      //int pocz1 = poczDij - 1;
      //int kon1 = konDij - 1;      bo nazewnictwo id od 1 a nie 0
      //var dij = new Dijkstra(network.Convertd());
      //var path = dij.makePath(pocz1, kon1);       tablica intow zwraca id nodeow (od 0!!!!)
      //double dlugoscDrogi = dij.DistanceFinal;


    public class AlgDijkstra
    {
        public double DistanceFinal { get; set; }
        public const double maxd = 9999999; 
        public const int ii = 9999999;
        public double[,] net;

        public AlgDijkstra(double[,] n) 
        {
            net = n;
        }

        public int[] makePath(int start, int end)
        {
            int netsize = net.GetLength(0);
            double[] dist = new double[netsize]; 
            int[] previous = new int[netsize]; 
            int[] nodes = new int[netsize];

            for (int i = 0; i < dist.Length; i++) 
            {
                dist[i] = maxd;
                previous[i] = ii;
                nodes[i] = i;
            }

            dist[start] = 0; 
            do
            {
                int small = nodes[0];
                int sindeks = 0;
                for (int i = 1; i < netsize; i++)
                {
                    if (dist[nodes[i]] < dist[small]) 
                    {
                        small = nodes[i];
                        sindeks = i;
                    }
                }
                netsize--;
                nodes[sindeks] = nodes[netsize];

                if (dist[small] == maxd || small == end) 
                    break;

                for (int i = 0; i < netsize; i++)
                {
                    int v = nodes[i];
                    double newdist = dist[small] + net[small, v];
                    if (newdist < dist[v]) 
                    {
                        dist[v] = newdist;
                        previous[v] = small;
                    }
                }
            } while (netsize > 0);
            return path(previous, start, end, dist[end]); 
        }

        public int[] path(int[] previous, int start, int end, double dista)
        {
            DistanceFinal = dista;
            int[] draw = new int[previous.Length];
            int currn = 0;
            draw[currn] = end;

            while (draw[currn] != maxd && draw[currn] != start)
            {
                draw[currn + 1] = previous[draw[currn]];
                currn++;
            }
            if (draw[currn] != start)
                return null;
            int[] pathdij = new int[currn + 1];
            for (int i = currn; i >= 0; i--)
            {
                pathdij[currn - i] = draw[i];
            }
            //ResultDij = Tuple.Create(pathdij, dista);
            //return ResultDij
            return pathdij;
        }

        //public double makePathDist(int start, int end)
        //{
        //    int netsize = net.GetLength(0);
        //    double[] dist = new double[netsize];
        //    int[] previous = new int[netsize];
        //    int[] nodes = new int[netsize];

        //    for (int i = 0; i < dist.Length; i++)
        //    {
        //        dist[i] = maxd;
        //        previous[i] = ii;
        //        nodes[i] = i;
        //    }

        //    dist[start] = 0;
        //    do
        //    {
        //        int small = nodes[0];
        //        int sindeks = 0;
        //        for (int i = 1; i < netsize; i++)
        //        {
        //            if (dist[nodes[i]] < dist[small])
        //            {
        //                small = nodes[i];
        //                sindeks = i;
        //            }
        //        }
        //        netsize--;
        //        nodes[sindeks] = nodes[netsize];

        //        if (dist[small] == maxd || small == end)
        //            break;

        //        for (int i = 0; i < netsize; i++)
        //        {
        //            int v = nodes[i];
        //            double newdist = dist[small] + net[small, v];
        //            if (newdist < dist[v])
        //            {
        //                dist[v] = newdist;
        //                previous[v] = small;
        //            }
        //        }
        //    } while (netsize > 0);
        //    return path2(previous, start, end, dist[end]);
        //}

        //public double path2(int[] previous, int start, int end, double dista)
        //{
        //    DistanceFinal = dista;
        //    int[] draw = new int[previous.Length];
        //    int currn = 0;
        //    draw[currn] = end;

        //    while (draw[currn] != maxd && draw[currn] != start)
        //    {
        //        draw[currn + 1] = previous[draw[currn]];
        //        currn++;
        //    }
        //    if (draw[currn] != start)
        //        return 0;
        //    int[] pathdij = new int[currn + 1];
        //    for (int i = currn; i >= 0; i--)
        //    {
        //        pathdij[currn - i] = draw[i];
        //    }
        //    //ResultDij = Tuple.Create(pathdij, dista);
        //    //return ResultDij
        //    return DistanceFinal;
        //}
    }
}
