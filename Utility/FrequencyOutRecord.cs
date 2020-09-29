using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class FrequencyOutRecord
    {
        public int Bandwidth { get; set; }

        public int Frequency { get; set; }

        public FrequencyOutRecord(int band, int freq)
        {
            Bandwidth = band;
            Frequency = freq;
        }
    }
}
