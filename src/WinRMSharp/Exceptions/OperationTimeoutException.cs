namespace WinRMSharp.Exceptions
{
    internal class OperationTimeoutException : Exception
    {
        public OperationTimeoutException()
        {
        }

        public OperationTimeoutException(string message)
            : base(message)
        {
        }

        public OperationTimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
