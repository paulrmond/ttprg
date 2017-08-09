using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchPoleInformationViewModel
    {
        public SearchPoleInformationViewModel()
        {
            this.CompanyList = new List<Company>();
            this.PoleInformationList = new List<PoleInformation>();
        }
        public string SearchInput { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<PoleInformation> PoleInformationList { get; set; }
    }
}