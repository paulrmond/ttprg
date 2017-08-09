using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchOPAccountsViewModel
    {
        public SearchOPAccountsViewModel()
        {
            this.OPAccountList = new List<OPAccount>();
            this.OPASubItemsList = new List<OPASubItems>();
            this.DivisionList = new List<Division>();
        }
        public string ReturnedId { get; set; }
        public string SearchInput { get; set; }
        public List<OPAccount> OPAccountList = new List<OPAccount>();
        public List<OPASubItems> OPASubItemsList = new List<OPASubItems>();
        public List<Division> DivisionList = new List<Division>();
    }
}