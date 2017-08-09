using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class Balances
    {
        public int BalancesId { get; set; }
        [StringLength(200)]
        public string BillingType { get; set; }
        [StringLength(200)]
        public string TransactionType { get; set; }
        [StringLength(50)]
        public string TransactionReference { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Amount { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriodId { get; set; }
        public DateTime BillingGenerationDate { get; set; }
        [StringLength(50)]
        public string BillingReference { get; set; }
        [StringLength(510)]
        public string BillingSubType { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Interest { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal VAT { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? PrincipalRemaining { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? PrincipalWithInterest { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? InterestBasis { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? OriginalPrincipal { get; set; }
        [Range(0, 999999999999999999.99)]
        public DateTime? ComputeInterestFromDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public string CompCode { get; set; }
        public DateTime PeriodDateFrom { get; set; }
        public DateTime PeriodDateTo { get; set; }
        public DateTime DueDate { get; set; }
        public int CurrentBillingPeriod { get; set; }
        public string BalanceType { get; set; }
    }
}