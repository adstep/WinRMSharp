using System.Xml.Serialization;
using System.Xml;

namespace WinRMSharp.Contracts
{
    [XmlRoot(Namespace = Namespace.SOAP_ENVELOPE)]
    public class Envelope
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces(
            new XmlQualifiedName[] {
                new XmlQualifiedName("xsd", Namespace.XML_SCHEMA),
                new XmlQualifiedName("xsi", Namespace.XML_SCHEMA_INSTANCE),
                new XmlQualifiedName("env", Namespace.SOAP_ENVELOPE),

                new XmlQualifiedName("a", Namespace.ADDRESSING),
                new XmlQualifiedName("b", Namespace.CIM_BINDING),
                new XmlQualifiedName("n", Namespace.ENUMERATION),
                new XmlQualifiedName("x", Namespace.TRANSFER),
                new XmlQualifiedName("w", Namespace.WSMAN_0),
                new XmlQualifiedName("p", Namespace.WSMAN_1),
                new XmlQualifiedName("rsp", Namespace.WSMAN_SHELL),
                new XmlQualifiedName("cfg", Namespace.WSMAN_CONFIG)
            }
        );

        [XmlElement(Namespace = Namespace.SOAP_ENVELOPE)]
        public required Header Header { get; set; }

        [XmlElement(Namespace = Namespace.SOAP_ENVELOPE)]
        public required Body Body { get; set; }
    }
}
