using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class Assessment
    {
        public int AssessmentId { get; set; }

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

        public string TransactionNo { get; set; }

        [Range(0, 999999999999999999.99)]

        public string ReferenceNo { get; set; }

        [Range(0, 999999999999999999.99)]

        public decimal DebitAmount { get; set; }

        [Range(0, 999999999999999999.99)]

        public decimal CreditAmount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string RateCode { get; set; }

        public string RateType { get; set; }

        public string TypeDesc { get; set; }

        public string AcctCode { get; set; }

        public string NGASCode { get; set; }

        public string BillPeriodMonth { get; set; }

        public string BillPeriodYear { get; set; }

        public string BillReferenceNo { get; set; }

        public string BillPeriodId { get; set; }

        public DateTime BillDate { get; set; }

        public DateTime BillGenerationDate { get; set; }

        public string BillingSubType { get; set; }
    }
}