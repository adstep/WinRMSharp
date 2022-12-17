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

        public static XDocument Parse(this string text)
        {
            return XDocument.Parse(text);
        }

        public static string? Format(this string text)
        {
            try
            {
                XDocument doc = XDocument.Parse(text);
                return doc.ToString();
            }
            catch (Exception)
            {
                return text;
            }
        }
    }
}
