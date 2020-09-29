using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class EONBorderNodeRecord
    {
        public int Frequency { get; set; }

        public int Modulation { get; set; }

        public int Bandwidth { get; set; }

        public int NumberHop { get; set; }

        public int Bitrate { get; set; }

        public IPAddress IPIn { get; set; }

        public IPAddress IPOut { get; set; }

        public int PortIn { get; set; }

        public int PortOut { get; set; }

        public EONBorderNodeRecord()
        {

        }

        public EONBorderNodeRecord(string ipi, string ipo, int porti, int porto, int freq, int modu, int band, int bitr, int hops)
        {
            IPIn = IPAddress.Parse(ipi);
            IPOut = IPAddress.Parse(ipo);
            PortIn = porti;
            PortOut = porto;
            Frequency = freq;
            Modulation = modu;
            Bandwidth = band;
            Bitrate = bitr;
            NumberHop = hops;
        }
    }
}