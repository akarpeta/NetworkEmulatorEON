using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node
{
    public class SNPP
    {
        public List<int> SlotsTable { get; set; }

        public List<SNP> Records { get; set; }

        public SNPP()
        {
            Records = new List<SNP>();
            SlotsTable = new List<int>(10);

            for (int i = 0; i < 10; i++)
            {
                SlotsTable.Add(-1);
            }
        }
    }
}
