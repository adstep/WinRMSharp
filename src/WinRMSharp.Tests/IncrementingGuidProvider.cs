namespace WinRMSharp.Tests
{
    internal class IncrementingGuidProvider : IGuidProvider
    {
        private uint _counter = 0;

        public Guid NewGuid()
        {
            string result = "00000000-0000-0000-0000-000000000000";
            uint value = _counter++;

            string hexValue = value.ToString("X");

            // Max int resolves to '7FFFFFFF', which should completely fit in
            // the last segment of the guid.
            return Guid.Parse($"{result.Substring(0, result.Length - hexValue.Length)}{hexValue}");
        }
    }
}
