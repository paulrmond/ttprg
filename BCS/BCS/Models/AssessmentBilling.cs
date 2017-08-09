using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class AssessmentBilling
    {
        public int AssessmentBillingId { get; set; }
        public string REF_NO { get; set; }
        public string TRANS_NO { get; set; }
        public string COMP_CODE { get; set; }
        public string TRANS_DATE { get; set; }
        public string RATE_CODE { get; set; }
        public string RATE_TYPE { get; set; }
        public string TYPE_DESC { get; set; }
        public string ACCT_CODE { get; set; }
        public string NGAS_CODE { get; set; }
        public string DUE_DATE { get; set; }
        public string ZONE_CODE { get; set; }
        public string AMOUNT_DUE { get; set; }
        public string BILL_PERIOD_MONTH { get; set; }
        public string BILL_PERIOD_YEAR { get; set; }
        public string BILL_REFERENCE_NO { get; set; }
        public string BILL_PERIOD_ID { get; set; }
        public string BILL_DATE { get; set; }
        public string BILL_GENERATION_DATE { get; set; }
        public string CURRENT_BILLING_PERIOD { get; set; }
        public string COMPANY_ID { get; set; }
    }
}