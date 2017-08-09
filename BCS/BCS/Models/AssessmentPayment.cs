using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class AssessmentPayment
    {
        public int AssessmentPaymentId { get; set; }
        public string REF_NO { get; set; }
        public string COMP_CODE { get; set; }
        public string RATE_TYPE { get; set; }
        public string ZONE_CODE { get; set; }
        public string BILL_PERIOD_MONTH { get; set; }
        public string BILL_PERIOD_YEAR { get; set; }
        public string BILL_GENERATION_DATE { get; set; }
        public string OR_NUM { get; set; }
        public string OR_DATE { get; set; }
        public string CHECK_AMOUNT { get; set; }
        public string CASH_AMOUNT { get; set; }
        public string CHECK_NO { get; set; }
        public string CHECK_BRANCH { get; set; }
        public string CHECK_DATE { get; set; }
        public string TOTAL_PAYMENT { get; set; }
        public string PRINCIPAL_PAYMENT { get; set; }
        public string INTEREST_PAYMENT { get; set; }
        public string ASSESSMENT_TYPE { get; set; }
        public string BILL_COVERAGE_DATE_FROM { get; set; }
        public string BILL_COVERAGE_DATE_TO { get; set; }
        public string RATE_CODE { get; set; }
        public string ACCT_CODE { get; set; }
        public string CURRENT_BILLING_PERIOD { get; set; }
        public string COMPANY_ID { get; set; }

    }
}