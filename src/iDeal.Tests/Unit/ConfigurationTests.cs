using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using iDeal.Configuration;
using iDeal.Tests.Util;
using NUnit.Framework;

namespace iDeal.Tests.Unit
{
    public class ConfigurationTests : TestFixture
    {
        [Test]
        public void CanGetConfigurationFromConfigFile()
        {
            var config = (ConfigurationSectionHandler)ConfigurationManager.GetSection("iDeal");

            Assert.AreEqual("123456789", config.Merchant.Id);
            Assert.AreEqual(0, config.Merchant.SubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.Aquirer.Url);
            Assert.AreEqual("Util\\TestCertificates\\idealsim_private.pfx", config.PrivateCertificate.Filename);
            Assert.AreEqual("idealsim", config.PrivateCertificate.Password);
            Assert.AreEqual("My", config.PrivateCertificate.StoreName);
            Assert.AreEqual("Test", config.PrivateCertificate.Name);
            Assert.AreEqual("My", config.PublicCertificate.StoreName);
            Assert.AreEqual("TestBank", config.PublicCertificate.Name);
            Assert.AreEqual("Util\\TestCertificates\\idealsim_public.cer", config.PublicCertificate.Filename);
        }

        [Test]
        public void CanCreateDefaultConfigurationFromConfigFile()
        {
            var configurationSectionHandler = (ConfigurationSectionHandler)ConfigurationManager.GetSection("iDeal");

            var config = new DefaultConfiguration(configurationSectionHandler);
            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.PrivateCertificate);
            Assert.IsNotNull(config.PublicCertificate);
        }

        [Test]
        public void CanCreateDefaultConfiguration()
        {
            var config = new DefaultConfiguration(new TestConfigurationSectionHandler
            {
                MerchantId = "123456789",
                MerchantSubId = 0,
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                PrivateCertificatePassword = "idealsim",
                PublicCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
            });

            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.PrivateCertificate);
            Assert.IsNotNull(config.PublicCertificate);
        }

        [Test]
        public void AtLeastStoreNameOrFileNameShouldBeSpecifiedForCertificateToCreateDefaultConfiguration()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificatePassword = "idealsim",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
                });
            });
        }

        [Test]
        public void AtLeastStoreNameOrFileNameShouldBeSpecifiedForPublicCertificateToCreateADefaultConfiguration()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    PrivateCertificatePassword = "idealsim",
                });
            });
        }

        [Test]
        public void WhenFilenameIsSetPasswordShouldAlsoBeSet()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void FilenameTakesPresedenceOverStoreName()
        {
            var config = new DefaultConfiguration(new TestConfigurationSectionHandler
            {
                MerchantId = "123456789",
                MerchantSubId = 0,
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                PrivateCertificatePassword = "idealsim",
                PrivateCertificateStoreName = "bogus",
                PrivateCertificateName = "bogus",
                PublicCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer",
                PublicCertificateStoreName = "bogus",
                PublicCertificateName = "bogus"
            });

            Assert.IsNotNull(config.PrivateCertificate);
            Assert.IsNotNull(config.PublicCertificate);
        }

        [Test]
        public void WhenStorenameIsSetForCertificateAlsoCertificateNameShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    PrivateCertificatePassword = "idealsim",
                    PrivateCertificateStoreName = "bogus",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void WhenStorenameIsSetForPublicCertificateAlsoCertificateNameShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    PrivateCertificatePassword = "idealsim",
                    PublicCertificateStoreName = "bogus",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void ExceptionIsThrownWhenCertificateFilenameIsNotFound()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateFilename = "Util\\TestCertificates\\idealsim_private.bogus",
                    PrivateCertificatePassword = "idealsim",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void ExceptionIsThrownWhenCertificateStorenameIsNotFound()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateStoreName = "Bogus",
                    PrivateCertificateName = "Bogus",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void ExceptionIsThrownWhenCertificateNameIsNotFound()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    PrivateCertificateStoreName = "My",
                    PrivateCertificateName = "Bogus",
                    PublicCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }
    }
}
