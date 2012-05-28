using System;
using iDeal.SignatureProviders;
using iDeal.Status;
using iDeal.Tests.Util;
using NUnit.Framework;
using System.IO;

namespace iDeal.Tests.Unit
{
    public class StatusTests : TestFixture
    {
        [Test]
        public void CanCreateStatusRequest()
        {
            var statusRequest = new StatusRequest("123456789", 0, "0123456789123456");

            Assert.AreEqual("123456789", statusRequest.MerchantId);
            Assert.AreEqual(0, statusRequest.MerchantSubId);
            Assert.AreEqual("0123456789123456", statusRequest.TransactionId);
        }

        [Test]
        public void TransactionIdCannotBeEmpty()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var statusRequest = new StatusRequest("123456789", 0, " ");
            });
        }

        [Test]
        public void TransactionIdMustContain16Characters()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                var statusRequest = new StatusRequest("123456789", 0, "123456789123456");
            });
        }

        [Test]
        public void CanGetXmlRepresentationOfStatusRequest()
        {
            var statusRequest = new StatusRequest("123456789", 0, "0123456789123456");
            var xmlStatusRequest = statusRequest.ToXml(new SignatureProvider(PrivateCertificate, PublicCertificate));

            Assert.IsNotNullOrEmpty(xmlStatusRequest);
        }

        [Test]
        public void CanCreateStatusResponseSuccess()
        {
            var statusResponseSuccess = new StreamReader("Util\\TestResponses\\StatusResponseSuccess.xml").ReadToEnd();

            var statusResponse = new StatusResponse(statusResponseSuccess);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual("0000000000078316", statusResponse.TransactionId);
            Assert.AreEqual(Status.Status.Success, statusResponse.Status);
            Assert.AreEqual("Webpirates", statusResponse.ConsumerName);
            Assert.AreEqual("108429563", statusResponse.ConsumerAccountNumber);
            Assert.AreEqual("Meppel", statusResponse.ConsumerCity);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
            Assert.AreEqual(statusResponse.CreateDateTimeStamp + "0000000000078316Success108429563", statusResponse.MessageDigest);
            Assert.AreEqual("LAwZjEiH+Z8BAzRTNGopGq3OT8V80if3H6pKcNfWcyyXb0yqzYcX3/+vkTeirq+A4Sv7UuVdrcmGqzbZW7kDzX1/fxbyGahfYlsbysQLvxDdR6ExIjHohRx1RHwFZ1NO1hbw3R4ab27hHBz43gtytz4YP5nT6B9zwW+eHIlNIuY=", statusResponse.SignatureValue);
        }

        [Test]
        public void CanCreateStatusResponseCancelled()
        {
            var statusResponseCancelled = new StreamReader("Util\\TestResponses\\StatusResponseCancelled.xml").ReadToEnd();

            var statusResponse = new StatusResponse(statusResponseCancelled);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual("0000000000078321", statusResponse.TransactionId);
            Assert.AreEqual(Status.Status.Cancelled, statusResponse.Status);
            Assert.IsNullOrEmpty(statusResponse.ConsumerName);
            Assert.IsNullOrEmpty(statusResponse.ConsumerAccountNumber);
            Assert.IsNullOrEmpty(statusResponse.ConsumerCity);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
            Assert.AreEqual(statusResponse.CreateDateTimeStamp + "0000000000078321Cancelled", statusResponse.MessageDigest);
            Assert.AreEqual("RTK+t4Nr5LRPfgOhWkUpECFts4GWbu2kKTw7VYmISdBl870D3rYIhg9gdaDPhXuj25hJ/ztwbCNKYNf+MnymTeOszIm1xdWDvuCHe4WT3F8vA+6rZIiPBUcVqFfyXAfj8Tr9pSsFWppWxzYddo0tGOkF5g8JxHB91NltEdWtzbY=", statusResponse.SignatureValue);
        }

        [Test]
        public void CanCreateStatusResponseExpired()
        {
            var statusResponseExpired = new StreamReader("Util\\TestResponses\\StatusResponseExpired.xml").ReadToEnd();
            
            var statusResponse = new StatusResponse(statusResponseExpired);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual("0000000000078324", statusResponse.TransactionId);
            Assert.AreEqual(Status.Status.Expired, statusResponse.Status);
            Assert.IsNullOrEmpty(statusResponse.ConsumerName);
            Assert.IsNullOrEmpty(statusResponse.ConsumerAccountNumber);
            Assert.IsNullOrEmpty(statusResponse.ConsumerCity);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
            Assert.AreEqual(statusResponse.CreateDateTimeStamp + "0000000000078324Expired", statusResponse.MessageDigest);
            Assert.AreEqual("ijQkrIqUzZbSWzFrQzPb9iYJ1NRJlZtFH1u0r6TbwkzltPgAGpbkoOdiPRiYC0kBEixJ3eTC9QhKGDy3dk1h5b7cBo1vdiCa8odKPMZ8wb1cP9qoy3EO+DKUKuutwJW2S4ocGtv1d5StJWslDkYvVlXHhOjN/t15NRVKb5jpvmU=", statusResponse.SignatureValue);
        }

        [Test]
        public void CanCreateStatusResponseFailure()
        {
            var statusResponseFailure = new StreamReader("Util\\TestResponses\\StatusResponseFailure.xml").ReadToEnd();

            var statusResponse = new StatusResponse(statusResponseFailure);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual("0000000000078322", statusResponse.TransactionId);
            Assert.AreEqual(Status.Status.Failure, statusResponse.Status);
            Assert.IsNullOrEmpty(statusResponse.ConsumerName);
            Assert.IsNullOrEmpty(statusResponse.ConsumerAccountNumber);
            Assert.IsNullOrEmpty(statusResponse.ConsumerCity);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
            Assert.AreEqual(statusResponse.CreateDateTimeStamp + "0000000000078322Failure", statusResponse.MessageDigest);
            Assert.AreEqual("MRscIWUKhPQAcuPxTF+PUKPt/FnMGMSG2jtw3CHUGCJ+KiMKDq6F3wdU/hzhKwqkg6iEzRvSLzZtuh6vQXHyx5f0NQ3sz63nkl6SoU+mRM6c3qWZs6+ckzNOfMRXClhZVGU87t+th3x1IVkvcS+jIgvG9F0JLlz7rt3oU4htDSU=", statusResponse.SignatureValue);
        }

        [Test]
        public void CanCreateStatusResponseOpen()
        {
            var statusResponseOpen = new StreamReader("Util\\TestResponses\\StatusResponseOpen.xml").ReadToEnd();

            var statusResponse = new StatusResponse(statusResponseOpen);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual("0000000000078323", statusResponse.TransactionId);
            Assert.AreEqual(Status.Status.Open, statusResponse.Status);
            Assert.IsNullOrEmpty(statusResponse.ConsumerName);
            Assert.IsNullOrEmpty(statusResponse.ConsumerAccountNumber);
            Assert.IsNullOrEmpty(statusResponse.ConsumerCity);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
            Assert.AreEqual(statusResponse.CreateDateTimeStamp + "0000000000078323Open", statusResponse.MessageDigest);
            Assert.AreEqual("UiyTyuINsQwL1FmpSnhvcHQ1p/wTUiE07Ag4XAIETFCNPGK5ag0PfktPfAYKXX3uB38fhuaoMddWlcpQ9nJLDe+i+LKxg37ILGs7+LUgROGyV5DpSM78pvD3hanP8r4bLPuvNBNJYpeddcb91iLcopbNGHBudZufeaX8mT8xMkc=", statusResponse.SignatureValue);
        }

        [Test]
        public void CannotCreateStatusResponseWithUnknownStatus()
        {
            var statusResponseUnknown = new StreamReader("Util\\TestResponses\\StatusResponseUnknown.xml").ReadToEnd();
            Assert.Throws<InvalidOperationException>(() => new StatusResponse(statusResponseUnknown));
        }
    }
}
