using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node
{
    public class SNP
    {
        public int PortIn { get; set; }

        public int PortOut { get; set; }

        public int FirstSlotID { get; set; }

        public int ModNumber { get; set; }

        public bool IsUsed { get; set; }

        public SNP()
        {

        }

        public SNP(int porti, int porto)
        {
            PortIn = porti;
            PortOut = porto;
            IsUsed = false;
            ModNumber = -1;
            FirstSlotID = -1;
        }

        public SNP(int porti, int porto, int mod, int slo)
        {
            PortIn = porti;
            PortOut = porto;
            ModNumber = mod;
            FirstSlotID = slo;
            IsUsed = true;
        }
    }
}
