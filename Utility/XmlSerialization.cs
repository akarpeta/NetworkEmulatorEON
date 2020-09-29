using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;


namespace Utility
{
    public class XmlSerialization
    {

        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeserializeObject<T>(string str)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(str))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string GetStringToNormal(string[] msg)
        {
            string resultstring = null;
            foreach (string str in msg)
            {
                resultstring += str + " ";
            }
            return resultstring;
        }

    }
}
