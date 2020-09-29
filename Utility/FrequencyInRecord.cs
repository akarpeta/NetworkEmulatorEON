using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class FrequencyInRecord
    {
        public int Bandwidth { get; set; }

        public int Frequency { get; set; }

        public FrequencyInRecord(int band, int freq)
        {
            Bandwidth = band;
            Frequency = freq;
        }
    }
}
