namespace WinRMSharp
{
    /// <summary>
    /// Represents a response from the WinRM Identify operation.
    /// Contains information about the remote machine including protocol and product details.
    /// </summary>
    public class IdentifyResponse
    {
        /// <summary>
        /// Gets or sets the WinRM protocol version supported by the server.
        /// </summary>
        public required string ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the vendor of the operating system on the remote machine.
        /// </summary>
        public required string ProductVendor { get; set; }

        /// <summary>
        /// Gets or sets the version of the operating system on the remote machine.
        /// </summary>
        public required string ProductVersion { get; set; }
    }
}
