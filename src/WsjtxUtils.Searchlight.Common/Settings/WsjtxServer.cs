namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// WSJT-X Server settings class
    /// </summary>
    public class WsjtxServer
    {
        /// <summary>
        /// Constructs server settings
        /// </summary>
        public WsjtxServer() : this(string.Empty, 0)
        {
        }

        /// <summary>
        /// Constructs server settings
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public WsjtxServer(string address, int port)
        {
            Address = address;
            Port = port;
        }

        /// <summary>
        /// IP Address for the server
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Port for the server
        /// </summary>
        public int Port { get; set; }
    }
}
