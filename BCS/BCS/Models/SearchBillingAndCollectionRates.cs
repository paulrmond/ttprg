using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchBillingAndCollectionRates
    {
        public SearchBillingAndCollectionRates()
        {
            this.BillingRate = new List<BillingRate>();
            this.Category = new List<string>();
            this.SubCategory = new List<string>();
        }
        public string SearchInput { get; set; }
        public List<String> Category { get; set; }
        public List<string> SubCategory { get; set; }
        public List<BillingRate> BillingRate { get; set; }
    }
}