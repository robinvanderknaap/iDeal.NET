using iDeal.Configuration;
using System.Security.Cryptography.X509Certificates;


namespace iDeal.Tests.Util
{
    public class TestConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public string MerchantId { get; set; }

        public int MerchantSubId { get; set; }

        public string AcquirerUrl { get; set; }

        public string AcceptantCertificateThumbprint { get; set; }

        public StoreLocation? AcceptantCertificateStoreLocation { get; set; }

        public string AcceptantCertificateStoreName { get; set; }

        public string AcceptantCertificateFilename { get; set; }

        public string AcceptantCertificatePassword { get; set; }

        public string AcquirerCertificateThumbprint { get; set; }

        public StoreLocation? AcquirerCertificateStoreLocation { get; set; }

        public string AcquirerCertificateStoreName { get; set; }

        public string AcquirerCertificateFilename { get; set; }
    }
}
