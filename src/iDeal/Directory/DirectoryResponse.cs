using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Directory
{
    public class DirectoryResponse : iDealResponse
    {
        private readonly IList<Issuer> _issuers = new List<Issuer>();

        public string DirectoryDateTimeStamp { get; private set; }

        public DateTime DirectoryDateTimeStampLocalTime { get { return DateTime.Parse(DirectoryDateTimeStamp); } }

        public IList<Issuer> Issuers
        {
            get { return new ReadOnlyCollection<Issuer>(_issuers); }
        }

        public DirectoryResponse(string xmlDirectoryResponse)
        {
            // Parse document
            var xDocument = XElement.Parse(xmlDirectoryResponse);
            XNamespace xmlNamespace = "http://www.idealdesk.com/Message";

            // Create datetimestamp
            CreateDateTimeStamp = xDocument.Element(xmlNamespace + "createDateTimeStamp").Value;
            
            // Acquirer id
            AcquirerId = (int)xDocument.Element(xmlNamespace + "Acquirer").Element(xmlNamespace + "acquirerID");

            // Directory datetimestamp
            DirectoryDateTimeStamp = xDocument.Element(xmlNamespace + "Directory").Element(xmlNamespace + "directoryDateTimeStamp").Value;

            // Get list of issuers
            foreach (var issuer in xDocument.Element(xmlNamespace + "Directory").Elements(xmlNamespace + "Issuer"))
            {
                _issuers.Add(
                        new Issuer(
                                (int)issuer.Element(xmlNamespace + "issuerID"),
                                issuer.Element(xmlNamespace + "issuerName").Value,
                                issuer.Element(xmlNamespace + "issuerList").Value == "Short" ? ListType.Shortlist : ListType.Longlist
                            )
                    );
            }
        }
    }
}
