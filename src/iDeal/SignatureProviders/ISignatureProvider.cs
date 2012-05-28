namespace iDeal.SignatureProviders
{
    public interface ISignatureProvider
    {
        /// <summary>
        /// Gets the digital signature used in each request send to the ideal api (stored in xml field tokenCode)
        /// </summary>
        /// <param name="messageDigest">Concatenation of designated fields from the request. Varies between types of request, consult iDeal Merchant Integratie Gids</param>
        string GetSignature(string messageDigest);

        /// <summary>
        /// Verifies the digital signature used in status responses from the ideal api (stored in xml field signature value)
        /// </summary>
        /// <param name="signature">Signature provided by ideal api, stored in signature value xml field</param>
        /// <param name="messageDigest">Concatenation of designated fields from the status response</param>
        bool VerifySignature(string signature, string messageDigest);

        /// <summary>
        /// Gets thumbprint of private certificate, used in each request to the ideal api (stored in field token)
        /// </summary>
        string GetThumbprintPrivateCertificate();

        /// <summary>
        /// Gets thumbprint of the public certificate, used in status response from ideal api
        /// </summary>
        string GetThumbprintPublicCertificate();
    }
}