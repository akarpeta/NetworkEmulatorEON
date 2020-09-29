using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class EONNodeRecord
    {
        public int PortIn { get; set; }

        public int PortOut { get; set; }

        public int FrequencyIn { get; set; }

        public int FrequencyOut { get; set; }

        public EONNodeRecord()
        {

        }

        public EONNodeRecord(int porti, int porto, int freqi, int freqo)
        {
            PortIn = porti;
            PortOut = porto;
            FrequencyIn = freqi;
            FrequencyOut = freqo;
        }
    }
}
