using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using iDeal.Directory;

namespace iDeal.Example.Models
{
    public class PaymentViewModel
    {
        public decimal Amount
        {
            get { return 500; }
        }

        [Required(ErrorMessage="* required")]
        public int Issuer { get; set; }

        public IEnumerable<SelectListItem> Issuers { get; private set; }

        public void SetIssuers(IList<Issuer> issuers)
        {
            Issuers = issuers.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
        }
    }
}