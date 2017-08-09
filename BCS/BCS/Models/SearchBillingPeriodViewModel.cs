using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchBillingPeriodViewModel
    {
        public SearchBillingPeriodViewModel()
        {
            this.BillPeriodList = new List<BillingPeriod>();
        }
        public string ReturnedId { get; set; }
        public List<BillingPeriod> BillPeriodList = new List<BillingPeriod>();
    }
}