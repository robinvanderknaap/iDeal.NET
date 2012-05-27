using System;
using iDeal.Tests.Util;
using iDeal.Transaction;
using NUnit.Framework;
using iDeal.SignatureProviders;

namespace iDeal.Tests.Unit
{
    public class TransactionTests : TestFixture
    {
        [Test]
        public void CanCreateTransactionRequest()
        {
            var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "1");

            Assert.AreEqual(1, transactionRequest.IssuerId);
            Assert.AreEqual("http://webpirates.nl", transactionRequest.MerchantReturnUrl);
            Assert.AreEqual("1", transactionRequest.PurchaseId);
            Assert.AreEqual(1000000, transactionRequest.Amount);
            Assert.AreEqual(TimeSpan.FromMinutes(30), transactionRequest.ExpirationPeriod);
            Assert.AreEqual("iMac 27", transactionRequest.Description);
            Assert.AreEqual("1", transactionRequest.EntranceCode);
        }

        [Test]
        public void CannotSetEmptyMerchantUrl()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "", "1", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "1");
            });
        }

        [Test]
        public void PurchaseIdCannotBeEmpty()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "1");
            });
        }

        [Test]
        public void PurchaseIdCannotContainMoreThan16Characters()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "12345678901234567", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "1");
            });
        }

        [Test]
        public void ExpirationPeriodCannotBeLessThan1Minute()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromSeconds(30), "iMac 27", "1");
            });
        }

        [Test]
        public void ExpirationPeriodCannotBeMoreThan60Minutes()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(61), "iMac 27", "1");
            });
        }

        [Test]
        public void DescriptionCannotContainMoreThan32Characters()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(30), "012345678901234567890123456789012", "1");
            });
        }

        [Test]
        public void EntranceCodeCannotContainMoreThan40Characters()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "01234567890123456789012345678901234567891");
            });
        }

        [Test]
        public void EntranceCodeIsRequired()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "");
            });
        }

        [Test]
        public void CanGetXmlRepresentationOfTransactionRequest()
        {
            var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, TimeSpan.FromMinutes(30), "iMac 27", "1");
            var xml = transactionRequest.ToXml(new SignatureProvider(Certificate, BankCertificate));
            Assert.IsNotNullOrEmpty(xml);
        }

        [Test]
        public void DefaultExpirationPeriodIs30Minutes()
        {
            var transactionRequest = new TransactionRequest("123456789", null, 1, "http://webpirates.nl", "1", 1000000, null, "iMac 27", "1");

            Assert.AreEqual(TimeSpan.FromMinutes(30), transactionRequest.ExpirationPeriod);
        }
    }
}
