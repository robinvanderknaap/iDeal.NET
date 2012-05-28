using System;
using System.Xml.Linq;
using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Directory
{
    public class DirectoryRequest : iDealRequest
    {
        public DirectoryRequest(string merchantId, int? subId)
        {
            MerchantId = merchantId;
            MerchantSubId = subId ?? 0; // If no sub id is specified, sub id should be 0
        }
        
        public override string MessageDigest
        {
            get { return CreateDateTimeStamp + MerchantId + MerchantSubId; }
        }

        /// <summary>
        /// Creates xml representation of directory request
        /// </summary>
        public override string ToXml(ISignatureProvider signatureProvider)
        {
            XNamespace xmlNamespace = "http://www.idealdesk.com/Message";

            var directoryRequestXmlMessage =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(xmlNamespace + "DirectoryReq",
                        new XAttribute("version", "1.1.0"),
                        new XElement(xmlNamespace + "createDateTimeStamp", CreateDateTimeStamp),
                        new XElement(xmlNamespace + "Merchant",
                            new XElement(xmlNamespace + "merchantID", MerchantId.PadLeft(9, '0')),
                            new XElement(xmlNamespace + "subID", MerchantSubId),
                            new XElement(xmlNamespace + "authentication", "SHA1_RSA"),
                            new XElement(xmlNamespace + "token", signatureProvider.GetThumbprintPrivateCertificate()),
                            new XElement(xmlNamespace + "tokenCode", signatureProvider.GetSignature(CreateDateTimeStamp + MerchantId + MerchantSubId))
                        )
                    )
                );

            //return directoryRequestXmlMessage.Declaration + directoryRequestXmlMessage.ToString(SaveOptions.OmitDuplicateNamespaces);
            return directoryRequestXmlMessage.Declaration + directoryRequestXmlMessage.ToString(SaveOptions.None);
        }
    }
}