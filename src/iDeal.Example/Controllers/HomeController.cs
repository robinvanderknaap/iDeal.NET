using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iDeal.Example.Models;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web.Caching;
using iDeal.Directory;

namespace iDeal.Example.Controllers
{
    public class HomeController : Controller
    {
        private iDealService _iDealService = new iDealService();

        public HomeController()
        {
            // Allow invalid ssl certificates, needed to connect with www.ideal-simulator.nl, which often has an expired certificate
            // You should never allow for a invalid ssl certificate in a production environment!
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;                       
        }

        [HttpGet]
        public ActionResult Index()
        {
            // Create new payment viewmodel and load issuers with ideal service.
            // Normally the list of issuers should be cached and refreshed once a day
            var paymentViewModel = new PaymentViewModel();
            paymentViewModel.SetIssuers(_iDealService.SendDirectoryRequest().Issuers);

            return View(paymentViewModel);
        }

        [HttpPost]
        public ActionResult Index(PaymentViewModel paymentViewModel)
        {
            if (ModelState.IsValid)
            {
                // Send transaction request to selected issuer
                var transactionResponse = _iDealService.SendTransactionRequest(
                    issuerId: paymentViewModel.Issuer, 
                    merchantReturnUrl: Url.Action("Status", "Home", null, Request.Url.Scheme, null), 
                    purchaseId: Guid.NewGuid().ToString().Substring(0, 16), 
                    amount: (int)paymentViewModel.Amount * 100, 
                    expirationPeriod: TimeSpan.FromMinutes(5), 
                    description: "Test payment",
                    entranceCode: Guid.NewGuid().ToString().Substring(0, 16));

                // Redirect user to transaction page of issuer
                Response.Redirect(transactionResponse.IssuerAuthenticationUrl);
            }

            paymentViewModel.SetIssuers(_iDealService.SendDirectoryRequest().Issuers);

            return View(paymentViewModel);
        }

        [HttpGet]
        public ActionResult Status(string trxid, string ec)
        {
            // Retrieve status of specified transaction
            var statusResponse = _iDealService.SendStatusRequest(trxid);

            return View(statusResponse);
        }

        // Callback used to validate the certificate in an SSL conversation
        protected static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}
