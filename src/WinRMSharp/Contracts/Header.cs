using System.Xml.Serialization;

namespace WinRMSharp.Contracts
{

    public class Header
    {
        [XmlElement(Namespace = Namespace.ADDRESSING)]
        public required string To { get; set; }

        [XmlElement(Namespace = Namespace.ADDRESSING)]
        public required ReplyTo ReplyTo { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public required MaxEnvelopeSize MaxEnvelopeSize { get; set; }

        [XmlElement(Namespace = Namespace.ADDRESSING)]
        public required string MessageID { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public required Locale Locale { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_1)]
        public required Locale DataLocale { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public required string OperationTimeout { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public required ResourceURI ResourceURI { get; set; }

        [XmlElement(Namespace = Namespace.ADDRESSING)]
        public required Action Action { get; set; }

        [XmlArray(Namespace = Namespace.WSMAN_0)]
        public Option[]? OptionSet { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public SelectorSet? SelectorSet { get; set; }
    }

    public class SelectorSet
    {
        [XmlElement(Namespace = Namespace.WSMAN_0)]
        public required Selector Selector { get; set; }
    }

    public class Selector
    {
        [XmlAttribute]
        public required string Name { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    public class Option
    {
        [XmlAttribute]
        public required string Name { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    public class ReplyTo
    {
        public required Address Address { get; set; }
    }

    public class Address
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = Namespace.ADDRESSING)]
        public bool MustUnderstand { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    public class Action
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = Namespace.ADDRESSING)]
        public bool MustUnderstand { get; set; }

        [XmlText]
        public required string Text { get; set; }
    }

    public class MaxEnvelopeSize
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = Namespace.ADDRESSING)]
        public bool MustUnderstand { get; set; }

        [XmlText]
        public int Value { get; set; }
    }

    public class Locale
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = Namespace.ADDRESSING)]
        public bool MustUnderstand { get; set; }

        [XmlAttribute("xml:lang", DataType = "language")]
        public required string Language { get; set; }
    }

    public class ResourceURI
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = Namespace.ADDRESSING)]
        public bool MustUnderstand { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }
}
