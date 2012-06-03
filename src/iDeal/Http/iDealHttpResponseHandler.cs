using System;
using System.IO;
using System.Security;
using System.Xml.Linq;
using iDeal.Base;
using iDeal.Directory;
using iDeal.SignatureProviders;
using iDeal.Status;
using iDeal.Transaction;

namespace iDeal.Http
{
    public class iDealHttpResponseHandler : IiDealHttpResponseHandler
    {
        public iDealResponse HandleResponse(string response, ISignatureProvider signatureProvider)
        {
            var xDocument = XElement.Parse(response);

            switch (xDocument.Name.LocalName)
            {
                case "DirectoryRes":
                    return new DirectoryResponse(response);
                
                case "AcquirerTrxRes":
                    return new TransactionResponse(response);
                
                case "AcquirerStatusRes":
                    var statusResponse = new StatusResponse(response);

                    // Check fingerprint
                    if (statusResponse.Fingerprint != signatureProvider.GetThumbprintAcquirerCertificate())
                        throw new SecurityException("Signature fingerprint from status respone does not match fingerprint acquirer's certificate");

                    // Check digital signature
                    if (!signatureProvider.VerifySignature(statusResponse.SignatureValue, statusResponse.MessageDigest))
                        throw new SecurityException("Signature status response from acquirer's certificate is not valid");
                    
                        
                    return statusResponse;
                
                case "ErrorRes":
                    throw new iDealException(xDocument);

                default:
                    throw new InvalidDataException("Unknown response");
            }
        }
    }
}
