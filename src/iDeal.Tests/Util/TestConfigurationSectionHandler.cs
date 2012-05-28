using iDeal.Configuration;


namespace iDeal.Tests.Util
{
    public class TestConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public string MerchantId { get; set; }

        public int MerchantSubId { get; set; }

        public string AcquirerUrl { get; set; }

        public string PrivateCertificateName { get; set; }

        public string PrivateCertificateStoreName { get; set; }

        public string PrivateCertificateFilename { get; set; }

        public string PrivateCertificatePassword { get; set; }

        public string PublicCertificateName { get; set; }

        public string PublicCertificateStoreName { get; set; }

        public string PublicCertificateFilename { get; set; }
    }
}
