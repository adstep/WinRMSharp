namespace WinRMSharp.Exceptions
{
    public class TransportException : WinRMException
    {
        public int Code { get; set; } = 500;
        public string? Content { get; set; }

        public TransportException(int code, string content)
            : base($"Bad HTTP response returend from server. Code: {code} Content: {content}")
        {
            Code = code;
            Content = content;
        }

        public TransportException(int code, string content, Exception inner)
            : base($"Bad HTTP response returend from server. Code: {code} Content: {content}", inner)
        {
            Code = code;
            Content = content;
        }
    }
}
