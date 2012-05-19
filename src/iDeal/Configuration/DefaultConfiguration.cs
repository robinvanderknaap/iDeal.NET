using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using iDeal.Base;

namespace iDeal.Configuration
{
    /// <summary>
    /// Default configuration will load information from .config file
    /// </summary>
    public class DefaultConfiguration : IConfiguration
    {
        public string MerchantId { get; private set; }
        public int MerchantSubId { get; private set; }
        public string AcquirerUrl { get; private set; }
        public X509Certificate2 Certificate { get; private set; }
        public X509Certificate2 BankCertificate { get; private set; }

        public DefaultConfiguration(IConfigurationSectionHandler configurationSectionHandler)
        {
            MerchantId = configurationSectionHandler.MerchantId;
            MerchantSubId = configurationSectionHandler.MerchantSubId;
            AcquirerUrl = configurationSectionHandler.AcquirerUrl;

            // Validate private certificate settings:
            // Make sure either filename or storename is specified
            if(configurationSectionHandler.CertificateFilename.IsNullEmptyOrWhiteSpace() && configurationSectionHandler.CertificateStoreName.IsNullEmptyOrWhiteSpace())
                throw new ConfigurationErrorsException("You should either specify a filename or storename to specify the certificate's location");

            // When filename is set, password should also be set
            if(!configurationSectionHandler.CertificateFilename.IsNullEmptyOrWhiteSpace() && configurationSectionHandler.CertificatePassword.IsNullEmptyOrWhiteSpace())
                throw new ConfigurationErrorsException("Password is required. Only when certificate is loaded from filesystem");

            // When storename is set, certificate name should also be set
            if (!configurationSectionHandler.CertificateStoreName.IsNullEmptyOrWhiteSpace() && configurationSectionHandler.CertificateName.IsNullEmptyOrWhiteSpace())
                throw new ConfigurationErrorsException("Certificate name is required when loading certificate from store");

            // Validate bank certificate settings in config
            // Make sure either filename or storename is specified
            if (configurationSectionHandler.BankCertificateFilename.IsNullEmptyOrWhiteSpace() && configurationSectionHandler.BankCertificateStoreName.IsNullEmptyOrWhiteSpace())
                throw new ConfigurationErrorsException("You should either specify a filename or storename to specify the bank certificate's location");

            // When storename is set, certificate name should also be set
            if (!configurationSectionHandler.BankCertificateStoreName.IsNullEmptyOrWhiteSpace() && configurationSectionHandler.BankCertificateName.IsNullEmptyOrWhiteSpace())
                throw new ConfigurationErrorsException("Certificate name is required when loading bank certificate from store");


            // Set private certificate, filename takes precedence over certificate store
            Certificate = !configurationSectionHandler.CertificateFilename.IsNullEmptyOrWhiteSpace()
                              ? GetCertificateFromFile(configurationSectionHandler.CertificateFilename, configurationSectionHandler.CertificatePassword)
                              : GetCertificateFromStore(configurationSectionHandler.CertificateStoreName, configurationSectionHandler.CertificateName);

            // Set bank certificate, filename takes precedence over certificate store
            BankCertificate = !configurationSectionHandler.BankCertificateFilename.IsNullEmptyOrWhiteSpace()
                              ? GetCertificateFromFile(configurationSectionHandler.BankCertificateFilename, null)
                              : GetCertificateFromStore(configurationSectionHandler.BankCertificateStoreName, configurationSectionHandler.BankCertificateName);
            
        }
        
        private static X509Certificate2 GetCertificateFromFile(string relativePath, string password)
        {
            var absolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            
            try
            {
                return password != null ? new X509Certificate2(absolutePath, password) : new X509Certificate2(absolutePath);
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException("Could not load certificate file", exception);
            }
        }

        private static X509Certificate2 GetCertificateFromStore(string storeName, string certificateName)
        {
            var certificateStore = new X509Store(storeName, StoreLocation.LocalMachine);
            certificateStore.Open(OpenFlags.ReadOnly);
            
            foreach (var certificate in certificateStore.Certificates)
            {
                if (certificate.SubjectName.Name != null)
                {
                    if (certificate.SubjectName.Name.Contains(certificateName))
                    {
                        return certificate;
                    }
                }
            }
            
            throw new ConfigurationErrorsException("Certificate '" + certificateName + "' not found");
        
        }
    }
}
