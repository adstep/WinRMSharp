using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WinRMSharp.Contracts;

namespace WinRMSharp.Utils
{
    internal static class Xml
    {
        public static string Serialize<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));

            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }

        public static XDocument Load(this Stream stream)
        {
            return XDocument.Load(new XmlTextReader(stream));
        }

        public static XDocument Parse(this string text)
        {
            return XDocument.Parse(text);
        }
    }
}
