using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models.Balance
{
    public class PrincipalBalanceAdminFee : IBalanceAdminFee
    {
        public decimal Balance(string BillingType, string BillingReference,string ZoneCode,int MaxBillingPeriod)
        {
            decimal balance = 0;
            BCS_Context db = new BCS_Context();
            var hasForwarded = db.Balances.Where(m => m.BillingType == BillingType)
                //.Where(m => m.TransactionType == "BILLING")
                .Where(m => m.BillingReference.ToUpper() == ZoneCode.ToUpper())
                .Where(m => m.CompCode.ToUpper() == BillingReference.ToUpper())
                .Where(m => m.BillingSubType == "BALANCE")
                .Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();

            if (hasForwarded.Count > 0)
            {
                var maxBalanceData = hasForwarded.Max(m => (int?)m.CurrentBillingPeriod) ?? 0;
                var balDr1 = db.Balances.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "BALANCE")
                    .Where(m => m.TransactionType == "BILLING")
                    .Where(m => m.BillingReference.ToUpper() == ZoneCode.ToUpper())
                    .Where(m => m.CurrentBillingPeriod == maxBalanceData)
                    .Where(m => m.CompCode.ToUpper() == BillingReference.ToUpper()).Sum(m => (decimal?)m.Amount) ?? 0;

                var balCr1 = 0;
                //= db.Balances.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "BALANCE")
                //    .Where(m => m.TransactionType == "CREDIT")
                //    .Where(m => m.BillingReference.ToUpper() == ZoneCode.ToUpper())
                //    .Where(m => m.CurrentBillingPeriod == maxBalanceData)
                //    .Where(m => m.CompCode.ToUpper() == BillingReference.ToUpper()).Sum(m => (decimal?)m.Amount) ?? 0;                

                var slDr1 = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "PRINCIPAL").Where(m => m.BillingReference.ToUpper() == BillingReference.ToUpper()).Where(m => m.Other == ZoneCode).Where(m => m.BillingPeriod == MaxBillingPeriod).Sum(m => (decimal?)m.DebitAmount) ?? 0;
                var slCr1 = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "PRINCIPAL").Where(m => m.BillingReference.ToUpper() == BillingReference.ToUpper()).Where(m => m.Other == ZoneCode).Where(m => m.BillingPeriod == MaxBillingPeriod).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var slCrWTAX = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.BillingSubType == "WTAX").Where(m => m.BillingReference.ToUpper() == BillingReference.ToUpper()).Where(m => m.Other == ZoneCode).Where(m => m.BillingPeriod == MaxBillingPeriod).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                slCr1 += slCrWTAX;

                bool isCancel = false;
                try
                { //If has Cancel OR Payment. Do not Subtract CR in SL
                  //Add this .Where(m => m.TransactionType.ToUpper() == "PAYMENT") and .Max(x => x.SubsidiaryLedgerId)
                    
                    var maxId = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType).Where(m => m.TransactionType.ToUpper() == "PAYMENT").Where(m => m.BillingSubType == "PRINCIPAL").Where(m => m.BillingReference.ToUpper() == BillingReference.ToUpper()).Where(m => m.Other == ZoneCode).Where(m => m.BillingPeriod == MaxBillingPeriod).Max(x => x.SubsidiaryLedgerId);

                    var referenceNumber = db.SubsidiaryLedger.Where(m => m.SubsidiaryLedgerId == maxId).Select(x => x.TransactionReference);
                    var cancelFlag = db.Database.SqlQuery<String>("select CancelFlag from GeneralCollection where ORNumber = '" + referenceNumber + "'").FirstOrDefault();
                    if (cancelFlag.ToUpper() == "YES")
                        isCancel = true;
                }
                catch (Exception)
                {

                }

                if (!isCancel)
                    balance = (balDr1 - (balCr1 * -1)) + (slDr1 - slCr1);
                else
                    balance = (balDr1 - (balCr1 * -1)) + (slDr1);
            }
            else
            {
                var balDr = db.Balances.Where(m => m.BillingType == BillingType)
                    .Where(m => m.BillingSubType == "BALANCE")
                    .Where(m => m.TransactionType == "BILLING")
                    .Where(m => m.BillingReference.ToUpper() == ZoneCode.ToUpper())
                    .Where(m => m.CompCode.ToUpper() == BillingReference.ToUpper()).Sum(m => (decimal?)m.Amount) ?? 0;

                var balCr = db.SubsidiaryLedger.Where(m => m.BillingType == BillingType)
                    .Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL")
                    .Where(m => m.TransactionType == "Payment")
                    .Where(m => m.Other.ToUpper() == ZoneCode.ToUpper())
                    .Where(m => m.BillingReference.ToUpper() == BillingReference.ToUpper()).Sum(m => (decimal?)m.CreditAmount) ?? 0;

              //  balance = Math.Abs(balDr - balCr);
                balance = balDr - balCr;

            }
            return balance;
        }
    }
}