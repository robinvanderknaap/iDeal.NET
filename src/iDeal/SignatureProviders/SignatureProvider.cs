using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace iDeal.SignatureProviders
{
    public class SignatureProvider : ISignatureProvider
    {
        private readonly X509Certificate2 _privateCertificate;
        private readonly X509Certificate2 _publicCertificate;

        public SignatureProvider(X509Certificate2 privateCertificate, X509Certificate2 publicCertificate)
        {
            _privateCertificate = privateCertificate;
            _publicCertificate = publicCertificate;
        }
        
        /// <summary>
        /// Gets the digital signature used in each request send to the ideal api (stored in xml field tokenCode)
        /// </summary>
        /// <param name="messageDigest">Concatenation of designated fields from the request. Varies between types of request, consult iDeal Merchant Integratie Gids</param>
        public string GetSignature(string messageDigest)
        {
            // Step 1: Create a 160 bit message digest
            var hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(messageDigest));

            //Step 2: Sign with 1024 bits private key (RSA)
            var rsaCryptoServiceProvider = (RSACryptoServiceProvider)_privateCertificate.PrivateKey; // Create rsa crypto provider from private key contained in certificate, weirdest cast ever!
            var encryptedMessage = rsaCryptoServiceProvider.SignHash(hash, "SHA1");

            // Step 3: Base64 encode string for storage in xml request
            return Convert.ToBase64String(encryptedMessage);
        }

        /// <summary>
        /// Verifies the digital signature used in status responses from the ideal api (stored in xml field signature value)
        /// </summary>
        /// <param name="signature">Signature provided by ideal api, stored in signature value xml field</param>
        /// <param name="messageDigest">Concatenation of designated fields from the status response</param>
        public bool VerifySignature(string signature, string messageDigest)
        {
            // Step 1: Create a 160 bit message digest to compare with the one provided in the signature
            var hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(messageDigest));
            
            // Step 2: Base 64 deocde signature
            var decodedSignature = System.Convert.FromBase64String(signature);

            // Step 3: Verify signature with public key
            var rsaCryptoServiceProvider = (RSACryptoServiceProvider)_publicCertificate.PublicKey.Key;
            return rsaCryptoServiceProvider.VerifyHash(hash, "SHA1", decodedSignature);
        }

        /// <summary>
        /// Gets thumbprint of acceptant's certificate, used in each request to the ideal api (stored in field token)
        /// </summary>
        public string GetThumbprintAcceptantCertificate()
        {
            return _privateCertificate.Thumbprint;
        }

        /// <summary>
        /// Gets thumbprint of the acquirer's certificate, used in status response from ideal api
        /// </summary>
        public string GetThumbprintAcquirerCertificate()
        {
            return _publicCertificate.Thumbprint;
        }

        
    }
}
