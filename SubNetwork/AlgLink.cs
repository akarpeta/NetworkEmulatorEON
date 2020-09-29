using Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class AlgLink
    {
        public int IdLink { get; set; }

        public int NodeFirst { get; set; }

        public int NodeSecond { get; set; }

        public AlgNode NodeOne { get; set; }

        public AlgNode NodeTwo { get; set; }

        public double Weight { get; set; }

        public AlgLink(int id, int first, int second)
        {
            IdLink = id;
            NodeFirst = first;
            NodeSecond = second;
            Weight = 0;
        }

        public AlgLink(int id, AlgNode one, AlgNode two, double wei)
        {
            IdLink = id;
            NodeOne = one;
            NodeTwo = two;
            NodeFirst = one.IdNode;
            NodeSecond = two.IdNode;
            Weight = wei;

            //int band = (int)(64 - Math.Sqrt(wei));

            //int firstSNPid;

            //int secondSNPid;


            //if (NodeOne.SNPPool.SNPList[0].PortO == -1)
            //{
            //    firstSNPid = 0;
            //    NodeOne.SNPPool.SNPList[0].PortO = IdLink;
            //}
            //else
            //{
            //    //Odnalezienie węzła z dołączonym interfejsem
            //    firstSNPid = this.NodeOne.SNPPool.SNPList.FindIndex(x => x.PortO == IdLink);
            //}

            //if (NodeTwo.SNPPool.SNPList[0].PortO == -1)
            //{
            //    secondSNPid = 0;
            //    NodeTwo.SNPPool.SNPList[0].PortO = IdLink;
            //}
            //else
            //{
            //    //Odnalezienie węzła z dołączonym interfejsem
            //    secondSNPid = this.NodeTwo.SNPPool.SNPList.FindIndex(x => x.PortO == IdLink);
            //}

            ////Jeżeli węzeł nie ma doprowadzonego takiego łącza
            //if (firstSNPid == -1)
            //{
            //    //Dodanie nowego interfejsu do wezla 1
            //    NodeOne.SNPPool.SNPList.Add(new SNP(IPAddress.Parse(NodeOne.IPNode), -1, IdLink));
            //    firstSNPid = NodeOne.SNPPool.SNPList.Count - 1;
            //}

            ////Jeżeli węzeł nie ma doprowadzonego takiego łącza
            //if (secondSNPid == -1)
            //{
            //    //Dodanie nowego interfejsu do wezla 1
            //    NodeTwo.SNPPool.SNPList.Add(new SNP(IPAddress.Parse(NodeTwo.IPNode), IdLink, -1));
            //    secondSNPid = NodeTwo.SNPPool.SNPList.Count - 1;
            //}

            //if (64 >= Math.Sqrt(Weight))
            //{
            //    //Na wyjsciu wezla 1   
            //    //this.NodeOne.SNPPool.SNPList[firstSNPid].slTable
            //    //    .addRow(new EONTableRowOut(0, (short)(64- band)));
            //    ////Na wejsciu wezla 2
            //    //this.Wezel2.SNPP.snps[SNP2Index].eonTable
            //    //    .addRow(new EONTableRowIN(0, (short)(EONTable.capacity - band)));

            //    Weight = wei;
            //}
        }


        //public AlgLink(int id, AlgNode one, AlgNode two, int band, int freq)
        //{
        //    IdLink = id;
        //    NodeOne = one;
        //    NodeTwo = two;
        //    NodeFirst = one.IdNode;
        //    NodeSecond = two.IdNode;

        //    int firstSNPid;

        //    int secondSNPid;

        //    if (NodeOne.SNPPool.SNPList[0].PortO == -1)
        //    {
        //        firstSNPid = 0;
        //        NodeOne.SNPPool.SNPList[0].PortO = IdLink;
        //    }
        //    else
        //    {
        //        //Odnalezienie węzła z dołączonym interfejsem
        //        firstSNPid = this.NodeOne.SNPPool.SNPList.FindIndex(x => x.PortO == IdLink);
        //    }

        //    if (NodeTwo.SNPPool.SNPList[0].PortO == -1)
        //    {
        //        secondSNPid = 0;
        //        NodeTwo.SNPPool.SNPList[0].PortO = IdLink;
        //    }
        //    else
        //    {
        //        //Odnalezienie węzła z dołączonym interfejsem
        //        secondSNPid = this.NodeTwo.SNPPool.SNPList.FindIndex(x => x.PortO == IdLink);
        //    }

        //    if (firstSNPid == -1)
        //    {
        //        //Dodanie nowego interfejsu do wezla 1
        //        NodeOne.SNPPool.SNPList.Add(new SNP(IPAddress.Parse(NodeOne.IPNode), -1, IdLink));
        //        firstSNPid = NodeOne.SNPPool.SNPList.Count - 1;
        //    }

        //    //Jeżeli węzeł nie ma doprowadzonego takiego łącza
        //    if (secondSNPid == -1)
        //    {
        //        //Dodanie nowego interfejsu do wezla 1
        //        NodeTwo.SNPPool.SNPList.Add(new SNP(IPAddress.Parse(NodeTwo.IPNode), IdLink, -1));
        //        secondSNPid = NodeTwo.SNPPool.SNPList.Count - 1;
        //    }

        //    if (64 >= band)
        //    {
        //        ////Na wyjsciu wezla 1   
        //        //this.Wezel1.SNPP.snps[SNP1Index].eonTable.addRow(new EONTableRowOut(freq, band));
        //        ////Na wejsciu wezla 2
        //        //this.Wezel2.SNPP.snps[SNP2Index].eonTable.addRow(new EONTableRowIN(freq, band));

        //        Weight = (double)Math.Pow(band, 2);
        //    }
        //}

        //public AlgLink(int id, AlgNode one, AlgNode two)  : this(id, one, two, 0, 0)
        //{

        //}

        //public void updateCost(short frequency, short band)
        //{
        //    //Odnalezienie indeksu SNP, który jest przypisan do tego łącza
        //    int snpIndex2 = this.Wezel2.SNPP.snps.FindIndex(x => x.portIN == this.idKrawedzi);
        //    int snpIndex1 = this.Wezel1.SNPP.snps.FindIndex(x => x.portOUT == this.idKrawedzi);

        //    //Jezeli sie udalo znalezc indeksy
        //    if (snpIndex1 != -1 && snpIndex2 != -1)
        //    {
        //        //Jeżeli odnaleziony SNP w węźle posiada wpis do tablicy eonowej o takim samym paśmie i częstotliwośći, co w rowOut
        //        if (this.Wezel2.SNPP.snps[snpIndex2].eonTable.TableIN.FindIndex(y => y.busyBandIN == band && y.busyFrequency == frequency) != -1
        //            && this.Wezel1.SNPP.snps[snpIndex1].eonTable.TableOut.FindIndex(y =>
        //                y.busyBandOUT == band && y.busyFrequency == frequency) != -1)
        //        {
        //            int sum = 0;
        //            var SNP = Wezel1.SNPP.snps.Find(x => x.portOUT == this.idKrawedzi);

        //            foreach (short value in SNP.eonTable.OutFrequencies)
        //            {
        //                if (value != -1)
        //                    sum++;
        //            }

        //            //Całe zajęte pasmo do kwadratu to waga
        //            this.Waga = (double)(Math.Pow(sum, 2));
        //        }
        //    }
        //}
    }
}
