using System;
using System.Linq;
using iDeal.Base;
using iDeal.Tests.Util;
using NUnit.Framework;

namespace iDeal.Tests.Integration
{
    public class Tests : TestFixture
    {
        [Test]
        public void CanSendDirectoryRequest()
        {
            var directoryResponse = new iDealService().SendDirectoryRequest();
            Assert.NotNull(directoryResponse);
            Assert.Greater(directoryResponse.Issuers.Count, 0);
        }

        [Test]
        public void CanSendTransactionRequest()
        {
            var directoryResponse = new iDealService().SendDirectoryRequest();
            var transactionResponse = new iDealService().SendTransactionRequest(directoryResponse.Issuers.First().Id, "http://webpirates.nl", "1", 200, TimeSpan.FromMinutes(30), "test", "test");

            Assert.NotNull(transactionResponse);
            Assert.AreEqual("1", transactionResponse.PurchaseId);
        }

        [Test]
        public void CanSendStatusRequest()
        {
            var directoryResponse = new iDealService().SendDirectoryRequest();
            var transactionResponse = new iDealService().SendTransactionRequest(directoryResponse.Issuers.First().Id, "http://www.webpirates.nl", "1", 3000, TimeSpan.FromMinutes(30), "test", "test");

            var statusResponse = new iDealService().SendStatusRequest(transactionResponse.TransactionId);

            Assert.AreEqual(Status.Status.Open, statusResponse.Status);
        }

        [Test]
        public void CanHandleErrorResponses()
        {
            var bogusConfiguration = new TestConfiguration
            {
                AcquirerUrl = "https://www.ideal-simulator.nl:443/professional/",
                AcceptantCertificate = PrivateCertificate,
                AcquirerCertificate = PublicCertificate,
                MerchantSubId = 0,
                MerchantId = "000000000" // faulty merchant id
            };

            Assert.Throws<iDealException>(delegate
            {
                var directoryResponse = new iDealService(bogusConfiguration).SendDirectoryRequest();
            });
        }
    }
}
