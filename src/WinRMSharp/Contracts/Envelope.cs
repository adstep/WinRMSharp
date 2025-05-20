using System.Xml.Serialization;
using System.Xml;

namespace WinRMSharp.Contracts
{
    [XmlRoot(Namespace = Namespace.SOAP_ENVELOPE)]
    public class Envelope
    {
        [XmlElement(Namespace = Namespace.SOAP_ENVELOPE)]
        public required Header Header { get; set; }

        [XmlElement(Namespace = Namespace.SOAP_ENVELOPE)]
        public required Body Body { get; set; }
    }
}
