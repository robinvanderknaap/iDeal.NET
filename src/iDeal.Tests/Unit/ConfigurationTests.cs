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
            Assert.AreEqual("Util\\TestCertificates\\idealsim_private.pfx", config.Certificate.Filename);
            Assert.AreEqual("idealsim", config.Certificate.Password);
            Assert.AreEqual("My", config.Certificate.StoreName);
            Assert.AreEqual("Test", config.Certificate.Name);
            Assert.AreEqual("My", config.BankCertificate.StoreName);
            Assert.AreEqual("TestBank", config.BankCertificate.Name);
            Assert.AreEqual("Util\\TestCertificates\\idealsim_bank.cer", config.BankCertificate.Filename);
        }

        [Test]
        public void CanCreateDefaultConfigurationFromConfigFile()
        {
            var configurationSectionHandler = (ConfigurationSectionHandler)ConfigurationManager.GetSection("iDeal");

            var config = new DefaultConfiguration(configurationSectionHandler);
            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.Certificate);
            Assert.IsNotNull(config.BankCertificate);
        }

        [Test]
        public void CanCreateDefaultConfiguration()
        {
            var config = new DefaultConfiguration(new TestConfigurationSectionHandler
            {
                MerchantId = "123456789",
                MerchantSubId = 0,
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                CertificatePassword = "idealsim",
                BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
            });

            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.Certificate);
            Assert.IsNotNull(config.BankCertificate);
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
                    CertificatePassword = "idealsim",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void AtLeastStoreNameOrFileNameShouldBeSpecifiedForBankCertificateToCreateADefaultConfiguration()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    CertificatePassword = "idealsim",
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
                    CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
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
                CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                CertificatePassword = "idealsim",
                CertificateStoreName = "bogus",
                CertificateName = "bogus",
                BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer",
                BankCertificateStoreName = "bogus",
                BankCertificateName = "bogus"
            });

            Assert.IsNotNull(config.Certificate);
            Assert.IsNotNull(config.BankCertificate);
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
                    CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    CertificatePassword = "idealsim",
                    CertificateStoreName = "bogus",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }

        [Test]
        public void WhenStorenameIsSetForBankCertificateAlsoCertificateNameShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    CertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    CertificatePassword = "idealsim",
                    BankCertificateStoreName = "bogus",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
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
                    CertificateFilename = "Util\\TestCertificates\\idealsim_private.bogus",
                    CertificatePassword = "idealsim",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
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
                    CertificateStoreName = "Bogus",
                    CertificateName = "Bogus",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
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
                    CertificateStoreName = "My",
                    CertificateName = "Bogus",
                    BankCertificateFilename = "Util\\TestCertificates\\idealsim_bank.cer"
                });
            });
        }
    }
}
