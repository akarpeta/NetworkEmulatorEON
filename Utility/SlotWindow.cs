using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Utility
{
    public class Connections
    {
        public SlotWindow slotWindow;
        public string Destination;
    }

    public class ConnectionMessage
    {

    }


    public class SlotWindow
    {
        public int FirstSlot;
        public int NumberofSlots;

        public SlotWindow(int first, int number)
        {
            FirstSlot = first;
            NumberofSlots = number;
        }
            
        public SlotWindow()
        {

        }

        public bool Collide(SlotWindow other)
        {
            if(FirstSlot >= other.FirstSlot && FirstSlot < other.FirstSlot + other.NumberofSlots)
            {
                return true;
            }

            if(other.FirstSlot >= FirstSlot && other.FirstSlot < FirstSlot + NumberofSlots)
            {
                return true;
            }

            return false;
        }

    }
}
