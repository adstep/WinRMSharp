namespace WinRMSharp.Exceptions
{
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
