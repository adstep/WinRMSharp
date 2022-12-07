namespace WinRMSharp
{
    public interface ITransport
    {
        Task<string> Send(string message);
    }
}
