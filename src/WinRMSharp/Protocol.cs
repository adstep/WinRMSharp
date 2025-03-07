using System.Xml.Linq;
using System.Xml;
using WinRMSharp.Contracts;
using WinRMSharp.Exceptions;
using WinRMSharp.Utils;
using System.Xml.XPath;
using System.Net;
using System.Text;

namespace WinRMSharp
{
    /// <summary>
    /// Options used to configure a <see cref="Protocol"/> instance.
    /// </summary>
    public class ProtocolOptions
    {
        /// <summary>
        /// Maximum allowed time in seconds for any single wsman HTTP operation
        /// </summary>
        public TimeSpan? OperationTimeout { get; set; }

        /// <summary>
        /// Maximum response size in bytes. 
        /// </summary>
        public int? MaxEnvelopeSize { get; set; }

    }

    public class Protocol : IProtocol
    {
        private const string RESOURCE_URI = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";

        private static readonly Encoding Encoding = Encoding.UTF8;

        private static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromSeconds(20);
        private static readonly int DefaultMaxEnvelopeSize = 153600;
        private static readonly string DefaultLocale = "en-US";

        private readonly IGuidProvider _guidProvider;

        /// <inheritdoc cref="IProtocol.Transport" />
        public ITransport Transport { get; private set; }

        /// <inheritdoc cref="IProtocol.OperationTimeout" />
        public TimeSpan OperationTimeout { get; }

        /// <inheritdoc cref="IProtocol.MaxEnvelopeSize" />
        public int MaxEnvelopeSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="transport">Network transport used for sending/receiving SOAP requests/responses.</param>
        /// <param name="options">Options to configure instance.</param>
        public Protocol(ITransport transport, ProtocolOptions? options = null)
            : this(transport, new GuidProvider(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="transport">Network transport used for sending/receiving SOAP requests/responses.</param>
        /// <param name="guidProvider">Guid generator for stamping outbound request messages.</param>
        /// <param name="options">Options to configure instance.</param>
        internal Protocol(ITransport transport, IGuidProvider guidProvider, ProtocolOptions? options = null)
        {
            _guidProvider = guidProvider;
            Transport = transport;

            OperationTimeout = options?.OperationTimeout ?? DefaultOperationTimeout;
            MaxEnvelopeSize = options?.MaxEnvelopeSize ?? DefaultMaxEnvelopeSize;
        }

        /// <inheritdoc cref="IProtocol.OpenShell" />
        public async Task<string> OpenShell(string inputStream = "stdin", string outputStream = "stdout stderr", string? workingDirectory = null, Dictionary<string, string>? envVars = null, TimeSpan? idleTimeout = null, int? codePage = null, bool? noProfile = null)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Create),
                Body = new Body()
                {
                    Shell = new Shell()
                    {
                        Environment = envVars?.Select(kv => new Variable() { Name = kv.Key, Value = kv.Value }).ToArray(),
                        IdleTimeout = idleTimeout,
                        InputStreams = inputStream,
                        OutputStreams = outputStream,
                        WorkingDirectory = workingDirectory
                    }
                }
            };

            envelope.Header.OptionSet = new List<Option>();

            if (noProfile.HasValue)
            {
                envelope.Header.OptionSet.Add(
                    new Option()
                    {
                        Name = "WINRS_NOPROFILE",
                        Value = noProfile.Value.ToString()
                    }
                );
            }

            if (codePage.HasValue)
            {
                envelope.Header.OptionSet.Add(
                    new Option()
                    {
                        Name = "WINRS_CODEPAGE",
                        Value = codePage.Value.ToString()
                    }
                );
            }

            XDocument root = await Send(envelope).ConfigureAwait(false);

            string? shellId = root.Descendants().FirstOrDefault(e => e.Attribute("Name")?.Value == "ShellId")?.Value;

            if (string.IsNullOrEmpty(shellId))
            {
                throw new WinRMException("Failed to extract shellId");
            }

            return shellId!;
        }

        /// <inheritdoc cref="IProtocol.RunCommand" />
        public async Task<string> RunCommand(string shellId, string command, string[]? args = null)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Command, shellId),
                Body = new Body()
                {
                    CommandLine = new CommandLine()
                    {
                        Command = command,
                        Arguments = (args != null) ? string.Join(" ", args) : null
                    }
                }
            };

            envelope.Header.OptionSet = new List<Option>()
            {
                new Option()
                {
                    Name = "WINRS_CONSOLEMODE_STDIN",
                    Value = true.ToString().ToUpper()
                },
                new Option()
                {
                    Name = "WINRS_SKIP_CMD_SHELL",
                    Value = false.ToString().ToUpper()
                }
            };

            XDocument root = await Send(envelope);

            string? commandId = root.Descendants().FirstOrDefault(e => e?.Name.ToString().EndsWith("CommandId") ?? false)?.Value;

            if (commandId == null)
            {
                throw new WinRMException("Failed to extract commandId");
            }

            return commandId;
        }

        /// <inheritdoc cref="IProtocol.SendCommandInput" />
        public async Task SendCommandInput(string shellId, string commandId, string input, bool end = false)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Send, shellId),
                Body = new Body()
                {
                    Send = new Send()
                    {
                        Stream = new InputStream()
                        {
                            CommandId = commandId,
                            Name = "stdin",
                            End = end,
                            Value = input.EncodeBase64(Encoding)
                        }
                    }
                }
            };

            await Send(envelope);
        }

        /// <inheritdoc cref="IProtocol.PollCommandState" />
        public async Task<CommandState> PollCommandState(string shellId, string commandId)
        {
            StringBuilder stdoutBuilder = new StringBuilder();
            StringBuilder stderrBuilder = new StringBuilder();

            bool done = false;
            int statusCode = 0;

            while (!done)
            {
                try
                {
                    CommandState state = await GetCommandState(shellId, commandId);

                    done = state.Done;
                    statusCode = state.StatusCode;

                    stdoutBuilder.Append(state.Stdout);
                    stderrBuilder.Append(state.Stderr);

                    if (!done)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                }
                catch (WSManFaultException ex)
                    when (ex.Code is Fault.OPERATION_TIMEOUT
                        && (stdoutBuilder.Length == 0)
                        && (stderrBuilder.Length == 0))
                {
                    // Expected exception when waiting for a long-running process with no output
                    // Spec says to continue to issue requests for the state immediately
                }
            }

            return new CommandState()
            {
                Stdout = stdoutBuilder.ToString(),
                Stderr = stderrBuilder.ToString(),
                StatusCode = statusCode,
                Done = true
            };
        }

        /// <inheritdoc cref="IProtocol.GetCommandState" />
        public async Task<CommandState> GetCommandState(string shellId, string commandId)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Receive, shellId),
                Body = new Body()
                {
                    Receive = new Receive()
                    {
                        DesiredStream = new DesiredStream()
                        {
                            CommandId = commandId,
                            Value = "stdout stderr"
                        }
                    }
                }
            };

            XDocument root = await Send(envelope);

            StringBuilder stdout = new StringBuilder();
            StringBuilder stderr = new StringBuilder();

            IEnumerable<XElement> streams = root.Descendants().Where(e => e?.Name.ToString().EndsWith("Stream") ?? false);

            foreach (XElement stream in streams)
            {
                if (string.IsNullOrEmpty(stream.Value))
                    continue;

                string? streamName = stream.Attribute("Name")?.Value;

                if (streamName == "stdout")
                    stdout.Append(stream.Value.DecodeBase64(Encoding));
                else if (streamName == "stderr")
                    stderr.Append(stream.Value.DecodeBase64(Encoding));
            }

            bool done = root.Descendants().FirstOrDefault(e => e.Attribute("State")?.Value.EndsWith("CommandState/Done") ?? false) != null;
            int statusCode = 0;

            if (done)
            {
                _ = int.TryParse(root.Descendants().FirstOrDefault(e => e.Name.ToString().EndsWith("ExitCode"))?.Value, out statusCode);
            }

            return new CommandState()
            {
                Stdout = stdout.ToString(),
                Stderr = stderr.ToString(),
                StatusCode = statusCode,
                Done = done
            };
        }

        /// <inheritdoc cref="IProtocol.CloseShell" />
        public async Task CloseShell(string shellId)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Delete, shellId),
                Body = new Body()
            };

            try
            {
                XDocument root = await Send(envelope);

                string? relatesTo = root.Descendants().FirstOrDefault(e => e?.Name.ToString().EndsWith("RelatesTo") ?? false)?.Value;

                if (relatesTo != envelope.Header.MessageID)
                {
                    throw new WinRMException("Close response id failed to match request");
                }
            }
            catch (WSManFaultException ex) when (ex.Code is Fault.SHELL_NOT_FOUND or Fault.ERROR_OPERATION_ABORTED)
            {
                // Ignore
            }
        }

        /// <inheritdoc cref="IProtocol.CloseCommand" />
        public async Task CloseCommand(string shellId, string commandId)
        {
            Envelope envelope = new Envelope()
            {
                Header = GetHeader(RESOURCE_URI, WSManAction.Signal, shellId),
                Body = new Body()
                {
                    Signal = new Signal()
                    {
                        CommandId = commandId,
                        Code = new Code()
                        {
                            Value = SignalCode.Terminate
                        }
                    }
                }
            };

            try
            {
                XDocument root = await Send(envelope);

                string? relatesTo = root.Descendants().FirstOrDefault(e => e?.Name.ToString().EndsWith("RelatesTo") ?? false)?.Value;

                if (relatesTo != envelope.Header.MessageID)
                {
                    throw new WinRMException("Close response id failed to match request");
                }
            }
            catch (WSManFaultException fault) when (fault.Code == Fault.SHELL_NOT_FOUND || fault.Code == Fault.ERROR_OPERATION_ABORTED)
            {
                // Ignore
                // Dont let the cleanup raise so we dont lose any errors from the command
            }
            catch (TransportException ex) when (ex.Code == (int)HttpStatusCode.InternalServerError)
            {
                // Ignore
                // Dont let the cleanup raise so we dont lose any errors from the command
            }
        }

        private async Task<XDocument> Send(Envelope envelope)
        {
            try
            {
                string response = await Transport.Send(Xml.Serialize(envelope));

                return Xml.Parse(response);
            }
            catch (TransportException ex)
            {
                if (string.IsNullOrEmpty(ex.Content))
                {
                    // Assume some other transport error and raise the original exception
                    throw ex;
                }

                XDocument root;

                try
                {
                    root = Xml.Parse(ex.Content!);
                }
                catch (Exception)
                {
                    // Assume some other transport error and raise the original exception
                    throw ex;
                }

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());

                nsmgr.AddNamespace("soapenv", Namespace.SOAP_ENVELOPE);
                nsmgr.AddNamespace("soapaddr", Namespace.ADDRESSING);
                nsmgr.AddNamespace("wsmanfault", Namespace.WSMAN_FAULT);

                XElement? fault = root.XPathSelectElement("//soapenv:Body/soapenv:Fault", nsmgr);

                string? code = null;
                string? machine = null;
                string? subCode = null;
                string? reason = null;
                string? message = null;
                string? provider = null;
                string? providerPath = null;
                string? providerFault = null;

                if (fault != null)
                {
                    code = fault.XPathSelectElement("//soapenv:Code/soapenv:Value", nsmgr)?.Value;
                    subCode = fault.XPathSelectElement("//soapenv:Code/soapenv:Subcode/soapenv:Value", nsmgr)?.Value;
                    reason = fault.XPathSelectElement("//soapenv:Reason/soapenv:Text", nsmgr)?.Value;
                }

                XElement? wsmanFault = fault?.XPathSelectElement("//soapenv:Detail/wsmanfault:WSManFault", nsmgr);
                if (wsmanFault != null)
                {
                    code = wsmanFault.Attribute("Code")?.Value;
                    machine = wsmanFault.Attribute("Machine")?.Value;
                    message = wsmanFault.XPathSelectElement("//wsmanfault:Message", nsmgr)?.Value;

                    XElement? providerInfo = wsmanFault.XPathSelectElement("//wsmanfault:Message/wsmanfault:ProviderFault", nsmgr);
                    if (providerInfo != null)
                    {
                        provider = providerInfo.Attribute("provider")?.Value;
                        providerPath = providerInfo.Attribute("path")?.Value;
                        providerFault = providerInfo.Value;
                    }
                }

                throw new WSManFaultException(ex)
                {
                    Code = code,
                    SubCode = subCode,
                    Machine = machine,
                    Reason = reason?.Trim(),
                    FaultMessage = message?.Trim(),
                    Provider = provider,
                    ProviderPath = providerPath,
                    ProviderFault = providerFault
                };
            }
        }

        private Header GetHeader(string resourceUri, string action, string? shellId = null)
        {
            string messageId = _guidProvider.NewGuid().ToString();

            Header header = new Header()
            {
                To = "http://windows-host:5985/wsman",
                ReplyTo = new ReplyTo()
                {
                    Address = new Address()
                    {
                        MustUnderstand = true,
                        Value = "http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous"
                    }
                },
                MaxEnvelopeSize = new MaxEnvelopeSize
                {
                    MustUnderstand = true,
                    Value = MaxEnvelopeSize
                },
                MessageID = $"uuid:{messageId}",
                Locale = new Locale()
                {
                    MustUnderstand = false,
                    Language = DefaultLocale
                },
                DataLocale = new Locale()
                {
                    MustUnderstand = false,
                    Language = DefaultLocale
                },
                OperationTimeout = $"PT{OperationTimeout.TotalSeconds}S",
                ResourceURI = new ResourceURI()
                {
                    MustUnderstand = true,
                    Value = resourceUri
                },
                Action = new Contracts.Action()
                {
                    MustUnderstand = true,
                    Text = action
                }
            };

            if (!string.IsNullOrEmpty(shellId))
            {
                header.SelectorSet = new SelectorSet()
                {
                    Selector = new Selector()
                    {
                        Name = "ShellId",
                        Value = shellId!
                    }
                };
            }

            return header;
        }
    }
}
