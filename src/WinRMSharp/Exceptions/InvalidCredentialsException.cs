namespace WinRMSharp.Exceptions
{
    public class InvalidCredentialsException : TransportException
    {
        public InvalidCredentialsException()
        {
            Code = 401;
        }

        public InvalidCredentialsException(string message)
            : base(message)
        {
            Code = 401;
        }

        public InvalidCredentialsException(string message, Exception inner)
            : base(message, inner)
        {
            Code = 401;
        }
    }
}
