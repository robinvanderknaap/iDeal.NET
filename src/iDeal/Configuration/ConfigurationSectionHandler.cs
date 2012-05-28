using System;
using System.Configuration;

namespace iDeal.Configuration
{
    public class ConfigurationSectionHandler : ConfigurationSection, IConfigurationSectionHandler
    {
        public string MerchantId
        {
            get { return Merchant.Id; }
        }

        public int MerchantSubId
        {
            get { return Merchant.SubId; }
        }

        public string AcquirerUrl
        {
            get { return Aquirer.Url; }
        }

        public string PrivateCertificateName
        {
            get { return PrivateCertificate.Name; }
        }

        public string PrivateCertificateStoreName
        {
            get { return PrivateCertificate.StoreName; }
        }

        public string PrivateCertificateFilename
        {
            get { return PrivateCertificate.Filename; }
        }

        public string PrivateCertificatePassword
        {
            get { return PrivateCertificate.Password; }
        }

        public string PublicCertificateName
        {
            get { return PublicCertificate.Name; }
        }

        public string PublicCertificateStoreName
        {
            get { return PublicCertificate.StoreName; }
        }

        public string PublicCertificateFilename
        {
            get { return PublicCertificate.Filename; }
        }

        
        [ConfigurationProperty("merchant")]
        public MerchantElement Merchant
        {
            get
            {
                return (MerchantElement)this["merchant"];
            }
        }

        [ConfigurationProperty("acquirer")]
        public AcquirerElement Aquirer
        {
            get
            {
                return (AcquirerElement)this["acquirer"];
            }
        }

        [ConfigurationProperty("privateCertificate")]
        public PrivateCertificateElement PrivateCertificate
        {
            get
            {
                return (PrivateCertificateElement)this["privateCertificate"];
            }
        }

        [ConfigurationProperty("publicCertificate")]
        public PublicCertificateElement PublicCertificate
        {
            get
            {
                return (PublicCertificateElement)this["publicCertificate"];
            }
        }
    }

    public class MerchantElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public String Id
        {
            get
            {
                return (String)this["id"];
            }
        }

        [ConfigurationProperty("subId", IsRequired = false)]
        public int SubId
        {
            get { return (int) this["subId"]; }
        }
    }

    public class AcquirerElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public String Url
        {
            get
            {
                return (String)this["url"];
            }
        }
    }

    public class PrivateCertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
        }

        [ConfigurationProperty("storeName", IsRequired = false)]
        public String StoreName
        {
            get
            {
                return (String)this["storeName"];
            }
        }

        [ConfigurationProperty("filename", IsRequired = false)]
        public String Filename
        {
            get
            {
                return (String)this["filename"];
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public String Password
        {
            get
            {
                return (String)this["password"];
            }
        }
    }

    public class PublicCertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
        }

        [ConfigurationProperty("storeName", IsRequired = false)]
        public String StoreName
        {
            get
            {
                return (String)this["storeName"];
            }
        }

        [ConfigurationProperty("filename", IsRequired = false)]
        public String Filename
        {
            get
            {
                return (String)this["filename"];
            }
        }
    }
}