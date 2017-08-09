using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS
{
    public class AegingDAL
    {
        BCS_Context db = new BCS_Context();
        int CompanyId;
        string BillingType;
        public AegingDAL(int CompanyId,string BillingType)
        {
            this.CompanyId = CompanyId;
            this.BillingType = BillingType;
        }
        public double Dr
        {
            get
            {
                return db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == BillingType.ToUpper()).Where(m => m.TransactionType.ToUpper() == "BILLING").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.CompanyId == CompanyId).Sum(m => (double?)m.DebitAmount) ?? 0;
            }
        }
        public double Cr
        {
            get
            {
                var cancelPayments = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == BillingType.ToUpper()).Where(m=>m.TransactionType.ToUpper() == "PAYMENT").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.CompanyId == CompanyId).Sum(m => (double?)m.DebitAmount) ?? 0;
                var payments = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == BillingType.ToUpper()).Where(m => m.TransactionType.ToUpper() == "PAYMENT").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.CompanyId == CompanyId).Sum(m => (double?)m.CreditAmount) ?? 0;
                var wtax = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == BillingType.ToUpper()).Where(m => m.BillingSubType.ToUpper() == "WTAX").Where(m => m.CompanyId == CompanyId).Sum(m => (double?)m.CreditAmount) ?? 0;
                return (payments + wtax) - cancelPayments;
            }
        }
    }
}

