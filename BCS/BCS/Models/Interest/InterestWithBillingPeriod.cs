using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models.Interest
{
    public class InterestWithBillingPeriod : IInterestWithBP
    {     
        public decimal Interest(string BillingType, int CompanyId, string billingReference, DateTime CoverageFrom, DateTime BillDate, decimal amount, string LastId, decimal currentPrincipalAmount)
        {
            BCS_Context db = new BCS_Context();
            decimal interest = 0;

            LatestBillingPeriodInfo latestBillingPeriodInfo = new LatestBillingPeriodInfo();
            var a = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId && m.BillingType.ToUpper() == BillingType.ToUpper() && m.TransactionType.ToUpper() == "BILLING" && m.BillingPeriod == db.SubsidiaryLedger.Where(k => k.CompanyId == CompanyId && k.BillingType == BillingType && k.TransactionType == "BILLING").Max(j => j.BillingPeriod))                
                .Select(x => new { x.BillingPeriod, x.BillingReference, x.TransactionReference }).FirstOrDefault();
            // will return the latest billing period and biling reference per company per billing reference

            var b = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId)
                .Where(m => m.TransactionType == "BILLING")
                .Where(m => m.BillingType == BillingType).ToList().Select(n => n.BillingPeriod).Distinct().ToList();
            //array of billing period per selected company and billing reference

            //If balance detected => check if has previous billing
            //If previous billing has no balance => ( (Principal_From_Previous * 0.01) / n) * Days_Late)
            //If previous billing has balance => ( (Previous_Balance * 0.01) / n) * Days_Late) + ( (Principal_Amount * 0.01) / n) * Days_Late)

            if (b.Count() > 0)
            {
                //Has previous billing

                var index = b.IndexOf(a.BillingPeriod);
                var previousBillingPeriod = b[index]; //get previous billing id

                //var previousBillingPeriodForBalances = 0;
                //if (b.Count == 1)
                //    previousBillingPeriodForBalances = 0;
                //else
                //    previousBillingPeriodForBalances = previousBillingPeriod;
                var existingBalance = 0.0M;
                var previousInterest = 0.0M;
                try
                {

                    existingBalance = db.Balances //get the Balance amount from previous balance
                   .Where(l => l.CurrentBillingPeriod == previousBillingPeriod)
                   .Where(n => n.BillingType.ToUpper() == BillingType.ToUpper())
                   .Where(v => v.CompanyId == CompanyId)
                   .Where(k => k.TransactionType == "BILLING")
                   .Where(o => o.BillingSubType == "BALANCE") //Remove if Type in Balances will not be used.
                   .FirstOrDefault().Amount;

                    previousInterest = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "INTEREST").FirstOrDefault().DebitAmount;

                }
                catch (Exception)
                {
                }

                var previousPrincipalAmount = 0.0M;

                
                    previousPrincipalAmount = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "PRINCIPAL").FirstOrDefault().DebitAmount;
                

                previousPrincipalAmount = previousPrincipalAmount > 0 ? previousPrincipalAmount : currentPrincipalAmount;

                var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DateTo;
                var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DueDate;

                int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                if (existingBalance > 0)
                    interest = ((previousPrincipalAmount * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString()) + (((amount + previousInterest) * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString());
                else
                {
                    interest = (amount * 0.01M) / daysInAMonth;
                    interest *= decimal.Parse(totalDaysLate.ToString());
                }

            }
            else
            {               
                int lastid = int.Parse(LastId);
                try
                {
                    if (BillingType.ToUpper() == "GARBAGE")
                    {                       
                        var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == lastid).SingleOrDefault().DateTo;
                        var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == lastid).SingleOrDefault().DueDate;

                        int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                        double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                        interest = (amount * 0.01M) / daysInAMonth;
                        interest *= decimal.Parse(totalDaysLate.ToString());
                    }
                }
                catch (Exception)
                {
                }
                //No previous billing = No interest
            }

            return interest + (interest * 0.12M); //12% is an add on by Sir Rufel 02-07-2017
        }
    }
}