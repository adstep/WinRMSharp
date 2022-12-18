namespace WinRMSharp.Tests.Utils
{
    internal class TemporaryFile : IDisposable
    {
        private readonly string _filePath;

        public TemporaryFile(string fileName)
        {
            _filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-{fileName}");
        }

        public static implicit operator string(TemporaryFile temporaryFile) => temporaryFile._filePath;

        public void Dispose()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
