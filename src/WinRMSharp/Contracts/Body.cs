using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace WinRMSharp.Contracts
{
    public class Body
    {
        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public Shell? Shell { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public CommandLine? CommandLine { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public Send? Send { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public Receive? Receive { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public Signal? Signal { get; set; }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/fa0e35c4-e2eb-460e-b915-c6d1ae1aafb8
    /// </summary>
    public class Variable
    {
        [XmlAttribute(AttributeName = "Name")]
        public required string Name { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/cc251546(v=PROT.13).aspx
    /// </summary>
    public class Shell
    {
        [XmlArray(Namespace = Namespace.WSMAN_SHELL)]
        public Variable[]? Environment { get; set; }

        [XmlIgnore]
        public TimeSpan? IdleTimeout { get; set; }

        [Browsable(false)]
        [XmlElement(ElementName = nameof(IdleTimeout), Namespace = Namespace.WSMAN_SHELL)]
        public string? IdleTimeOutString
        {
            get => (IdleTimeout != null) ? XmlConvert.ToString(IdleTimeout.Value) : null;
        }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public string InputStreams { get; set; } = "stdin";

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public string OutputStreams { get; set; } = "stdout stderr";

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public string? WorkingDirectory { get; set; }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/8ec4bd59-8374-46e2-a134-1ca21d37e65d
    /// </summary>
    public class CommandLine
    {
        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public required string Command { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public string? Arguments { get; set; }
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/4c0a7e68-d5c6-4fc9-81b0-1c4654297a53
    /// </summary>
    public class Send
    {
        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public required InputStream Stream { get; set; }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/08b98520-b467-429c-95ce-d665fd23ef40
    /// </summary>
    public class Receive
    {
        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public required DesiredStream DesiredStream { get; set; }
    }

    public class InputStream
    {
        [XmlAttribute]
        public string? CommandId { get; set; }

        [XmlAttribute]
        public required string Name { get; set; }

        [XmlAttribute]
        public bool End { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    public class DesiredStream
    {
        [XmlAttribute]
        public required string CommandId { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/4d6a53ab-3db9-4219-b06d-2643e9dda9a0
    /// </summary>
    public class Signal
    {
        [XmlAttribute]
        public string? CommandId { get; set; }

        [XmlElement(Namespace = Namespace.WSMAN_SHELL)]
        public required Code Code { get; set; }
    }

    public class Code
    {
        [XmlText]
        public required string Value { get; set; }
    }
}
