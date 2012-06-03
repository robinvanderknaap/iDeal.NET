using System.Security.Cryptography.X509Certificates;

namespace iDeal.Configuration
{
    public interface IConfiguration
    {
        string MerchantId { get; }
        int MerchantSubId { get; }
        string AcquirerUrl { get; }
        X509Certificate2 AcceptantCertificate { get; }
        X509Certificate2 AcquirerCertificate { get; }
    }
}
