using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Utility
{
    public class MySocket : Socket
    {
        public MySocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
            ReceiveTimeout = 999999999;
        }

        public MyPacket Receive()
        {
            byte[] receiveBuffer = new byte[256];
            int receivedBytes = Receive(receiveBuffer);

            if (Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes).Substring(0, 1).Equals("C"))
            {
                return null;
            }

            return MyPacket.BytesToPacket(receiveBuffer);
        }

        public int Send(MyPacket packet)
        {
            return Send(packet.PacketToBytes());
        }

    }
}
