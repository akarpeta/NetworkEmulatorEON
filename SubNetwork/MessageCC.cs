using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class MessageCC
    {
        public string AddressCC { get; set; }
        public string AddressStart { get; set; }
        public string AddressEnd { get; set; }
        public string PortStart { get; set; }
        public string PortEnd { get; set; }

        public MessageCC(string adcc, string ads, string ade, string ports, string porte)
        {
            AddressCC = adcc;
            AddressStart = ads;
            AddressEnd = ade;
            PortStart = ports;
            PortEnd = porte;
        }
    }
}
