namespace WinRMSharp.Tests
{
    public class Config
    {
        public const string OPEN_SHELL_REQUEST =
$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/09/transfer/Create</a:Action>
		<w:OptionSet>
			<w:Option Name="WINRS_NOPROFILE">False</w:Option>
			<w:Option Name="WINRS_CODEPAGE">437</w:Option>
		</w:OptionSet>
	</env:Header>
	<env:Body>
		<rsp:Shell>
			<rsp:InputStreams>stdin</rsp:InputStreams>
			<rsp:OutputStreams>stdout stderr</rsp:OutputStreams>
		</rsp:Shell>
	</env:Body>
</env:Envelope>
""";

        public const string OPEN_SHELL_RESPONSE =
$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer">
	<s:Header>
		<a:Action>http://schemas.xmlsoap.org/ws/2004/09/transfer/CreateResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<x:ResourceCreated>
			<a:Address>http://windows-host:5985/wsman</a:Address>
			<a:ReferenceParameters>
				<w:ResourceURI>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
				<w:SelectorSet>
					<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
				</w:SelectorSet>
			</a:ReferenceParameters>
		</x:ResourceCreated>
	</s:Body>
</s:Envelope>
""";

        public const string CLOSE_SHELL_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body />
</env:Envelope>
""";

        public const string CLOSE_SHELL_RESPONSE =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.xmlsoap.org/ws/2004/09/transfer/DeleteResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body/>
</s:Envelope>
""";

        public const string RUN_CMD_WITH_ARGS_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action>
		<w:OptionSet>
		  <w:Option Name="WINRS_CONSOLEMODE_STDIN">TRUE</w:Option>
		  <w:Option Name="WINRS_SKIP_CMD_SHELL">FALSE</w:Option>
		</w:OptionSet>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:CommandLine>
			<rsp:Command>ipconfig</rsp:Command>
			<rsp:Arguments>/all</rsp:Arguments>
		</rsp:CommandLine>
	</env:Body>
</env:Envelope>
""";

        public const string RUN_CMD_WO_ARGS_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action>
		<w:OptionSet>
		  <w:Option Name="WINRS_CONSOLEMODE_STDIN">TRUE</w:Option>
		  <w:Option Name="WINRS_SKIP_CMD_SHELL">FALSE</w:Option>
		</w:OptionSet>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:CommandLine>
			<rsp:Command>hostname</rsp:Command>
		</rsp:CommandLine>
	</env:Body>
</env:Envelope>
""";

        public const string RUN_CMD_PS_RESPONSE =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:CommandResponse>
			<rsp:CommandId>11111111-1111-1111-1111-1111111111{0}4</rsp:CommandId>
		</rsp:CommandResponse>
	</s:Body>
</s:Envelope>
""";

        public const string RUN_PS_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-8"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
		<w:OptionSet>
			<w:Option Name="WINRS_CONSOLEMODE_STDIN">TRUE</w:Option>
			<w:Option Name="WINRS_SKIP_CMD_SHELL">FALSE</w:Option>
		</w:OptionSet>
	</env:Header>
	<env:Body>
		<rsp:CommandLine>
			<rsp:Command>powershell -encodedcommand VwByAGkAdABlAC0ARQByAHIAbwByACAAIgBFAHIAcgBvAHIAIgA=</rsp:Command>
		</rsp:CommandLine>
	</env:Body>
</env:Envelope>
""";

        public const string CLEANUP_CMD_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Signal CommandId="11111111-1111-1111-1111-1111111111{0}4">
			<rsp:Code>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate</rsp:Code>
		</rsp:Signal>
	</env:Body>
</env:Envelope>
""";

        public const string CLEANUP_CMD_RESPONSE =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/SignalResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:SignalResponse/>
	</s:Body>
</s:Envelope>
""";

        public const string GET_CMD_PS_OUTPUT_REQUEST =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Receive>
			<rsp:DesiredStream CommandId="11111111-1111-1111-1111-1111111111{0}4">stdout stderr</rsp:DesiredStream>
		</rsp:Receive>
	</env:Body>
</env:Envelope>
""";

        public const string GET_CMD_OUTPUT_RESPONSE =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd">
    <s:Header>
        <a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/ReceiveResponse</a:Action>
        <a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
        <a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
        <a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
    </s:Header>
    <s:Body>
        <rsp:ReceiveResponse>
            <rsp:Stream CommandId="11111111-1111-1111-1111-111111111114" Name="stdout">DQpXaW5kb3dzIElQIENvbmZpZ3VyYXRpb24NCg0K</rsp:Stream>
            <rsp:Stream CommandId="11111111-1111-1111-1111-111111111114" Name="stdout">ICAgSG9zdCBOYW1lIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiAuIDogV0lORE9XUy1IT1NUCiAgIFByaW1hcnkgRG5zIFN1ZmZpeCAgLiAuIC4gLiAuIC4gLiA6IAogICBOb2RlIFR5cGUgLiAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBIeWJyaWQKICAgSVAgUm91dGluZyBFbmFibGVkLiAuIC4gLiAuIC4gLiAuIDogTm8KICAgV0lOUyBQcm94eSBFbmFibGVkLiAuIC4gLiAuIC4gLiAuIDogTm8KCkV0aGVybmV0IGFkYXB0ZXIgTG9jYWwgQXJlYSBDb25uZWN0aW9uOgoKICAgQ29ubmVjdGlvbi1zcGVjaWZpYyBETlMgU3VmZml4ICAuIDogCiAgIERlc2NyaXB0aW9uIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IEludGVsKFIpIDgyNTY3Vi0yIEdpZ2FiaXQgTmV0d29yayBDb25uZWN0aW9uCiAgIFBoeXNpY2FsIEFkZHJlc3MuIC4gLiAuIC4gLiAuIC4gLiA6IEY4LTBGLTQxLTE2LTg4LUU4CiAgIERIQ1AgRW5hYmxlZC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IE5vCiAgIEF1dG9jb25maWd1cmF0aW9uIEVuYWJsZWQgLiAuIC4gLiA6IFllcwogICBMaW5rLWxvY2FsIElQdjYgQWRkcmVzcyAuIC4gLiAuIC4gOiBmZTgwOjphOTkwOjM1ZTM6YTZhYjpmYzE1JTEwKFByZWZlcnJlZCkgCiAgIElQdjQgQWRkcmVzcy4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IDE3My4xODUuMTUzLjkzKFByZWZlcnJlZCkgCiAgIFN1Ym5ldCBNYXNrIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IDI1NS4yNTUuMjU1LjI0OAogICBEZWZhdWx0IEdhdGV3YXkgLiAuIC4gLiAuIC4gLiAuIC4gOiAxNzMuMTg1LjE1My44OQogICBESENQdjYgSUFJRCAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiAyNTExMzc4NTcKICAgREhDUHY2IENsaWVudCBEVUlELiAuIC4gLiAuIC4gLiAuIDogMDAtMDEtMDAtMDEtMTYtM0ItM0YtQzItRjgtMEYtNDEtMTYtODgtRTgKICAgRE5TIFNlcnZlcnMgLiAuIC4gLiAuIC4gLiAuIC4gLiAuIDogMjA3LjkxLjUuMzIKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgMjA4LjY3LjIyMi4yMjIKICAgTmV0QklPUyBvdmVyIFRjcGlwLiAuIC4gLiAuIC4gLiAuIDogRW5hYmxlZAoKRXRoZXJuZXQgYWRhcHRlciBMb2NhbCBBcmVhIENvbm5lY3Rpb24qIDk6CgogICBNZWRpYSBTdGF0ZSAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBNZWRpYSBkaXNjb25uZWN0ZWQKICAgQ29ubmVjdGlvbi1zcGVjaWZpYyBETlMgU3VmZml4ICAuIDogCiAgIERlc2NyaXB0aW9uIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IEp1bmlwZXIgTmV0d29yayBDb25uZWN0IFZpcnR1YWwgQWRhcHRlcgogICBQaHlzaWNhbCBBZGRyZXNzLiAuIC4gLiAuIC4gLiAuIC4gOiAwMC1GRi1BMC04My00OC0wNAogICBESENQIEVuYWJsZWQuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBZZXMKICAgQXV0b2NvbmZpZ3VyYXRpb24gRW5hYmxlZCAuIC4gLiAuIDogWWVzCgpUdW5uZWwgYWRhcHRlciBpc2F0YXAue0FBNDI2QjM3LTM2OTUtNEVCOC05OTBGLTRDRkFDODQ1RkQxN306CgogICBNZWRpYSBTdGF0ZSAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBNZWRpYSBkaXNjb25uZWN0ZWQKICAgQ29ubmVjdGlvbi1zcGVjaWZpYyBETlMgU3VmZml4ICAuIDogCiAgIERlc2NyaXB0aW9uIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IE1pY3Jvc29mdCBJU0FUQVAgQWRhcHRlcgogICBQaHlzaWNhbCBBZGRyZXNzLiAuIC4gLiAuIC4gLiAuIC4gOiAwMC0wMC0wMC0wMC0wMC0wMC0wMC1FMAogICBESENQIEVuYWJsZWQuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBObwogICBBdXRvY29uZmlndXJhdGlvbiBFbmFibGVkIC4gLiAuIC4gOiBZZXMKClR1bm5lbCBhZGFwdGVyIFRlcmVkbyBUdW5uZWxpbmcgUHNldWRvLUludGVyZmFjZToKCiAgIENvbm5lY3Rpb24tc3BlY2lmaWMgRE5TIFN1ZmZpeCAgLiA6IAogICBEZXNjcmlwdGlvbiAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBUZXJlZG8gVHVubmVsaW5nIFBzZXVkby1JbnRlcmZhY2UKICAgUGh5c2ljYWwgQWRkcmVzcy4gLiAuIC4gLiAuIC4gLiAuIDogMDAtMDAtMDAtMDAtMDAtMDAtMDAtRTAKICAgREhDUCBFbmFibGVkLiAuIC4gLiAuIC4gLiAuIC4gLiAuIDogTm8KICAgQXV0b2NvbmZpZ3VyYXRpb24gRW5hYmxlZCAuIC4gLiAuIDogWWVzCiAgIElQdjYgQWRkcmVzcy4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IDIwMDE6MDo5ZDM4Ojk1M2M6MmNlZjo3ZmM6NTI0Njo2NmEyKFByZWZlcnJlZCkgCiAgIExpbmstbG9jYWwgSVB2NiBBZGRyZXNzIC4gLiAuIC4gLiA6IGZlODA6OjJjZWY6N2ZjOjUyNDY6NjZhMiUxMyhQcmVmZXJyZWQpIAogICBEZWZhdWx0IEdhdGV3YXkgLiAuIC4gLiAuIC4gLiAuIC4gOiAKICAgTmV0QklPUyBvdmVyIFRjcGlwLiAuIC4gLiAuIC4gLiAuIDogRGlzYWJsZWQKClR1bm5lbCBhZGFwdGVyIDZUTzQgQWRhcHRlcjoKCiAgIENvbm5lY3Rpb24tc3BlY2lmaWMgRE5TIFN1ZmZpeCAgLiA6IAogICBEZXNjcmlwdGlvbiAuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiBNaWNyb3NvZnQgNnRvNCBBZGFwdGVyICMyCiAgIFBoeXNpY2FsIEFkZHJlc3MuIC4gLiAuIC4gLiAuIC4gLiA6IDAwLTAwLTAwLTAwLTAwLTAwLTAwLUUwCiAgIERIQ1AgRW5hYmxlZC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IE5vCiAgIEF1dG9jb25maWd1cmF0aW9uIEVuYWJsZWQgLiAuIC4gLiA6IFllcwogICBJUHY2IEFkZHJlc3MuIC4gLiAuIC4gLiAuIC4gLiAuIC4gOiAyMDAyOmFkYjk6OTk1ZDo6YWRiOTo5OTVkKFByZWZlcnJlZCkgCiAgIERlZmF1bHQgR2F0ZXdheSAuIC4gLiAuIC4gLiAuIC4gLiA6IDIwMDI6YzA1ODo2MzAxOjpjMDU4OjYzMDEKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgMjAwMjpjMDU4OjYzMDE6OjEKICAgRE5TIFNlcnZlcnMgLiAuIC4gLiAuIC4gLiAuIC4gLiAuIDogMjA3LjkxLjUuMzIKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgMjA4LjY3LjIyMi4yMjIKICAgTmV0QklPUyBvdmVyIFRjcGlwLiAuIC4gLiAuIC4gLiAuIDogRGlzYWJsZWQKClR1bm5lbCBhZGFwdGVyIGlzYXRhcC57QkExNjBGQzUtNzAyOC00QjFGLUEwNEItMUFDODAyQjBGRjVBfToKCiAgIE1lZGlhIFN0YXRlIC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IE1lZGlhIGRpc2Nvbm5lY3RlZAogICBDb25uZWN0aW9uLXNwZWNpZmljIEROUyBTdWZmaXggIC4gOiAKICAgRGVzY3JpcHRpb24gLiAuIC4gLiAuIC4gLiAuIC4gLiAuIDogTWljcm9zb2Z0IElTQVRBUCBBZGFwdGVyICMyCiAgIFBoeXNpY2FsIEFkZHJlc3MuIC4gLiAuIC4gLiAuIC4gLiA6IDAwLTAwLTAwLTAwLTAwLTAwLTAwLUUwCiAgIERIQ1AgRW5hYmxlZC4gLiAuIC4gLiAuIC4gLiAuIC4gLiA6IE5vCiAgIEF1dG9jb25maWd1cmF0aW9uIEVuYWJsZWQgLiAuIC4gLiA6IFllcwo=</rsp:Stream>
            <rsp:Stream CommandId="11111111-1111-1111-1111-111111111114" End="true" Name="stdout"/>
            <rsp:Stream CommandId="11111111-1111-1111-1111-111111111114" End="true" Name="stderr"/>
            <rsp:CommandState CommandId="11111111-1111-1111-1111-111111111114" State="http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandState/Done">
                <rsp:ExitCode>0</rsp:ExitCode>
            </rsp:CommandState>
        </rsp:ReceiveResponse>
    </s:Body>
</s:Envelope>
""";

        public const string GET_PS_OUTPUT_RESPONSE =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<s:Envelope xml:lang="en-US"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/ReceiveResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:ReceiveResponse>
			<rsp:Stream CommandId="11111111-1111-1111-1111-111111111124" Name="stderr">IzwgQ0xJWE1MDQo=</rsp:Stream>
			<rsp:Stream Name="stderr" CommandId="11111111-1111-1111-1111-111111111124">PE9ianMgVmVyc2lvbj0iMS4xLjAuMSIgeG1sbnM9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vcG93ZXJzaGVsbC8yMDA0LzA0Ij48UyBTPSJFcnJvciI+V3JpdGUtRXJyb3IgIkVycm9yIiA6IEVycm9yX3gwMDBEX194MDAwQV88L1M+PFMgUz0iRXJyb3IiPiAgICArIENhdGVnb3J5SW5mbyAgICAgICAgICA6IE5vdFNwZWNpZmllZDogKDopIFtXcml0ZS1FcnJvcl0sIFdyaXRlRXJyb3JFeGNlcCBfeDAwMERfX3gwMDBBXzwvUz48UyBTPSJFcnJvciI+ICAgdGlvbl94MDAwRF9feDAwMEFfPC9TPjxTIFM9IkVycm9yIj4gICAgKyBGdWxseVF1YWxpZmllZEVycm9ySWQgOiBNaWNyb3NvZnQuUG93ZXJTaGVsbC5Db21tYW5kcy5Xcml0ZUVycm9yRXhjZXB0aW8gX3gwMDBEX194MDAwQV88L1M+PFMgUz0iRXJyb3IiPiAgIG5feDAwMERfX3gwMDBBXzwvUz48UyBTPSJFcnJvciI+IF94MDAwRF9feDAwMEFfPC9TPjwvT2Jqcz4=</rsp:Stream>
			<rsp:Stream CommandId="11111111-1111-1111-1111-111111111124" End="true" Name="stdout"/>
			<rsp:Stream CommandId="11111111-1111-1111-1111-111111111124" End="true" Name="stderr"/>
			<rsp:CommandState CommandId="11111111-1111-1111-1111-111111111124" State="http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandState/Done">
				<rsp:ExitCode>1</rsp:ExitCode>
			</rsp:CommandState>
		</rsp:ReceiveResponse>
	</s:Body>
</s:Envelope>
""";

        public const string RUN_CMD_REQ_INPUT =
$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action>
		<w:OptionSet>
			<w:Option Name="WINRS_CONSOLEMODE_STDIN">TRUE</w:Option>
			<w:Option Name="WINRS_SKIP_CMD_SHELL">FALSE</w:Option>
		</w:OptionSet>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:CommandLine>
			<rsp:Command>cmd</rsp:Command>
		</rsp:CommandLine>
	</env:Body>
</env:Envelope>
""";

        public const string RUN_CMD_REQ_INPUT_RESPONSE =
$$""" 
<?xml version="1.0"?>
<s:Envelope
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd" xml:lang="en-US">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandResponse</a:Action>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111114</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111112</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:CommandResponse>
			<rsp:CommandId>11111111-1111-1111-1111-111111111111</rsp:CommandId>
		</rsp:CommandResponse>
	</s:Body>
</s:Envelope>
""";

        public const string RUN_CMD_SEND_INPUT =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope 
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Send</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Send>
			<rsp:Stream CommandId="11111111-1111-1111-1111-111111111111" Name="stdin" End="false" >ZWNobyAiaGVsbG8gd29ybGQiICYmIGV4aXRcclxu</rsp:Stream>
		</rsp:Send>
	</env:Body>
</env:Envelope>
""";

        public const string RUN_CMD_SEND_INPUT_RESPONSE =
$$"""
<?xml version="1.0" encoding="UTF-8"?>
<s:Envelope 
	xmlns:s="http://www.w3.org/2003/05/soap-envelope" 
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing" 
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd" 
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell" 
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd" 
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer" 
	xml:lang="en-US">
   <s:Header>
      <a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/SendResponse</a:Action>
      <a:MessageID>uuid:72371E37-E073-474B-B4BA-6559D8D94632</a:MessageID>
      <a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
      <a:RelatesTo>uuid:9c3de121-c3a4-452b-8f82-36b84e25b7fe</a:RelatesTo>
   </s:Header>
   <s:Body>
      <rsp:SendResponse />
   </s:Body>
</s:Envelope>
""";

        public const string RUN_CMD_SEND_INPUT_GET_OUTPUT =
$$"""
<?xml version="1.0" encoding="utf-16"?>
<env:Envelope 
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
   <env:Header>
      <a:To>http://windows-host:5985/wsman</a:To>
      <a:ReplyTo>
         <a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
      </a:ReplyTo>
      <w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
      <a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
      <w:Locale a:mustUnderstand="false" xml:lang="en-US" />
      <p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
      <w:OperationTimeout>PT20S</w:OperationTimeout>
      <w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
      <a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action>
      <w:SelectorSet>
         <w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
      </w:SelectorSet>
   </env:Header>
   <env:Body>
      <rsp:Receive>
         <rsp:DesiredStream CommandId="11111111-1111-1111-1111-111111111111">stdout stderr</rsp:DesiredStream>
      </rsp:Receive>
   </env:Body>
</env:Envelope>
""";

        public const string RUN_CMD_SEND_INPUT_GET_OUTPUT_RESPONSE =
$$"""
<?xml version="1.0" encoding="UTF-8"?>
<s:Envelope 
	xmlns:s="http://www.w3.org/2003/05/soap-envelope" 
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing" 
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd" 
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell" 
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd" 
	xml:lang="en-US">
   <s:Header>
      <a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/ReceiveResponse</a:Action>
      <a:MessageID>uuid:6468086A-377E-4BE3-AC71-1155F0F1D4E1</a:MessageID>
      <a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
      <a:RelatesTo>uuid:02f258b6-186f-4ac0-adc3-51550a131e64</a:RelatesTo>
   </s:Header>
   <s:Body>
      <rsp:ReceiveResponse>
         <rsp:Stream Name="stdout" CommandId="11111111-1111-1111-1111-111111111111">TWljcm9zb2Z0IFdpbmRvd3MgW1ZlcnNpb24gMTAuMC4xNzc2My4xMDdd</rsp:Stream>
         <rsp:Stream Name="stdout" CommandId="11111111-1111-1111-1111-111111111111">DQooYykgMjAxOCBNaWNyb3NvZnQgQ29ycG9yYXRpb24uIEFsbCByaWdodHMgcmVzZXJ2ZWQuDQoNCkM6XFVzZXJzXHJ3ZWJlcj5lY2hvIGhlbGxvIHdvcmxkICYmIGV4aXQNCmhlbGxvIHdvcmxkIA0K</rsp:Stream>
         <rsp:Stream Name="stdout" CommandId="11111111-1111-1111-1111-111111111111" End="true" />
         <rsp:Stream Name="stderr" CommandId="11111111-1111-1111-1111-111111111111" End="true" />
         <rsp:CommandState CommandId="11111111-1111-1111-1111-111111111111" State="http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandState/Done">
            <rsp:ExitCode>0</rsp:ExitCode>
         </rsp:CommandState>
      </rsp:ReceiveResponse>
   </s:Body>
</s:Envelope>
""";

        public const string STDIN_CMD_CLEANUP =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<env:Envelope
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
	xmlns:a=""http://schemas.xmlsoap.org/ws/2004/08/addressing""
	xmlns:b=""http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd""
	xmlns:n=""http://schemas.xmlsoap.org/ws/2004/09/enumeration""
	xmlns:x=""http://schemas.xmlsoap.org/ws/2004/09/transfer""
	xmlns:w=""http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd""
	xmlns:p=""http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd""
	xmlns:rsp=""http://schemas.microsoft.com/wbem/wsman/1/windows/shell""
	xmlns:cfg=""http://schemas.microsoft.com/wbem/wsman/1/config""
	xmlns:env=""http://www.w3.org/2003/05/soap-envelope"">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand=""true"">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand=""true"">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand=""false"" xml:lang=""en-US""/>
		<p:DataLocale a:mustUnderstand=""false"" xml:lang=""en-US""/>
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand=""true"">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand=""true"">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal</a:Action>
		<w:SelectorSet>
			<w:Selector Name=""ShellId"">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Signal CommandId=""11111111-1111-1111-1111-111111111111"">
			<rsp:Code>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate</rsp:Code>
		</rsp:Signal>
	</env:Body>
</env:Envelope>";

        public const string STDIN_CMD_CLEANUP_RESPONSE =
@"<?xml version=""1.0""?>
<s:Envelope
	xmlns:s=""http://www.w3.org/2003/05/soap-envelope""
	xmlns:a=""http://schemas.xmlsoap.org/ws/2004/08/addressing""
	xmlns:x=""http://schemas.xmlsoap.org/ws/2004/09/transfer""
	xmlns:w=""http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd""
	xmlns:rsp=""http://schemas.microsoft.com/wbem/wsman/1/windows/shell""
	xmlns:p=""http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"" xml:lang=""en-US"">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/SignalResponse</a:Action>
		<a:MessageID>uuid:8A875405-3494-4400-A988-B47A563922E7</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:SignalResponse/>
	</s:Body>
</s:Envelope>";

        public const string OPERATION_TIMEOUT_REQUEST =
$$"""
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action>
		<w:OptionSet>
			<w:Option Name="WINRS_CONSOLEMODE_STDIN">TRUE</w:Option>
			<w:Option Name="WINRS_SKIP_CMD_SHELL">FALSE</w:Option>
		</w:OptionSet>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:CommandLine>
			<rsp:Command>powershell -Command Start-Sleep -s 40</rsp:Command>
		</rsp:CommandLine>
	</env:Body>
</env:Envelope>
""";

        public const string OPERATION_TIMEOUT_RESPONSE =
$$"""
<s:Envelope xml:lang="en-US"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandResponse</a:Action>
		<a:MessageID>uuid:DFC09822-5AF3-4E7C-AEF7-B3699B019595</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:CommandResponse>
			<rsp:CommandId>11111111-1111-1111-1111-111111111134</rsp:CommandId>
		</rsp:CommandResponse>
	</s:Body>
</s:Envelope>
""";

        public const string OPERATION_TIMEOUT_GET_0_REQUEST =
$$"""
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Receive>
			<rsp:DesiredStream CommandId="11111111-1111-1111-1111-111111111134">stdout stderr</rsp:DesiredStream>
		</rsp:Receive>
	</env:Body>
</env:Envelope>
""";

        public const string OPERATION_TIMEOUT_GET_0_RESPONSE =
$$"""
<s:Envelope xml:lang="en-US"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:e="http://schemas.xmlsoap.org/ws/2004/08/eventing"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.dmtf.org/wbem/wsman/1/wsman/fault</a:Action>
		<a:MessageID>uuid:D98EC41B-42DA-4147-818E-698CC364A820</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<s:Fault>
			<s:Code>
				<s:Value>s:Receiver</s:Value>
				<s:Subcode>
					<s:Value>w:TimedOut</s:Value>
				</s:Subcode>
			</s:Code>
			<s:Reason>
				<s:Text xml:lang="en-US">The WS-Management service cannot complete the operation within the time specified in OperationTimeout.  </s:Text>
			</s:Reason>
			<s:Detail>
				<f:WSManFault
					xmlns:f="http://schemas.microsoft.com/wbem/wsman/1/wsmanfault" Code="2150858793" Machine="windows-host">
					<f:Message>The WS-Management service cannot complete the operation within the time specified in OperationTimeout.  </f:Message>
				</f:WSManFault>
			</s:Detail>
		</s:Fault>
	</s:Body>
</s:Envelope>
""";

        public const string OPERATION_TIMEOUT_GET_1_REQUEST =
$$"""
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111112</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Receive>
			<rsp:DesiredStream CommandId="11111111-1111-1111-1111-111111111134">stdout stderr</rsp:DesiredStream>
		</rsp:Receive>
	</env:Body>
</env:Envelope>
""";

        public const string OPERATION_TIMEOUT_GET_1_RESPONSE =
$$"""
<s:Envelope xml:lang="en-US"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/ReceiveResponse</a:Action>
		<a:MessageID>uuid:B7258081-489D-4081-B08B-54E61776844F</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111112</a:RelatesTo>
	</s:Header>
	<s:Body>
		<rsp:ReceiveResponse>
			<rsp:Stream Name="stdout" CommandId="11111111-1111-1111-1111-111111111134" End="true"></rsp:Stream>
			<rsp:Stream Name="stderr" CommandId="11111111-1111-1111-1111-111111111134" End="true"></rsp:Stream>
			<rsp:CommandState CommandId="11111111-1111-1111-1111-111111111134" State="http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandState/Done">
				<rsp:ExitCode>0</rsp:ExitCode>
			</rsp:CommandState>
		</rsp:ReceiveResponse>
	</s:Body>
</s:Envelope>
""";

        public const string OPERATION_TIMEOUT =
$$"""
<s:Envelope 
	xml:lang="en-US"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:e="http://schemas.xmlsoap.org/ws/2004/08/eventing"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.dmtf.org/wbem/wsman/1/wsman/fault</a:Action>
		<a:MessageID>uuid:D6A43602-1E80-4371-A7D5-49619CB839EE</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:607e3715-d540-41ad-9cb8-5341436e6bbf</a:RelatesTo>
	</s:Header>
	<s:Body>
		<s:Fault>
			<s:Code>
				<s:Value>s:Receiver</s:Value>
				<s:Subcode>
					<s:Value>w:TimedOut</s:Value>
				</s:Subcode>
			</s:Code>
			<s:Reason>
				<s:Text xml:lang="en-US">The WS-Management service cannot complete the operation within the time specified in OperationTimeout.  </s:Text>
			</s:Reason>
			<s:Detail>
				<f:WSManFault
					xmlns:f="http://schemas.microsoft.com/wbem/wsman/1/wsmanfault" Code="2150858793" Machine="windows-host">
					<f:Message>The WS-Management service cannot complete the operation within the time specified in OperationTimeout.  </f:Message>
				</f:WSManFault>
			</s:Detail>
		</s:Fault>
	</s:Body>
</s:Envelope>
""";

        public const string CLOSE_COMMAND_FAULT_REQUEST =
$$"""
<env:Envelope
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:b="http://schemas.dmtf.org/wbem/wsman/1/cimbinding.xsd"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd"
	xmlns:rsp="http://schemas.microsoft.com/wbem/wsman/1/windows/shell"
	xmlns:cfg="http://schemas.microsoft.com/wbem/wsman/1/config"
	xmlns:env="http://www.w3.org/2003/05/soap-envelope">
	<env:Header>
		<a:To>http://windows-host:5985/wsman</a:To>
		<a:ReplyTo>
			<a:Address mustUnderstand="true">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>
		</a:ReplyTo>
		<w:MaxEnvelopeSize a:mustUnderstand="true">153600</w:MaxEnvelopeSize>
		<a:MessageID>uuid:11111111-1111-1111-1111-111111111111</a:MessageID>
		<w:Locale a:mustUnderstand="false" xml:lang="en-US" />
		<p:DataLocale a:mustUnderstand="false" xml:lang="en-US" />
		<w:OperationTimeout>PT20S</w:OperationTimeout>
		<w:ResourceURI a:mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd</w:ResourceURI>
		<a:Action mustUnderstand="true">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal</a:Action>
		<w:SelectorSet>
			<w:Selector Name="ShellId">11111111-1111-1111-1111-111111111113</w:Selector>
		</w:SelectorSet>
	</env:Header>
	<env:Body>
		<rsp:Signal CommandId="11111111-1111-1111-1111-111111111117">
			<rsp:Code>http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate</rsp:Code>
		</rsp:Signal>
	</env:Body>
</env:Envelope>
""";

        public const string CLOSE_COMMAND_FAULT_RESPONSE =
$$"""
<s:Envelope xml:lang="en-US"
	xmlns:s="http://www.w3.org/2003/05/soap-envelope"
	xmlns:a="http://schemas.xmlsoap.org/ws/2004/08/addressing"
	xmlns:x="http://schemas.xmlsoap.org/ws/2004/09/transfer"
	xmlns:e="http://schemas.xmlsoap.org/ws/2004/08/eventing"
	xmlns:n="http://schemas.xmlsoap.org/ws/2004/09/enumeration"
	xmlns:w="http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd"
	xmlns:p="http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd">
	<s:Header>
		<a:Action>http://schemas.dmtf.org/wbem/wsman/1/wsman/fault</a:Action>
		<a:MessageID>uuid:BDE06D49-FC33-4410-AE1D-E09641B6A333</a:MessageID>
		<a:To>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:To>
		<a:RelatesTo>uuid:11111111-1111-1111-1111-111111111111</a:RelatesTo>
	</s:Header>
	<s:Body>
		<s:Fault>
			<s:Code>
				<s:Value>s:Sender</s:Value>
				<s:Subcode>
					<s:Value>w:InvalidSelectors</s:Value>
				</s:Subcode>
			</s:Code>
			<s:Reason>
				<s:Text xml:lang="en-US">The WS-Management service cannot process the request because the request contained invalid selectors for the resource. </s:Text>
			</s:Reason>
			<s:Detail>
				<w:FaultDetail>http://schemas.dmtf.org/wbem/wsman/1/wsman/faultDetail/UnexpectedSelectors</w:FaultDetail>
				<f:WSManFault
					xmlns:f="http://schemas.microsoft.com/wbem/wsman/1/wsmanfault" Code="2150858843" Machine="windows-host">
					<f:Message>The request for the Windows Remote Shell with ShellId 11111111-1111-1111-1111-111111111111 failed because the shell was not found on the server. Possible causes are: the specified ShellId is incorrect or the shell no longer exists on the server. Provide the correct ShellId or create a new shell and retry the operation. </f:Message>
				</f:WSManFault>
			</s:Detail>
		</s:Fault>
	</s:Body>
</s:Envelope>
""";
    }
}
