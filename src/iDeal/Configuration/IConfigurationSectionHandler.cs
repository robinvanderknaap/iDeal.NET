namespace iDeal.Configuration
{
    public interface IConfigurationSectionHandler
    {
        string MerchantId { get; }
        int MerchantSubId { get; }
        string AcquirerUrl { get; }
        string CertificateName { get; }
        string CertificateStoreName { get; }
        string CertificateFilename { get; }
        string CertificatePassword { get; }
        string BankCertificateName { get; }
        string BankCertificateStoreName { get; }
        string BankCertificateFilename { get; }
    }
}