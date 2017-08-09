using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models.Balance
{
    public class InterestBalancePOB : IBalance
    {
        public decimal Balance(string BillingType, string BillingReference, int CompanyId, int MaxBillingPeriod)
        {
            BCS_Context db = new BCS_Context();
            var balDr = db.Balances.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "INTEREST").Where(m => m.TransactionType == "BILLING").Where(m => m.BillingReference == BillingReference).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.Amount) ?? 0;
            var balCr = db.Balances.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "BALANCE").Where(m => m.TransactionType == "CREDIT").Where(m => m.BillingReference == BillingReference).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.Amount) ?? 0;

            var slDr = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "INTEREST").Where(m => m.Other == BillingReference).Where(m => m.CompanyId == CompanyId).Where(m => m.BillingPeriod == MaxBillingPeriod).Sum(m => (decimal?)m.DebitAmount) ?? 0;
            var slCr = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "INTEREST").Where(m => m.Other == BillingReference).Where(m => m.CompanyId == CompanyId).Where(m => m.BillingPeriod == MaxBillingPeriod).Sum(m => (decimal?)m.CreditAmount) ?? 0;

            decimal bal = (balDr - (balCr * -1)) + (slDr - slCr);

            return bal;
        }
    }
}