namespace WinRMSharp.Exceptions
{
    public class WinRMException : Exception
    {
        public int Code { get; set; } = 500;

        public WinRMException()
        {
        }

        public WinRMException(string message)
            : base(message)
        {
        }

        public WinRMException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
