using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using System.IO;

namespace iDeal.Tests.Util
{
    [TestFixture]
    public class TestFixture
    {
        protected readonly X509Certificate2 PrivateCertificate = new X509Certificate2("Util\\TestCertificates\\idealsim_private.pfx", "idealsim");
        protected readonly X509Certificate2 PublicCertificate = new X509Certificate2("Util\\TestCertificates\\idealsim_public.cer");

        /// <summary>
        /// Ideal-simulator has an expired ssl certificate on their server, this makes sure the webrequest isn't cancelled
        /// </summary>
        [TestFixtureSetUp]
        protected void AllowInvalidSSLCertificate()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        // Callback used to validate the certificate in an SSL conversation
        protected static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
        
        protected TestConfiguration GetTestConfiguration()
        {
            return new TestConfiguration
            {
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                MerchantId = "123456789",
                AcceptantCertificate = PrivateCertificate,
                AcquirerCertificate = PublicCertificate
            };
        }
    }
}
