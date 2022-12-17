namespace WinRMSharp.Exceptions
{
    /// <summary>
    /// Wraps WinRM transport errors (unexpected HTTP error codes, etc). 
    /// </summary>
    public class TransportException : WinRMException
    {
        /// <summary>
        /// Http status code of response.
        /// </summary>
        public int Code { get; set; } = 500;

        /// <summary>
        /// Content of response message.
        /// </summary>
        public string? Content { get; set; }

        public TransportException(int code, string content, Exception inner)
            : base($"Bad HTTP response returend from server. Code: {code} Content: {content}", inner)
        {
            Code = code;
            Content = content;
        }
    }
}
