using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NUnit.Framework;
using iDeal.Tests.Util;

namespace iDeal.Tests.Unit
{
    public class ServiceTests : TestFixture
    {
        [Test]
        public void CanInstantiateiDealServiceWithDefaultConfiguration()
        {
            Assert.DoesNotThrow(() => new iDealService());
        }

        [Test]
        public void CanInstantiateServiceWithCustomConfiguration()
        {
            Assert.DoesNotThrow(() => new iDealService(GetTestConfiguration()));
        }

        [Test]
        public void CannotCreateServiceWithEmptyMerchantId()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.MerchantId = " ";
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithMerchantIdContainingMoreThanNineCharacters()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.MerchantId = "0123456789";
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithSubIdOutOfRange()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.MerchantSubId = 7;
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithEmptyAcquirerUrl()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.AcquirerUrl = " ";
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithoutPrivateCertificate()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.PrivateCertificate = null;
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithoutPrivateKeyInPrivateCertificate()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.PrivateCertificate = new X509Certificate2("Util\\TestCertificates\\CertificateWithoutPrivateKey.cer");
                new iDealService(configuration);
            });
        }

        [Test]
        public void CannotCreateServiceWithoutPublicCertificate()
        {
            Assert.Throws<ConfigurationErrorsException>(delegate
            {
                var configuration = GetTestConfiguration();
                configuration.PublicCertificate = null;
                new iDealService(configuration);
            });
        }
    }
}
