using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchCompany
    {
        public SearchCompany()
        {
            this.CompanyList = new List<Company>();
            this.RentalInformationList = new List<RentalInformation>();
            this.SubCategory = new List<string>();
            this.BillingRateList = new List<BillingRate>();
            this.Category = new List<string>();
            this.ZoneList = new List<Zone>();
        }
        public string SearchInput { get; set; }
        public List<RentalInformation> RentalInformationList { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<BillingRate> BillingRateList { get; set; }
        public List<BillingRate> BillingRate { get; set; }
        public List<String> SubCategory { get; set; }
        public List<String> Category { get; set; }
        public List<Zone> ZoneList { get; set; }
        public string LastIncSeries { get; set; }
        public string AutoCompanyCode { get; set; }
    }
}