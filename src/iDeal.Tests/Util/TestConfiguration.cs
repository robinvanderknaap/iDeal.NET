using System;
using System.Security.Cryptography.X509Certificates;
using iDeal.Configuration;

namespace iDeal.Tests.Util
{
    public class TestConfiguration : IConfiguration
    {
        public string MerchantId { get; set; }

        public int MerchantSubId { get; set; }

        public string AcquirerUrl { get; set; }

        public X509Certificate2 Certificate { get; set; }

        public X509Certificate2 BankCertificate { get; set; }
    }
}
