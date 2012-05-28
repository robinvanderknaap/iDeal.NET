using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDeal.Tests.Util;
using NUnit.Framework;
using iDeal.SignatureProviders;

namespace iDeal.Tests.Unit
{
    class SignatureProviderTests : TestFixture
    {
        [Test]
        public void CanGetSignature()
        {
            var signatureProvider = new SignatureProvider(PrivateCertificate, PublicCertificate);
            var messageDigest = "2012-05-19T17:27:16.630Z1234567890";
            Assert.AreEqual("YVS+Yrr280+ztQ44Qiwp0jYaRO5YJivg/HK7+/AdqUTDLYkPf2hqW6PO1n04LNOhGzAMgCgBmfkxV3fkEAnjmX/tcXU0mnwFxmQC8CjQJ/xpnXo/LDYJtkmfpaFLv0aJCWa2BSNB71Ygr8mtZQ/tach/jaaJwyWbpn+3wkUl7Zc=", signatureProvider.GetSignature(messageDigest));
        }

        [Test]
        public void CanVerifySignature()
        {
            var signatureProvider = new SignatureProvider(PrivateCertificate, PublicCertificate);
            var messageDigest = "2012-05-19T17:31:18.000Z0000000000078330Success108429563";
            var signature = "BXgUCWykw5I+4aDMC0gcJIY4crVfsPMt9NtTpLD9hzANEZB+gZIUOB6iIX2aS8AyxlDEikhM5eCA6UirtIbqcW94W4z0ekvtgBm0dUROACsZxHxqTuSRYDy22a+Qg92ei1eQ6GH0245BCAo3B4H48A91oFCOc963rJ43DrHd0x8=";
            Assert.IsTrue(signatureProvider.VerifySignature(signature, messageDigest));
        }

        [Test]
        public void CanGetThumbprint()
        {
            var signatureProvider = new SignatureProvider(PrivateCertificate, PublicCertificate);
            Assert.AreEqual(PrivateCertificate.Thumbprint, signatureProvider.GetThumbprintPrivateCertificate());
        }

        [Test]
        public void CanGetThumbprintPublicCertificate()
        {
            var signatureProvider = new SignatureProvider(PrivateCertificate, PublicCertificate);
            Assert.AreEqual(PublicCertificate.Thumbprint, signatureProvider.GetThumbprintPublicCertificate());
        }
    }
}
