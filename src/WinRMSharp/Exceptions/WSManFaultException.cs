namespace WinRMSharp.Exceptions
{
    /// <summary>
    /// Wraps error information returned as part of a response with a WSManFualt (see https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wsmv/2b6ab0b1-4d5c-4c13-9f28-0f04716e5fa4).
    /// </summary>
    internal class WSManFaultException : WinRMException
    {
        public string? FaultCode { get; set; }
        public string? FaultSubCode { get; set; }
        public string? FaultDescription { get; set; }

        public WSManFaultException(string? faultCode, string? faultSubCode, string? faultDescription, string message)
            : base(message)
        {
            FaultCode = faultCode;
            FaultSubCode = faultSubCode;
            FaultDescription = faultDescription;
        }
    }
}
