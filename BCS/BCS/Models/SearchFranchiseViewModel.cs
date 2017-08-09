using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchFranchiseViewModel
    {
        public SearchFranchiseViewModel()
        {
            this.Companies = new List<Company>();
            this.FranchiseFeeInformations = new List<FranchiseFeeInformation>();
        }
        public int CompanyId;
        public List<Company> Companies = new List<Company>();
        public List<FranchiseFeeInformation> FranchiseFeeInformations = new List<FranchiseFeeInformation>();
    }
}