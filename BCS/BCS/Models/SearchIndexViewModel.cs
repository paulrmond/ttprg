using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchIndexViewModel
    {
        public SearchIndexViewModel()
        {
            this.CompanyList = new List<Company>();
            this.UnpaidCompanyName = new List<string>();
            this.UnpaidCompanyAddress = new List<string>();
            this.UnpaidEnterpriseType = new List<string>();
            this.UnpaidBillingType = new List<string>();
            this.UnpaidDueDate = new List<DateTime>();
            this.UnpaidAmount = new List<decimal>();
            this.DueCompanyName = new List<string>();
            this.DueBillingType = new List<string>();
            this.DueEndDate = new List<DateTime>();
            this.DueDueDate = new List<DateTime>();
            this.DueAmount = new List<decimal>();
            this.DueDate = new List<DateTime>();
            this.DelinCompanyName = new List<string>();
            this.DelinBillingType = new List<string>();
            this.DelinEndDate = new List<DateTime>();
            this.DelinDueDate = new List<DateTime>();
            this.DelinAmount = new List<decimal>();
            this.GeneralBillingList = new List<GeneralBilling>();
            this.SubsidiaryLedger = new List<SubsidiaryLedger>();
            this.RentalInformationList = new List<RentalInformation>();
            this.PoleRentalInformationList = new List<PoleInformation>();
            this.WaterMeterAssignmentList = new List<WaterMeterAssignment>();
            this.PassedOnBillingInformationList = new List<PassedOnBillingInformation>();
            this.FranchiseFeeInformationList = new List<FranchiseFeeInformation>();
        }
        public List<Company> CompanyList { get; set; }
        public List<GeneralBilling> GeneralBillingList { get; set; }
        public List<SubsidiaryLedger> SubsidiaryLedger { get; set; }
        public List<RentalInformation> RentalInformationList { get; set; }
        public List<PoleInformation> PoleRentalInformationList { get; set; }
        public List<WaterMeterAssignment> WaterMeterAssignmentList { get; set; }
        public List<PassedOnBillingInformation> PassedOnBillingInformationList { get; set; }
        public List<FranchiseFeeInformation> FranchiseFeeInformationList { get; set; }
        public List<string> UnpaidCompanyName { get; set; }
        public List<string> UnpaidCompanyAddress { get; set; }
        public List<string> UnpaidEnterpriseType { get; set; }
        public List<DateTime> UnpaidDueDate { get; set; }
        public List<decimal> UnpaidAmount { get; set; }
        public List<string> UnpaidBillingType { get; set; }
        public List<string> DueCompanyName { get; set; }
        public List<string> DueBillingType { get; set; }
        public List<DateTime> DueEndDate { get; set; }
        public List<DateTime> DueDueDate { get; set; }
        public List<decimal> DueAmount { get; set; }
        public List<DateTime> DueDate { get; set; }
        public List<string> DelinCompanyName { get; set; }
        public List<string> DelinBillingType { get; set; }
        public List<DateTime> DelinEndDate { get; set; }
        public List<DateTime> DelinDueDate { get; set; }
        public List<decimal> DelinAmount { get; set; }
    }
}