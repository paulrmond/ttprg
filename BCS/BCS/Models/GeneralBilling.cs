using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class GeneralBilling
    {
        public int GeneralBillingId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingNumber { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriod { get; set; }
        public DateTime BillingDate { get; set; }
        public DateTime DueDate { get; set; }
        [StringLength(150)]
        public string BillingType { get; set; }
        [StringLength(150)]
        public string TransactionType { get; set; }
        [StringLength(150)]
        public string BillingReference { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal BillingAmount { get; set; }
        public DateTime GenerationDate { get; set; }
        public DateTime CoverageFrom { get; set; }
        public DateTime CoverageTo { get; set; }
        [StringLength(150)]
        public string Currency { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; }
    }
}