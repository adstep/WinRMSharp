namespace WinRMSharp.Tests.Sessions
{
    internal class Recording
    {
        public required RecordedRequest Request { get; set; }
        public required RecordedResponse Response { get; set; }
    }
}
