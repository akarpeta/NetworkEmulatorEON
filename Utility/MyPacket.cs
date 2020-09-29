using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class MyPacket
    {

        //dlugosc naglowka w bajtach
        public const int PacketHeaderLength = 40;

        public int ID { get; set; }
        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }

        //pole z częstotliwością czyli id tablicy od ktorego zajmuje szczeliny
        public int Frequency { get; set; }
        //ilosc szczelin
        public int Bandwith { get; set; }

        //Predkosc biotwa (rzeczywista= BitRate*12,5Gb/s)
        public short BitRate { get; set; }

        //Wydajnosc modulacji
        public short Performance { get; set; }

        public int Port { get; set; }

        public string Payload { get; set; }

        //public List<byte> Header { get; set; }

        public int PacketLength { get; set; }

        public SlotWindow slotWindow = new SlotWindow();
       

        //Testowy konstruktor
        public MyPacket()
        {

        }



        public MyPacket(string Payload, int port, string destinationIP, string sourceIP, SlotWindow inSlot)
        {
            this.Payload = Payload;
            this.Port = port;
            this.SourceAddress = IPAddress.Parse(sourceIP);
            this.DestinationAddress = IPAddress.Parse(destinationIP);
            //0 + 193 000 GHz...
            this.Frequency = 122;
            this.ID = 111;
            //12.5GHz
            this.Bandwith = 3;

            //predkosc bitowa, z jaka leci pakiet n*12.5Gb/s
            this.BitRate = 4;

            //Jeden symbol koduje 1 bit
            this.Performance = 5;

            this.slotWindow = inSlot;
        }

        public byte[] PacketToBytes()
        {
            List<byte> packetbytes = new List<byte>();
            PacketLength = PacketHeaderLength + Payload.Length;

            packetbytes.AddRange(BitConverter.GetBytes(PacketLength));
            packetbytes.AddRange(BitConverter.GetBytes(ID));
            packetbytes.AddRange(SourceAddress.GetAddressBytes());
            packetbytes.AddRange(DestinationAddress.GetAddressBytes());
            packetbytes.AddRange(BitConverter.GetBytes(Frequency));
            packetbytes.AddRange(BitConverter.GetBytes(Bandwith));
            packetbytes.AddRange(BitConverter.GetBytes(BitRate));
            packetbytes.AddRange(BitConverter.GetBytes(Performance));
            packetbytes.AddRange(BitConverter.GetBytes(Port));
            packetbytes.AddRange(BitConverter.GetBytes(slotWindow.FirstSlot));
            packetbytes.AddRange(BitConverter.GetBytes(slotWindow.NumberofSlots));
            packetbytes.AddRange(Encoding.ASCII.GetBytes(Payload ?? ""));
            return packetbytes.ToArray();
        }

        public static MyPacket BytesToPacket(byte[] bytes)
        {
            MyPacket packet = new MyPacket
            {
                PacketLength = BitConverter.ToInt32(bytes, 0),
                ID = BitConverter.ToInt32(bytes, 4),
                SourceAddress = new IPAddress(new byte[]
                {bytes[8], bytes[9], bytes[10], bytes[11]}),
                DestinationAddress = new IPAddress(new byte[]
                {bytes[12], bytes[13], bytes[14], bytes[15]}),
                Frequency = BitConverter.ToInt16(bytes, 16),
                Bandwith = BitConverter.ToInt16(bytes, 20),
                BitRate = BitConverter.ToInt16(bytes, 24),
                Performance = BitConverter.ToInt16(bytes, 26),
                Port = BitConverter.ToInt32(bytes, 28)
            };

            packet.slotWindow.FirstSlot = BitConverter.ToInt32(bytes, 32);
            packet.slotWindow.NumberofSlots = BitConverter.ToInt32(bytes, 36);

            var usefulPayload = new List<byte>();
            usefulPayload.AddRange(bytes.ToList()
                .GetRange(40, packet.PacketLength - PacketHeaderLength));
            packet.Payload = Encoding.ASCII.GetString(usefulPayload.ToArray());

            return packet;
        }
    }
}
