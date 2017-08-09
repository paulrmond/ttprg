using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models.Interest
{
    public class InterestWithoutBillingPeriod : IIneterestNoBP
    {
        public decimal Interest(string BillingType, int CompanyId, string billingReference, DateTime CoverageFrom, DateTime BillDate, decimal amount, decimal prevbalance,string lastId, decimal currentPrincipalAmount)
        {
            BCS_Context db = new BCS_Context();
            //AS OF NOW COMPUTATION OF BILLING INTEREST IS FIXED TO PER MONTH ///////////////////////////////////////////////////////////////////////////////
            decimal interest = 0;

            //var maxbillp = db.SubsidiaryLedger.Where(k => k.CompanyId == CompanyId && k.BillingType == BillingType && k.TransactionType == "BILLING").Max(j => j.BillingPeriod);
            //var ab = db.SubsidiaryLedger.Where(m => m.CompanyId == CompanyId).Where(j=>j.TransactionType.ToUpper() == "BILLING").Where(h=> h.BillingPeriod == maxbillp).Where(l => l.BillingReference == billingReference).FirstOrDefault();


            var a = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId && m.TransactionType.ToUpper() == "BILLING" && m.BillingPeriod == db.SubsidiaryLedger.Where(k => k.CompanyId == CompanyId && k.BillingType == BillingType && k.TransactionType == "BILLING").Max(j => j.BillingPeriod))
                .Where(l => l.BillingReference == billingReference)
                .Select(x => new { x.BillingPeriod, x.BillingReference, x.TransactionReference }).FirstOrDefault();
            // will return the latest billing period and biling reference per company per billing reference

            //var countBalances = db.Balances.Where(m => m.CompanyId == CompanyId && m.BillingReference == billingReference && m.BillingType.ToUpper() == BillingType.ToUpper() && m.TransactionReference == a.TransactionReference).ToList();
            //if countBalances == 0 then no interest.; The billing period of 1st billing generation is 0.; Changes made 12/27/2016

            var b = db.SubsidiaryLedger
                    .Where(m => m.CompanyId == CompanyId)
                    .Where(m => m.BillingType == BillingType)
                    .Where(m => m.TransactionType == "BILLING")
                    .Where(l => l.BillingReference == billingReference).ToList().Select(n => n.BillingPeriod).Distinct().ToList();
            //array of billing period per selected company and billing reference

            //If balance detected => check if has previous billing
            //If previous billing has no balance => ( (Principal_From_Previous * 0.01) / n) * Days_Late)
            //If previous billing has balance => ( (Previous_Balance * 0.01) / n) * Days_Late) + ( (Principal_Amount * 0.01) / n) * Days_Late)

            if (b.Count() > 0)
            {
                //Has previous billing

                var index = b.IndexOf(a.BillingPeriod);
                var previousBillingPeriod = b[index]; //get previous billing id
                                                      //var billp = b[b.Count - 1];

                //var previousBillingPeriodForBalances = 0;
                //if (b.Count == 1)
                //    previousBillingPeriodForBalances = 0;
                //else
                //    previousBillingPeriodForBalances = previousBillingPeriod;
                var existingBalance = 0.0M;
                var previousInterest = 0.0M;
                try
                {
                    existingBalance = db.Balances.Where(m => m.BillingReference == billingReference) //get the Balance amount from previous balance
                    .Where(l => l.CurrentBillingPeriod == previousBillingPeriod)
                    .Where(n => n.BillingType.ToUpper() == BillingType.ToUpper())
                    .Where(v => v.CompanyId == CompanyId)
                    .Where(k => k.TransactionType == "BILLING")
                    .Where(o => o.BillingSubType == "BALANCE") //Remove if Type in Balances will not be used.
                    .Where(c => c.TransactionReference == a.TransactionReference).FirstOrDefault().Amount;

                    previousInterest = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(w => w.BillingReference == billingReference)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "INTEREST").FirstOrDefault().DebitAmount;
                }
                catch (Exception)
                {
                }
                

                //var previousBalance = prevbalance;

                var previousPrincipalAmount = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(w => w.BillingReference == billingReference)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "PRINCIPAL").FirstOrDefault().DebitAmount;

                previousPrincipalAmount = previousPrincipalAmount > 0 ? previousPrincipalAmount : currentPrincipalAmount;

                var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).FirstOrDefault().DateTo;
                var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).FirstOrDefault().DueDate;

                int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                if (existingBalance > 0)
                    interest = ((prevbalance * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString()) + (((amount + previousInterest) * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString());
                else
                {
                    interest = (amount * 0.01M);
                    interest /= daysInAMonth;
                    interest *= Math.Abs(decimal.Parse(totalDaysLate.ToString()));
                }
            
            }
            else
            {
                int LastId = int.Parse(lastId);
                try
                {                    
                    var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == LastId).FirstOrDefault().DateTo;
                    var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == LastId).FirstOrDefault().DueDate;

                    int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                    double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                    interest = (amount * 0.01M);
                    interest /= daysInAMonth;
                    interest *= Math.Abs(decimal.Parse(totalDaysLate.ToString()));
                }
                catch (Exception)
                {
                }
            }

            return interest + (interest * 0.12M); //12% is an add on by Sir Rufel 02-07-2017
        }
    }
}