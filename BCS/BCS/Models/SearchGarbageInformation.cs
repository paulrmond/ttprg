using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchGarbageInformation
    {
        public SearchGarbageInformation()
        {
            this.Companies = new List<Company>();
            this.BillingPeriods = new List<BillingPeriod>();
            this.BillingRates = new List<BillingRate>();            
            this.GarbageInformations = new List<GarbageInformation>();
            this.SubCategory = new List<string>();
        }
       
        public string CompanyId { get; set; }
        public List<GarbageInformation> GarbageInformations = new List<GarbageInformation>();
        public List<Company> Companies = new List<Company>();
        public List<BillingRate> BillingRates = new List<BillingRate>();
        public List<BillingPeriod> BillingPeriods = new List<BillingPeriod>();
        public List<String> SubCategory = new List<string>();
    }
}