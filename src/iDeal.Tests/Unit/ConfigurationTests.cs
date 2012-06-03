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
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.Acquirer.Url);
            Assert.AreEqual("Util\\TestCertificates\\idealsim_private.pfx", config.AcceptantCertificateFilename);
            Assert.AreEqual("idealsim", config.AcceptantCertificatePassword);
            Assert.AreEqual(StoreLocation.LocalMachine, config.AcceptantCertificateStoreLocation);
            Assert.AreEqual("My", config.AcceptantCertificateStoreName);
            Assert.AreEqual("48 fa ca 26 2a 9f 76 66 67 f0 bf 2f ed 54 b8 db 16 f9 10 87", config.AcceptantCertificateThumbprint);
            Assert.AreEqual(StoreLocation.LocalMachine, config.AcquirerCertificateStoreLocation);
            Assert.AreEqual("My", config.AcquirerCertificateStoreName);
            Assert.AreEqual("6c fc 36 38 9c 7a 3c 49 44 0b 87 33 d2 58 cb 21 67 fa c1 8f", config.AcquirerCertificateThumbprint);
            Assert.AreEqual("Util\\TestCertificates\\idealsim_public.cer", config.AcquirerCertificateFilename);
        }

        [Test]
        public void CanCreateDefaultConfigurationFromConfigFile()
        {
            var configurationSectionHandler = (ConfigurationSectionHandler)ConfigurationManager.GetSection("iDeal");

            var config = new DefaultConfiguration(configurationSectionHandler);
            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.AcceptantCertificate);
            Assert.IsNotNull(config.AcquirerCertificate);
        }

        [Test]
        public void CanCreateDefaultConfiguration()
        {
            var config = new DefaultConfiguration(new TestConfigurationSectionHandler
            {
                MerchantId = "123456789",
                MerchantSubId = 0,
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                AcceptantCertificatePassword = "idealsim",
                AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
            });

            Assert.AreEqual("123456789", config.MerchantId);
            Assert.AreEqual(0, config.MerchantSubId);
            Assert.AreEqual("https://www.ideal-simulator.nl:443/professional/", config.AcquirerUrl);
            Assert.IsNotNull(config.AcceptantCertificate);
            Assert.IsNotNull(config.AcquirerCertificate);
        }

        [Test]
        public void FilenameTakesPresedenceOverStoreLocation()
        {
            var config = new DefaultConfiguration(new TestConfigurationSectionHandler
            {
                MerchantId = "123456789",
                MerchantSubId = 0,
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                AcceptantCertificatePassword = "idealsim",
                AcceptantCertificateStoreLocation = StoreLocation.LocalMachine,
                AcceptantCertificateStoreName = "bogus",
                AcceptantCertificateThumbprint = "bogus",
                AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer",
                AcquirerCertificateStoreLocation = StoreLocation.LocalMachine,
                AcquirerCertificateStoreName = "bogus",
                AcquirerCertificateThumbprint = "bogus"
            });

            Assert.IsNotNull(config.AcceptantCertificate);
            Assert.IsNotNull(config.AcquirerCertificate);
        }

        [Test]
        public void StoreLocationOrFileNameShouldBeSpecifiedForAcceptantCertificateToCreateDefaultConfiguration()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificatePassword = "idealsim",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer",
                });
            });
        }

        [Test]
        public void WhenFilenameIsSetPasswordShouldAlsoBeSetForAcceptantCertificate()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
                });
            });
        }

        [Test]
        public void WhenStoreLocationIsSetForAcceptantCertificateAlsoStorenameShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcceptantCertificateThumbprint = "1234",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
                });
            });
        }

        [Test]
        public void WhenStoreLocationIsSetForAcceptantCertificateAlsoCertificateThumbprintShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcceptantCertificateStoreName = "bogus",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
                });
            });
        }

        [Test]
        public void StoreLocationOrFileNameShouldBeSpecifiedForAcquirerCertificateToCreateDefaultConfiguration()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    AcceptantCertificatePassword = "idealsim",
                });
            });
        }

        [Test]
        public void WhenStoreLocationIsSetForAcquirerCertificateAlsoStoreNameShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    AcceptantCertificatePassword = "idealsim",
                    AcquirerCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcquirerCertificateThumbprint = "1234"
                });
            });
        }

        [Test]
        public void WhenStoreLocationIsSetForAcquirerCertificateAlsoCertificateThumbprintShouldBeSupplied()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.pfx",
                    AcceptantCertificatePassword = "idealsim",
                    AcquirerCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcquirerCertificateStoreName = "bogus"
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
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_private.bogus",
                    AcceptantCertificatePassword = "idealsim",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
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
                    AcceptantCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcceptantCertificateStoreName = "Bogus",
                    AcceptantCertificateThumbprint = "Bogus",
                    AcquirerCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer"
                });
            });
        }

        [Test]
        public void ExceptionIsThrownWhenCertificateThumbprintIsNotFound()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                new DefaultConfiguration(new TestConfigurationSectionHandler
                {
                    MerchantId = "123456789",
                    MerchantSubId = 0,
                    AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                    AcquirerCertificateStoreLocation = StoreLocation.LocalMachine,
                    AcquirerCertificateStoreName = "My",
                    AcquirerCertificateThumbprint = "Bogus",
                    AcceptantCertificateFilename = "Util\\TestCertificates\\idealsim_public.cer",
                    AcceptantCertificatePassword = "12345"
                });
            });
        }
    }
}
