using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class RouterMapRecord
    {
        public SlotWindow slotWindow = new SlotWindow();
        public int DestinationID;

        public RouterMapRecord()
        {

        }
        public RouterMapRecord(SlotWindow slot, int destID )
        {
            slotWindow = slot;
            DestinationID = destID;
        }
    }
}
