using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Http
{
    public interface IiDealHttpRequest
    {
        iDealResponse SendRequest(iDealRequest idealRequest, ISignatureProvider signatureProvider, string url, IiDealHttpResponseHandler iDealHttpResponseHandler);
    }
}
