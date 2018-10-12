using System.Collections.Generic;

namespace Tabi
{
    public interface IApiConfiguration
    {
        string Url { get; set; }

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        /// <value>Timeout in seconds.</value>
        int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the sync interval in minutes.
        /// </summary>
        /// <value>Sync interval in minutes.</value>
        int SyncInterval { get; set; }

        string ClientIdentifier { get; set; }
        string ClientKey { get; set; }
        bool GzipRequests { get; set; }
        List<string> CertificateKeys { get; set; }
    }
}