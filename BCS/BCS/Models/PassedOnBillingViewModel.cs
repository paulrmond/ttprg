using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class PassedOnBillingViewModel
    {

        public PassedOnBillingViewModel()
        {
            this.CompanyList = new List<Company>();
            this.BillingPeriodList = new List<BillingPeriod>();
            this.PassedOnBillingList = new List<PassedOnBillingInformation>();
            this.ZoneList = new List<Zone>();
        }
        public string SearchInput { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<PassedOnBillingInformation> PassedOnBillingList { get; set; }
        public List<BillingPeriod> BillingPeriodList { get; set; }
        public List<Zone> ZoneList { get; set; }
    }
}