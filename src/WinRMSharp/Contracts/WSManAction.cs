namespace WinRMSharp.Contracts
{
    internal class WSManAction
    {
#pragma warning disable IDE1006 // Naming Styles
        public const string Create = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Create";
        public const string Delete = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete";

        public const string Command = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command";
        public const string Receive = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive";
        public const string Send = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Send";
        public const string Signal = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal";
#pragma warning disable IDE1006 // Naming Styles
    }
}
