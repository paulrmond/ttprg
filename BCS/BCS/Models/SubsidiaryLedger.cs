using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class SubsidiaryLedger
    {
        public int SubsidiaryLedgerId { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriod { get; set; }
        public DateTime BillingDate { get; set; }
        public DateTime DueDate { get; set; }
        [StringLength(50)]
        public string BillingType { get; set; }
        [StringLength(50)]
        public string TransactionType { get; set; }
        [StringLength(50)]
        public string BillingReference { get; set; }
        [StringLength(50)]
        public string TransactionReference { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal DebitAmount { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal CreditAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? DollarAmount { get; set; }
        [StringLength(30)]
        public string Currency { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(50)]
        public string BillingSubType { get; set; }
        public decimal? FXRate { get; set; }
        public decimal? Peso { get; set; }
        public string Other { get; set; }
        public string ATC { get; set; }
        public string ATCRates { get; set; }
    }
}