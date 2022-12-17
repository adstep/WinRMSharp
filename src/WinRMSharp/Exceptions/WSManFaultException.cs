namespace WinRMSharp.Exceptions
{
    /// <summary>
    /// Wraps error information returned as part of a response with a WSManFualt (see https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/2b6ab0b1-4d5c-4c13-9f28-0f04716e5fa4).
    /// </summary>
    internal class WSManFaultException : WinRMException
    {
        public string? Code { get; set; }
        public string? SubCode { get; set; }
        public string? Machine { get; set; }
        public string? Reason { get; set; }
        public string? FaultMessage { get; set; }
        public string? Provider { get; set; }
        public string? ProviderPath { get; set; }
        public string? ProviderFault { get; set; }

        public WSManFaultException(Exception innerException) : base("WSManFaultException", innerException)
        {
        }
    }
}
