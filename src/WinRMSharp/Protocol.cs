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
    public class Protocol
    {
        private ITransport _transport;

        public Protocol(ITransport transport)
        {
            _transport = transport;
        }

        public async Task<string> OpenShell(string inputStream = "stdin", string outputStream = "stdout stderr", string? workingDirectory = null,  Dictionary<string, string>? envVars = null, TimeSpan? idleTimeout = null)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Create";

            const bool noProfile = false;
            const int codePage = 437;

            Envelope envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action),
                Body = new Body()
                {
                    Shell = new Shell()
                    {
                        Environment = envVars?.Select(kv => new EnvironmentVariable() { Name = kv.Key, Value = kv.Value }).ToArray(),
                        //IdleTimeout = (idleTimeout.HasValue) ? idleTimeout.Value : TimeSpan.FromDays(2),
                        InputStreams = inputStream,
                        OutputStreams = outputStream,
                        WorkingDirectory = workingDirectory
                    }
                }
            };

            envelope.Header.OptionSet = new Option[]
            {
                new Option()
                {
                    Name = "WINRS_NOPROFILE",
                    Value = noProfile.ToString()
                },
                new Option()
                {
                    Name = "WINRS_CODEPAGE",
                    Value = codePage.ToString()
                }
            };

            XDocument root = await Send(envelope).ConfigureAwait(false);

            string? shellId = root.Descendants().FirstOrDefault(e => e.Attribute("Name")?.Value == "ShellId")?.Value;

            if (string.IsNullOrEmpty(shellId))
            {
                throw new WinRMException("Failed to extract shellId");
            }

            return shellId;
        }

        /// <summary>
        /// Run a command on a machine with an open shell. See <see cref="OpenShell(string, string, string)"/>
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine.</param>
        /// <param name="command">The command to run on the remote machine.</param>
        /// <param name="args">An array of arguments for the command.</param>
        /// <returns>The commandId needed to query the output.</returns>
        public async Task<string> RunCommand(string shellId, string command, string[]? args = null)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command";

            Envelope envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action, shellId),
                Body = new Body()
                {
                    CommandLine = new CommandLine()
                    {
                        Command = command,
                        Arguments = (args != null) ? string.Join(" ", args) : null
                    }
                }
            };

            envelope.Header.OptionSet = new Option[]
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

        public async Task SendCommandInput(string shellId, string commandId, string input, bool end = false)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Send";

            Envelope envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action, shellId),
                Body = new Body()
                {
                    Send = new Send()
                    {
                        Stream = new InputStream()
                        {
                            CommandId = commandId,
                            Name = "stdin",
                            End = end,
                            Value = input.EncodeBase64()
                        }
                    }
                }
            };

            XDocument root = await Send(envelope);
        }

        public async Task<CommandState> GetCommandState(string shellId, string commandId)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive";

            Envelope envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action, shellId),
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
                    stdout.Append(stream.Value.DecodeBase64());
                else if (streamName == "stderr")
                    stderr.Append(stream.Value.DecodeBase64());
            }

            bool done = root.Descendants().FirstOrDefault(e => e.Attribute("State")?.Value.EndsWith("CommandState/Done") ?? false) != null;
            int statusCode = 0;

            if (done)
            {
                int.TryParse(root.Descendants().FirstOrDefault(e => e.Name.ToString().EndsWith("ExitCode"))?.Value, out statusCode);
            }

            return new CommandState()
            {
                Stdout = stdout.ToString(),
                Stderr = stderr.ToString(),
                StatusCode = statusCode,
                Done = done
            };
        }

        /// <summary>
        /// Close the shell.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine.</param>
        /// <returns></returns>
        public async Task CloseShell(string shellId)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete";

            var messageId = Guid.NewGuid().ToString();

            var envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action, shellId, messageId),
                Body = new Body()
            };

            try
            {
                var root = await Send(envelope);

                var relatesTo = root.Descendants().FirstOrDefault(e => e?.Name.ToString().EndsWith("RelatesTo") ?? false)?.Value;

                //if (relatesTo?.Replace("uuid:", "") != messageId)
                //{
                //    throw new WinRMException("");
                //}
            }
            catch (WSManFaultException ex) when (ex.FaultCode == Fault.SHELL_NOT_FOUND || ex.FaultSubCode == Fault.ERROR_OPERATION_ABORTED)
            {
                // Ignore
            }
        }

        /// <summary>
        /// Cleans up after a command. See <see cref="RunCommand(string, string, string[], bool, bool)"></see>
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell(string, string, string)"/></param>
        /// <param name="commandId">The command id on the remote machine. See <see cref="RunCommand(string, string, string[], bool, bool)"/></param>
        /// <returns></returns>
        /// <exception cref="WinRMException"></exception>
        public async Task CleanupCommand(string shellId, string commandId)
        {
            const string resourceUri = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/cmd";
            const string action = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal";

            var messageId = Guid.NewGuid().ToString();

            var envelope = new Envelope()
            {
                Header = GetHeader(resourceUri, action, shellId, messageId),
                Body = new Body()
                {
                    Signal = new Signal()
                    {
                        CommandId = commandId,
                        Code = new Code()
                        {
                            Value = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate"
                        }
                    }
                }
            };

            try
            {
                var root = await Send(envelope);

                var relatesTo = root.Descendants().FirstOrDefault(e => e?.Name.ToString().EndsWith("RelatesTo") ?? false)?.Value;

                //if (relatesTo?.Replace("uuid:", "") != messageId)
                //{
                //    throw new WinRMException("");
                //}
            }
            catch (WSManFaultException fault) when (fault.FaultCode == Fault.SHELL_NOT_FOUND || fault.FaultSubCode == Fault.ERROR_OPERATION_ABORTED)
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
                //Console.WriteLine("REQUEST: ");
                //Console.WriteLine(XDocument.Parse(Xml.Serialize(envelope)).ToString());

                var response = await _transport.Send(Xml.Serialize(envelope));

                //Console.WriteLine("RESPONSE: ");
                //Console.WriteLine(XDocument.Parse(response).ToString());

                return Xml.Parse(response);
            }
            catch (TransportException ex)
            {
                XDocument root;

                try
                {
                    root = Xml.Parse(ex.Content);
                }
                catch (Exception)
                {
                    // Assume some other transport error and raise the original exception
                    throw ex;
                }

                var nsmgr = new XmlNamespaceManager(new NameTable());

                nsmgr.AddNamespace("soapenv", Namespace.SOAP_ENVELOPE);
                nsmgr.AddNamespace("soapaddr", Namespace.ADDRESSING);
                nsmgr.AddNamespace("wsmanfault", Namespace.WSMAN_FAULT);

                XElement? fault = root.XPathSelectElement("//soapenv:Body/soapenv:Fault", nsmgr);

                if (fault != null)
                {
                    XElement? wsmanFault = fault.XPathSelectElement("//soapenv:Detail/wsmanfault:WSManFault", nsmgr);
                    string? wsmanFaultCode = wsmanFault?.Attribute("Code")?.Value;

                    string? faultCode = fault.XPathSelectElement("//soapenv:Code/soapenv:Value", nsmgr)?.Value;
                    string? faultSubCode = fault.XPathSelectElement("//soapenv:Code/soapenv:Subcode/soapenv:Value", nsmgr)?.Value;

                    string? errorMessage = fault.XPathSelectElement("//soapenv:Reason/soapenv:Text", nsmgr)?.Value;

                    if (errorMessage == null)
                        errorMessage = "(no error message in fault)";

                    var faultData = new
                    {
                        WsmanFaultCode = wsmanFaultCode,
                        FaultCode = faultCode,
                        FaultSubCode = faultSubCode
                    };

                    throw new WSManFaultException(wsmanFaultCode, faultSubCode, errorMessage, $"{errorMessage} (extended fault data: {faultData}");
                }

                throw new WinRMException($"Failed to extract fault data: {ex.Content}");
            }
        }

        private Header GetHeader(string resourceUri, string action, string? shellId = null, string? messageId = null)
        {
            var operationTimeout = TimeSpan.FromSeconds(20);

            if (messageId == null)
            {
                messageId = Guid.NewGuid().ToString();
            }

            var header = new Header()
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
                    Value = 153600
                },
                MessageID = $"uuid:{messageId}",
                Locale = new Locale()
                {
                    MustUnderstand = false,
                    Language = "en-US"
                },
                DataLocale = new Locale()
                {
                    MustUnderstand = false,
                    Language = "en-US"
                },
                OperationTimeout = $"PT{operationTimeout.TotalSeconds}S",
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
                        Value = shellId
                    }
                };
            }

            return header;
        }
    }
}
