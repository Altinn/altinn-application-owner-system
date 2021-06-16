using System;
using System.Collections.Generic;
using System.Text;

namespace AltinnApplicationOwnerSystem.Functions.Config
{
    /// <summary>
    /// Required configuration for the system
    /// </summary>
    public class AltinnApplicationOwnerSystemSettings
    {
        public string AppsBaseUrl { get; set; }

        public string PlatformBaseUrl { get; set; }

        public string BlobEndpoint { get; set; }

        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public string StorageContainer { get; set; }

        public string MaskinportenBaseAddress { get; set; }


        public string MaskinPortenClientId { get; set; }

        public bool TestMode { get; set; }

        public string LocalCertThumbprint { get; set; }
    }
}
