using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class SlotRecord
    {
        public int PortIn { get; set; }

        public int PortOut { get; set; }

        public int SlotNumber { get; set; }

        public SlotRecord()
        {

        }

        public SlotRecord(int porti, int porto, int slo)
        {
            PortIn = porti;
            PortOut = porto;
            SlotNumber = slo;
        }
    }
}
