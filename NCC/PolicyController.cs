using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NCC
{
    public class PolicyController
    {
        private List<string> ListofAccessHost = new List<string>();

        public bool PolicyAccess(string hostname)
        {
            bool access;
            if (ListofAccessHost.Contains(hostname))
            {
                access = true;
            }
            else
            {
                access = false;
            }
            return access;
        }

        public void ReadConfig()
        {
            //kazda domena ma swoj plik konfiguracyjny
            string configurationPath = "ConfigurationPolicy.xml";
            XmlReader reader = new XmlReader(configurationPath);
            int numberofhost = reader.GetNumberOfItems("hostname");
            for (int i = 0; i < numberofhost; i++)
            {
                string hostname = reader.GetAttributeValue(i, "hostname", "NAME");
                ListofAccessHost.Add(hostname);
            }
        }

    }
}
