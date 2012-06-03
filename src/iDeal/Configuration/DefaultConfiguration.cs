using System;
using System.Linq;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using iDeal.Base;
using System.Diagnostics;
using System.IO;

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
        public X509Certificate2 AcceptantCertificate { get; private set; }
        public X509Certificate2 AcquirerCertificate { get; private set; }

        public DefaultConfiguration(IConfigurationSectionHandler configurationSectionHandler)
        {
            MerchantId = configurationSectionHandler.MerchantId;
            MerchantSubId = configurationSectionHandler.MerchantSubId;
            AcquirerUrl = configurationSectionHandler.AcquirerUrl;

            // Retrieve acceptant's certificate
            if (!configurationSectionHandler.AcceptantCertificateFilename.IsNullEmptyOrWhiteSpace())
            {
                // Retrieve certificate from file
                if (configurationSectionHandler.AcceptantCertificatePassword.IsNullEmptyOrWhiteSpace())
                    throw new ConfigurationErrorsException("Password is required when acceptant's certificate is loaded from filesystem");

                AcceptantCertificate = GetCertificateFromFile(configurationSectionHandler.AcceptantCertificateFilename, configurationSectionHandler.AcceptantCertificatePassword);
            }
            else if (configurationSectionHandler.AcceptantCertificateStoreLocation != null)
            {
                // Retrieve certificate from certificate store
                if (configurationSectionHandler.AcceptantCertificateStoreName.IsNullEmptyOrWhiteSpace())
                    throw new ConfigurationErrorsException("Acceptant's certificate store name is required when loading certificate from the certificate store");

                if (configurationSectionHandler.AcceptantCertificateThumbprint.IsNullEmptyOrWhiteSpace())
                    throw new ConfigurationErrorsException("Acceptant's certificate thumbprint is required when loading certificate from the certificate store");
                
                AcceptantCertificate = GetCertificateFromStore(configurationSectionHandler.AcceptantCertificateStoreLocation.Value, configurationSectionHandler.AcceptantCertificateStoreName, configurationSectionHandler.AcceptantCertificateThumbprint);
            }
            else
            {
                // Neither filename nor store location is specified
                throw new ConfigurationErrorsException("You should either specify a filename or a certificate store location to specify the acceptant's certificate.");
            }

            // Retrieve acquirer's certificate
            if (!configurationSectionHandler.AcquirerCertificateFilename.IsNullEmptyOrWhiteSpace())
            {
                // Retrieve certificate from file
                AcquirerCertificate = GetCertificateFromFile(configurationSectionHandler.AcquirerCertificateFilename, null);
            }
            else if (configurationSectionHandler.AcquirerCertificateStoreLocation != null)
            {
                // Retrieve certificate from certificate store
                if (configurationSectionHandler.AcquirerCertificateStoreName.IsNullEmptyOrWhiteSpace())
                    throw new ConfigurationErrorsException("Acquirer's certificate store name is required when loading certificate from the certificate store");

                if (configurationSectionHandler.AcquirerCertificateThumbprint.IsNullEmptyOrWhiteSpace())
                    throw new ConfigurationErrorsException("Acquirer's certificate thumbprint is required when loading certificate from the certificate store");
                
                AcquirerCertificate = GetCertificateFromStore(configurationSectionHandler.AcquirerCertificateStoreLocation.Value, configurationSectionHandler.AcquirerCertificateStoreName, configurationSectionHandler.AcquirerCertificateThumbprint);
            }
            else
            {
                // Neither filename nor store location is specified
                throw new ConfigurationErrorsException("You should either specify a filename or a certificate store location to specify the acquirer's certificate.");
            }
        }

        private static X509Certificate2 GetCertificateFromFile(string relativePath, string password)
        {
            try
            {
                var absolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                return password != null ? new X509Certificate2(absolutePath, password, X509KeyStorageFlags.MachineKeySet) : new X509Certificate2(absolutePath);
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException("Could not load certificate file", exception);
            }
        }

        private static X509Certificate2 GetCertificateFromStore(StoreLocation storeLocation, string storeName, string thumbprint)
        {
            try
            {
                var certificateStore = new X509Store(storeName, storeLocation);
                certificateStore.Open(OpenFlags.OpenExistingOnly);
                
                foreach (var certificate in certificateStore.Certificates)
                {
                    if (certificate.Thumbprint.Trim().ToUpper() == thumbprint.Replace(" ", "").ToUpper())
                    {
                        return certificate;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException("Could not retrieve certificate from store " + storeName, exception);
            }
            
            throw new ConfigurationErrorsException("Certificate with thumbprint '" + thumbprint + "' not found");
        }
    }
}
