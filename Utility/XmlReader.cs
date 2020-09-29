using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace Utility
{
    public class XmlReader
    {
        public XmlDocument XmlDoc;

        public XmlReader(string PathFile)
        {
            XmlDoc = new XmlDocument();
            XmlDoc.Load(PathFile);
        }

        public int GetNumberOfItems(string name)
        {
            return XmlDoc.GetElementsByTagName(name).Count;
        }

        public string GetElementValue(int number, string name)
        {
            return XmlDoc.GetElementsByTagName(name).Item(number).InnerText;
        }

        public string GetAttributeValue(int number, string ElementName, string AttributeName)
        {
            XmlNode Node = XmlDoc.GetElementsByTagName(ElementName).Item(number);
            return Node.Attributes.GetNamedItem(AttributeName).InnerText;
        }
        public string GetOptionalAttributeValue(int number, string ElementName, string AttributeName)
        {
            XmlNode Node = XmlDoc.GetElementsByTagName(ElementName).Item(number);
            XmlNode AttrNode = Node.Attributes.GetNamedItem(AttributeName);
            return (null != AttrNode) ? AttrNode.InnerText : null;
        }

        public string GetAttributeValue( string ElementName, string AttributeName)
        {
            XmlNode Node = XmlDoc.GetElementsByTagName(ElementName).Item(0);
            return Node.Attributes.GetNamedItem(AttributeName).InnerText;
        }
        //TO DO:
        public Dictionary<string, string> GetHostItems(string tagname, string item1, string item2)
        {

            Dictionary<string, string> lists = new Dictionary<string, string>();
            XmlNode Node = XmlDoc.GetElementsByTagName(tagname).Item(0);
            XmlNodeList ChildNodes = Node.ChildNodes;
            
            foreach(XmlNode distanthost in ChildNodes)
            {
                string tmp1 = distanthost.Attributes.GetNamedItem(item1).InnerText;
                string tmp2 = distanthost.Attributes.GetNamedItem(item2).InnerText;
                lists.Add(tmp1, tmp2);
            }

            return lists;
        } 

        public IPAddress GetIPAddressCC(int numberDomain)
        {
            XmlNode Node = XmlDoc.GetElementsByTagName("CC").Item(0);
            XmlNodeList ChildNodes = Node.ChildNodes;
            IPAddress address = IPAddress.Parse("0.0.0.0");
            foreach(XmlNode xml in ChildNodes)
            {
                int tmp1 = Int32.Parse(xml.Attributes.GetNamedItem("DOMAIN").InnerText);
                IPAddress tmp2 = IPAddress.Parse(xml.Attributes.GetNamedItem("IP_ADDRESS").InnerText);
                if(tmp1==numberDomain)
                {
                    address = tmp2;
                }
                
            }

            return address;
        }

    }
}
