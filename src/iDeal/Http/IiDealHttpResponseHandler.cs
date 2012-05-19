using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Http
{
    public interface IiDealHttpResponseHandler
    {
        iDealResponse HandleResponse(string response, ISignatureProvider signatureProvider);
    }
}
