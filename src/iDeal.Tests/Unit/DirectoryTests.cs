using System;
using System.Linq;
using iDeal.Directory;
using iDeal.Tests.Util;
using NUnit.Framework;
using System.IO;

namespace iDeal.Tests.Unit
{
    public class DirectoryTests : TestFixture
    {
        [Test]
        public void CanCreateDirectoryRequest()
        {
            var directoryRequest = new DirectoryRequest("100", 2);

            Assert.AreEqual("100", directoryRequest.MerchantId);
            Assert.AreEqual(2, directoryRequest.MerchantSubId);
            Assert.AreEqual(directoryRequest.CreateDateTimeStamp + "1002", directoryRequest.MessageDigest);
        }
        
        [Test]
        public void CannotSetEmptyMerchantId()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                    {
                        new DirectoryRequest("  ", 0);
                    });
        }

        [Test]
        public void CannotSetMerchantIdContainingMoreThanNineCharacters()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    new DirectoryRequest("0123456789", 0);
                });
        }

        [Test]
        public void MerchantIdCannotContainWhitespaces()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    new DirectoryRequest("0123 789", 0);
                });
        }

        [Test]
        public void SubIdCannotContainANegativeValue()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    new DirectoryRequest("123456789", -1);
                });
        }

        [Test]
        public void SubIdCannotContainValueGreaterThanSix()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    new DirectoryRequest("123456789", 7);
                });
        }

        [Test]
        public void SubIdDefaultsToZero()
        {
            var directoryRequest = new DirectoryRequest("ABCD", null);

            Assert.AreEqual(0, directoryRequest.MerchantSubId);
        }

        [Test]
        public void CanGetXmlRepresentationOfRequest()
        {
            var directoryRequest = new DirectoryRequest("ABCD", null);
            var xml = directoryRequest.ToXml(new SignatureProviders.SignatureProvider(PrivateCertificate, PublicCertificate));
            Assert.IsNotNullOrEmpty(xml);
        }

        [Test]
        public void CanCreateDirectoryResponse()
        {
            var directoryResponseXml = new StreamReader("Util\\TestResponses\\DirectoryResponse.xml").ReadToEnd();

            var directoryResponse = new DirectoryResponse(directoryResponseXml);

            Assert.AreEqual(20, directoryResponse.AcquirerId);
            Assert.AreEqual("2011-06-01T12:19:48.073Z", directoryResponse.CreateDateTimeStamp);
            Assert.AreEqual(DateTime.Parse("2011-06-01T12:19:48.073Z"), directoryResponse.CreateDateTimeStampLocalTime);
            Assert.AreEqual("2009-10-08T00:00:00.000Z", directoryResponse.DirectoryDateTimeStamp);
            Assert.AreEqual(DateTime.Parse("2009-10-08T00:00:00.000Z"), directoryResponse.DirectoryDateTimeStampLocalTime);
            Assert.AreEqual(3, directoryResponse.Issuers.Count);
            Assert.AreEqual(121, directoryResponse.Issuers.First().Id);
            Assert.AreEqual("Test Issuer", directoryResponse.Issuers.First().Name);
            Assert.AreEqual(ListType.Shortlist, directoryResponse.Issuers.First().ListType);
        }

       
    }
}
