namespace WinRMSharp.Exceptions
{
    /// <summary>
    /// Base exeception for encapsulating all WinRM exceptions.
    /// </summary>
    public class WinRMException : Exception
    {
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
