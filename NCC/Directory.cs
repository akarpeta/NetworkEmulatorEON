using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NCC
{
    public class Directory
    {
        Dictionary<string, int> DirectoryHostDomain = new Dictionary<string, int>();
        Dictionary<string, IPAddress> DirectoryHostNametoIP = new Dictionary<string, IPAddress>();
        Dictionary<IPAddress, string> DirectoryHostIPtoName = new Dictionary<IPAddress, string>();
        
        public Directory()
        {

        }

        //translacja adresów, zwraca zbior punktow SNPP
        public bool Directory_request(string source, string destination)
        {
                if(DirectoryHostDomain[source]==DirectoryHostDomain[destination])
                {
                //sa w tej same domenie CallIndication
                    return true;
                }else
                {
                // w roznych domenach CallCoordination
                    return false;
                }
        }

        public bool Directory_request(IPAddress source, string destination)
        {
            string sourcename = DirectoryHostIPtoName[source];

            if (DirectoryHostDomain[sourcename] == DirectoryHostDomain[destination])
            {
                //sa w tej same domenie CallIndication
                return true;

            }
            else
            {
                // w roznych domenach CallCoordination
                return false;
            }
        }

        public bool Directory_request(IPAddress source, IPAddress destination)
        {
            string sourcename = DirectoryHostIPtoName[source];
            string destname = DirectoryHostIPtoName[destination];

            if (DirectoryHostDomain[sourcename] == DirectoryHostDomain[destname])
            {
                //sa w tej same domenie CallIndication
                return true;

            }
            else
            {
                // w roznych domenach CallCoordination
                return false;
            }
        }

        public int ReturnDomainDestinationHost(string destination)
        {
            return DirectoryHostDomain[destination]; 
        }



        //translacja adresu IP
        public IPAddress Translation_Address(string hostname) 
        {
            return DirectoryHostNametoIP[hostname];
        }

        public void ReadConfig()
        {
            XmlReader reader = new XmlReader("ConfigurationDirectory.xml");
            int snpp_number = reader.GetNumberOfItems("SNPP");
            for(int i=0; i<snpp_number;i++)
            {
                string hostname = reader.GetAttributeValue(i, "SNPP", "HOST_NAME");
                IPAddress iPAddress = IPAddress.Parse(reader.GetAttributeValue(i, "SNPP", "IP_ADDRESS"));
                int domain = Int32.Parse(reader.GetAttributeValue(i, "SNPP", "DOMAIN"));
                DirectoryHostDomain.Add(hostname, domain);
                DirectoryHostNametoIP.Add(hostname, iPAddress);
                DirectoryHostIPtoName.Add(iPAddress, hostname);
            }
        }
    
    }
}
