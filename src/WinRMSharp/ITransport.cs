namespace WinRMSharp
{
    /// <summary>
    /// Encapsulates network transport for sending/receiving SOAP requests/responses
    /// over WinRM.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Maximum timeout to wait before an HTTP connect/read times out.
        /// </summary>
        TimeSpan ReadTimeout { get; }

        /// <summary>
        /// Sends an XML message to the destination host.
        /// </summary>
        /// <param name="message">XML request message to send</param>
        /// <returns>XML response message</returns>
        /// <exception cref="InvalidCredentialException">Thrown when provided credentials are invalid.</exception>
        /// <exception cref="TransportException">Thrown when response returns non success status code [200,299).</exception>
        Task<string> Send(string message);
    }
}
