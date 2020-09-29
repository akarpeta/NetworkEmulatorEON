using System;
using System.Net;
using Utility;

namespace Node
{
    public class BehaviourOfPacket
    {
        private TablesInNode RoutingTables { get; set; }

        private NetworkNodeWindow Window { get; set; }

        public BehaviourOfPacket(NetworkNodeWindow window)
        {
            Window = window;
        }

        public MyPacket RoutePacket(MyPacket packet, TablesInNode getTables, NetworkNodeWindow window)
        {
            Window = window;
            RoutingTables = getTables;

            MyPacket routedPacket = null;

            int tableCount = getTables.SlTable.Records.Count;

            int portIn = packet.Port;

            int modType = packet.Performance;

            int startingSlot = packet.Frequency;

            int slotNr = packet.Bandwith;

            int endFor = slotNr + startingSlot;

            for (int i = 0; i < tableCount; i++)
            {
                NewLog($"count {portIn} ::::{getTables.SlTable.Records[i].PortIn}", Window);
                if (portIn == getTables.SlTable.Records[i].PortIn)
                {
                    NewLog($"wszedl  {portIn} ::::{getTables.SlTable.Records[i].PortIn}", Window);
                    //for (int z = startingSlot; z < endFor; z++)
                    //{
                    //    getTables.SlTable.SlotsTable[z] = startingSlot;
                    //}
                    //getTables.SlTable.Records[i].IsUsed = true;
                    //getTables.SlTable.Records[i].ModNumber = modType;
                    //getTables.SlTable.Records[i].FirstSlotID = startingSlot;
                    packet.Port = getTables.SlTable.Records[i].PortOut;
                    NewLog($"wszedl  csoic ::::{getTables.SlTable.Records[i].PortOut}", Window);
                    routedPacket = packet;
                    break;
                }
            }
            NewLog($"countfsdjsj ::::{routedPacket.Port}", Window);
            if (routedPacket == null)
            {
                NewLog("Pakiet odrzucony, brak odpowiedniego rekordu", Window);
                return null;
            }

            return routedPacket;
        }
        public void NewLog(string info, NetworkNodeWindow tmp)
        {

            Window.Dispatcher.Invoke(() => Window.NewLog(info, tmp));

        }
    }

 




    //public class BehaviourOfPacket
    //{
    //    private TablesInNode RoutingTables { get; set; }

    //    private NetworkNodeWindow Window { get; set; }

    //    public BehaviourOfPacket(NetworkNodeWindow window)
    //    {
    //        Window = window;
    //    }

    //    public MyPacket RoutePacket(MyPacket packet, TablesInNode getTables, NetworkNodeWindow window)
    //    {
    //        Window = window;
    //        RoutingTables = getTables;

    //        MyPacket routedPacket = null;

    //        int tableCount = getTables.SlTable.Records.Count;

    //        int portIn = packet.Port;

    //        int band = packet.Bandwith;

    //        for(int i = 0; i < tableCount; i++)
    //        {
    //            if(portIn == getTables.SlTable.Records[i].PortIn && band == getTables.SlTable.Records[i].SlotNumber)
    //            {
    //                packet.Port = (ushort)getTables.SlTable.Records[i].PortOut;
    //                routedPacket = packet;
    //            }
    //        }

    //        if (routedPacket == null)
    //        {
    //            //NewLog("Pakiet odrzucony, brak odpowiedniego rekordu", Window,"LightYellow");
    //            return null;
    //        }

    //        return routedPacket;
    //    }


    //}
}
