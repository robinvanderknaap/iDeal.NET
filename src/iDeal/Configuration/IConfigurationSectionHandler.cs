using System.Security.Cryptography.X509Certificates;
namespace iDeal.Configuration
{
    public interface IConfigurationSectionHandler
    {
        string MerchantId { get; }
        int MerchantSubId { get; }
        string AcquirerUrl { get; }
        
        StoreLocation? AcceptantCertificateStoreLocation { get; }
        string AcceptantCertificateThumbprint { get; }
        string AcceptantCertificateStoreName { get; }
        string AcceptantCertificateFilename { get; }
        string AcceptantCertificatePassword { get; }

        StoreLocation? AcquirerCertificateStoreLocation { get; }
        string AcquirerCertificateThumbprint { get; }
        string AcquirerCertificateStoreName { get; }
        string AcquirerCertificateFilename { get; }
    }
}