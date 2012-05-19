using iDeal.Configuration;


namespace iDeal.Tests.Util
{
    public class TestConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public string MerchantId { get; set; }

        public int MerchantSubId { get; set; }

        public string AcquirerUrl { get; set; }

        public string CertificateName { get; set; }

        public string CertificateStoreName { get; set; }

        public string CertificateFilename { get; set; }

        public string CertificatePassword { get; set; }

        public string BankCertificateName { get; set; }

        public string BankCertificateStoreName { get; set; }

        public string BankCertificateFilename { get; set; }
    }
}
