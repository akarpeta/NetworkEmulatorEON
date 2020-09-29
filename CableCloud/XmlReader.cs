using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using System.Xml;

//tylko przeklejone
namespace CableCloud
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
    }
}
