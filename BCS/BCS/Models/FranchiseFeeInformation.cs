using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public interface IFranchiseFeeInformation
    {
         int CompanyId { get; set; }
         decimal Amount { get; set; }
         string BillMode { get; set; }
         int DueOn { get; set; }
         DateTime StartDate { get; set; }
         DateTime EndDate { get; set; }
         string BillingMonths { get; set; }
    }
    public class FranchiseFeeInformation:IFranchiseFeeInformation
    {
        public int FranchiseFeeInformationId { get; set; }
        [Range(0,999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Amount { get; set; }
        [StringLength(250)]
        public string BillMode { get; set; }
        [Range(0, 999999999999999999)]
        public int DueOn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(30)]
        public string BillingMonths { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(50)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(150)]
        public string BillingStatus { get; set; }
        public string CompCode { get; set; }
        public int OldFranchiseFeeInformationId { get; set; }
    }
}