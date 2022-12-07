namespace WinRMSharp
{
    public class CommandState
    {
        public string Stdout { get; set; } = string.Empty;
        public string Stderr { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public bool Done { get; set; }
    }
}
