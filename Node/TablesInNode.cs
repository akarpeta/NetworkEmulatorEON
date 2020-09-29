using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utility;

namespace Node
{
    public class TablesInNode
    {
        //private Agent myAgent { get; set; }
        private NetworkNodeWindow Window { get; set; }
        public SNPP SlTable { get; set; }

        public TablesInNode(NetworkNodeWindow myWindow)
        {
            Window = myWindow;
            SlTable = new SNPP();
        }

        public void StartManagementAgent()
        {
            //Task.Run(() => myAgent.StartAgent());
        }

        private void TableEventAction(object sender, TableEvent e)
        {
            switch (e.Action)
            {
                case "SlotRecordAdded":
                    //SlTable.Records.Add(e.SNPP);
                    //NewLog($"Dodana reguła FEC", Window, "LightSlateGrey");
                    break;

                case "SlotRecordRemove":
                   // int indexFec = SlTable.Records.FindIndex(f => (f.PortIn.Equals(e.SRecord.PortIn)) &&
                   // (f.PortOut.Equals(e.SRecord.PortOut)) &&
                   //(f.SNPP.Equals(e.SRecord.SlotNumber)));

                   // SlTable.Records.RemoveAt(indexFec);
                    //NewLog("Usunięta reguła FEC", Window, "LightSlateGrey");
                    break;
            }
        }

        
    }

    public class TableEvent : EventArgs
    {
        public string Action { get; set; }

        public SlotRecord SRecord { get; set; }

        public TableEvent(string action, SlotRecord record)
        {
            Action = action;
            SRecord = record;
        }
    }
}