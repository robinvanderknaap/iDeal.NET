using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDeal.Tests.Util;
using iDeal.Http;
using System.IO;
using Moq;
using iDeal.SignatureProviders;
using iDeal.Base;
using NUnit.Framework;
using iDeal.Directory;
using iDeal.Transaction;
using iDeal.Status;
using System.Security;

namespace iDeal.Tests.Unit
{
    class iDealHttpResponseHandlerTests : TestFixture
    {
        [Test]
        public void CanHandleDirectoryResponse()
        {
            var directoryResponseXml = new StreamReader("Util\\TestResponses\\DirectoryResponse.xml").ReadToEnd();
            var signatureProvider = new Mock<ISignatureProvider>().Object;
            var responseHandler = new iDealHttpResponseHandler();

            var directoryResponse = (DirectoryResponse)responseHandler.HandleResponse(directoryResponseXml, signatureProvider);

            Assert.AreEqual(20, directoryResponse.AcquirerId);
            Assert.AreEqual(3, directoryResponse.Issuers.Count);
            Assert.AreEqual(121, directoryResponse.Issuers[0].Id);
            Assert.AreEqual("Test Issuer", directoryResponse.Issuers[0].Name);
            Assert.AreEqual(ListType.Shortlist, directoryResponse.Issuers[0].ListType);
        }

        [Test]
        public void CanHandleTransactionResponse()
        {
            var transactionResponseXml = new StreamReader("Util\\TestResponses\\TransactionResponse.xml").ReadToEnd();
            var signatureProvider = new Mock<ISignatureProvider>().Object;
            var responseHandler = new iDealHttpResponseHandler();

            var transactionResponse = (TransactionResponse)responseHandler.HandleResponse(transactionResponseXml, signatureProvider);

            Assert.AreEqual(8, transactionResponse.AcquirerId);
            Assert.AreEqual("https://www.ideal-simulator.nl/professional/payment.php?trxid=0000000000078329&ec=498b1489-b393-44", transactionResponse.IssuerAuthenticationUrl);
            Assert.AreEqual("0000000000078329", transactionResponse.TransactionId);
            Assert.AreEqual("526fd526-5825-4a", transactionResponse.PurchaseId);
        }

        [Test]
        public void CanHandleStatusResponse()
        {
            var statusResponseXml = new StreamReader("Util\\TestResponses\\StatusResponseSuccess.xml").ReadToEnd();
            
            // Setup signature provider
            var signatureProvider = new Mock<ISignatureProvider>();
            signatureProvider.Setup(x => x.VerifySignature("LAwZjEiH+Z8BAzRTNGopGq3OT8V80if3H6pKcNfWcyyXb0yqzYcX3/+vkTeirq+A4Sv7UuVdrcmGqzbZW7kDzX1/fxbyGahfYlsbysQLvxDdR6ExIjHohRx1RHwFZ1NO1hbw3R4ab27hHBz43gtytz4YP5nT6B9zwW+eHIlNIuY=", "2012-05-19T12:20:01.000Z0000000000078316Success108429563")).Returns(true);
            signatureProvider.Setup(x => x.GetThumbprintPublicCertificate()).Returns("6CFC36389C7A3C49440B8733D258CB2167FAC18F");
            
            var responseHandler = new iDealHttpResponseHandler();

            var statusResponse = (StatusResponse)responseHandler.HandleResponse(statusResponseXml, signatureProvider.Object);

            Assert.AreEqual(8, statusResponse.AcquirerId);
            Assert.AreEqual(Status.Status.Success, statusResponse.Status);
            Assert.AreEqual("Webpirates", statusResponse.ConsumerName);
            Assert.AreEqual("108429563", statusResponse.ConsumerAccountNumber);
            Assert.AreEqual("Meppel", statusResponse.ConsumerCity);
            Assert.AreEqual("LAwZjEiH+Z8BAzRTNGopGq3OT8V80if3H6pKcNfWcyyXb0yqzYcX3/+vkTeirq+A4Sv7UuVdrcmGqzbZW7kDzX1/fxbyGahfYlsbysQLvxDdR6ExIjHohRx1RHwFZ1NO1hbw3R4ab27hHBz43gtytz4YP5nT6B9zwW+eHIlNIuY=", statusResponse.SignatureValue);
            Assert.AreEqual("6CFC36389C7A3C49440B8733D258CB2167FAC18F", statusResponse.Fingerprint);
        }

        [Test]
        public void CannotHandleStatusResponseWhenFingerPrintsDoNoMatch()
        {
            var statusResponseXml = new StreamReader("Util\\TestResponses\\StatusResponseSuccess.xml").ReadToEnd();

            // Setup signature provider
            var signatureProvider = new Mock<ISignatureProvider>();
            signatureProvider.Setup(x => x.VerifySignature("LAwZjEiH+Z8BAzRTNGopGq3OT8V80if3H6pKcNfWcyyXb0yqzYcX3/+vkTeirq+A4Sv7UuVdrcmGqzbZW7kDzX1/fxbyGahfYlsbysQLvxDdR6ExIjHohRx1RHwFZ1NO1hbw3R4ab27hHBz43gtytz4YP5nT6B9zwW+eHIlNIuY=", "2012-05-19T12:20:01.000Z0000000000078316Success108429563")).Returns(true);
            signatureProvider.Setup(x => x.GetThumbprintPublicCertificate()).Returns("bogus");

            var responseHandler = new iDealHttpResponseHandler();

            Assert.Throws<SecurityException>(() =>
                responseHandler.HandleResponse(statusResponseXml, signatureProvider.Object)
            );
        }

        [Test]
        public void CannotHandleStatusResponseWhenSignatureDoesNotMatch()
        {
            var statusResponseXml = new StreamReader("Util\\TestResponses\\StatusResponseSuccess.xml").ReadToEnd();

            // Setup signature provider
            var signatureProvider = new Mock<ISignatureProvider>();
            signatureProvider.Setup(x => x.VerifySignature("LAwZjEiH+Z8BAzRTNGopGq3OT8V80if3H6pKcNfWcyyXb0yqzYcX3/+vkTeirq+A4Sv7UuVdrcmGqzbZW7kDzX1/fxbyGahfYlsbysQLvxDdR6ExIjHohRx1RHwFZ1NO1hbw3R4ab27hHBz43gtytz4YP5nT6B9zwW+eHIlNIuY=", "2012-05-19T12:20:01.000Z0000000000078316Success108429563")).Returns(false);
            signatureProvider.Setup(x => x.GetThumbprintPublicCertificate()).Returns("6CFC36389C7A3C49440B8733D258CB2167FAC18F");

            var responseHandler = new iDealHttpResponseHandler();

            Assert.Throws<SecurityException>(() =>
                responseHandler.HandleResponse(statusResponseXml, signatureProvider.Object)
            );
        }

        [Test]
        public void CanHandleErrorResponse()
        {
            var errorResponse = new StreamReader("Util\\TestResponses\\ErrorResponse.xml").ReadToEnd();
            var signatureProvider = new Mock<ISignatureProvider>().Object;
            
            var responseHandler = new iDealHttpResponseHandler();

            Assert.Throws<iDealException>(() => responseHandler.HandleResponse(errorResponse, signatureProvider));

            try
            {
                responseHandler.HandleResponse(errorResponse, signatureProvider);
            }
            catch (iDealException e)
            {
                Assert.AreEqual("Betalen met iDEAL is nu niet mogelijk. Probeer het later nogmaals of betaal op een andere manier.", e.ConsumerMessage);
                Assert.AreEqual("SE2700", e.ErrorCode);
                Assert.AreEqual("System generating error: acquirer", e.ErrorDetail);
                Assert.AreEqual("Invalid electronic signature", e.ErrorMessage);
            }
        }

        [Test]
        public void CannotHandleUnknownResponses()
        {
            var unknownResponseXml = new StreamReader("Util\\TestResponses\\UnknownResponse.xml").ReadToEnd();

            // Setup signature provider
            var signatureProvider = new Mock<ISignatureProvider>();

            var responseHandler = new iDealHttpResponseHandler();

            Assert.Throws<InvalidDataException>(() =>
                responseHandler.HandleResponse(unknownResponseXml, signatureProvider.Object)
            );
        }
    }
}
