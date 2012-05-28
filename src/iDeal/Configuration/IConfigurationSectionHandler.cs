namespace iDeal.Configuration
{
    public interface IConfigurationSectionHandler
    {
        string MerchantId { get; }
        int MerchantSubId { get; }
        string AcquirerUrl { get; }
        string PrivateCertificateName { get; }
        string PrivateCertificateStoreName { get; }
        string PrivateCertificateFilename { get; }
        string PrivateCertificatePassword { get; }
        string PublicCertificateName { get; }
        string PublicCertificateStoreName { get; }
        string PublicCertificateFilename { get; }
    }
}