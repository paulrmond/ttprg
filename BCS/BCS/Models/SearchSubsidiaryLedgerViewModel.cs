using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchSubsidiaryLedgerViewModel
    {
        public SearchSubsidiaryLedgerViewModel()
        {
            this.CompanyList = new List<Company>();
            this.SubsidiaryLedgerList = new List<SubsidiaryLedger>();
            this.SubsidiaryLedgerToList = new List<SubsidiaryLedger>();
            this.BillingRateList = new List<BillingRate>();
            this.BalanceList = new List<Balances>();
            this.Category = new List<String>();
            this.CategoryList = new List<String>();
            this.Credit = new List<Decimal>();
            this.Debit = new List<Decimal>();
            this.PreviousBalance = new List<Decimal>();
            this.CurrentBalance = new List<Decimal>();
            this.BillingType = new List<String>();
            this.ZoneList = new List<Zone>();
            this.ZoneGroupList = new List<ZoneGroup>();
			this.BillingPeriodList = new List<BillingPeriod>();
            this.BCSAgingOutput = new List<Int32>();
        }
        public string SearchInput { get; set; }
        public List<Decimal> Credit { get; set; }
        public List<Decimal> Debit { get; set; }
        public List<Decimal> CurrentBalance { get; set; }
        public List<Decimal> PreviousBalance { get; set; }
        public List<String> BillingType { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<SubsidiaryLedger> SubsidiaryLedgerList { get; set; }
        public List<SubsidiaryLedger> SubsidiaryLedgerToList { get; set; }
        public List<BillingRate> BillingRateList { get; set; }
        public List<Balances> BalanceList { get; set; }
        public List<String> Category { get; set; }
        public List<String> CategoryList { get; set; }

        public List<Zone> ZoneList { get; set; }
        public List<ZoneGroup> ZoneGroupList { get; set; }
        public List<BillingPeriod> BillingPeriodList { get; set; }
        public List<Int32> BCSAgingOutput { get; set; }
    }
}