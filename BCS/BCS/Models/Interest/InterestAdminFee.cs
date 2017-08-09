using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models.Interest
{
    public class InterestAdminFee:IInterestAdminFee
    {
        public decimal Interest(string BillingType,int CompanyId, string BillingReference, string ZoneCode, DateTime CoverageFrom, DateTime BillDate, decimal amount)
        {
            bool hasValidPreviousBillingPeriod = true;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserName = HttpContext.Current.User.Identity.Name;
            var user = context.Users.Where(m => m.UserName == UserName).FirstOrDefault();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == user.Id).ZoneGroup;

            BCS_Context db = new BCS_Context();
            decimal interest = 0;
            var a = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId && m.TransactionType.ToUpper() == "BILLING" && m.BillingPeriod == db.SubsidiaryLedger.Where(k => k.CompanyId == CompanyId && k.BillingType == BillingType && k.TransactionType == "BILLING" && k.Other == ZoneCode).Max(j => j.BillingPeriod))
                .Select(x => new { x.BillingPeriod, x.BillingReference, x.TransactionReference }).FirstOrDefault();
            // will return the latest billing period and biling reference per company per billing reference

            var b = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId)
                .Where(m => m.BillingType == BillingType)
                .Where(m => m.TransactionType == "BILLING")
                .Where(m => m.Other == ZoneCode).ToList().Select(n => n.BillingPeriod).Distinct().ToList();
            //array of billing period per selected company and billing reference

            var latestBillingPeriod = db.BillingPeriod.Where(m => m.BillingPeriodId == a.BillingPeriod).FirstOrDefault();
            if(latestBillingPeriod.groupCode != ZoneGroup)
            {
                hasValidPreviousBillingPeriod = false;
            }

            

            //If balance detected => check if has previous billing
            //If previous billing has no balance => ( (Principal_From_Previous * 0.01) / n) * Days_Late)
            //If previous billing has balance => ( (Previous_Balance * 0.01) / n) * Days_Late) + ( (Principal_Amount * 0.01) / n) * Days_Late)

            if (b.Count() > 0 && hasValidPreviousBillingPeriod)
            {
                //Has previous billing

                var index = b.IndexOf(a.BillingPeriod);
                var previousBillingPeriod = b[index]; //get previous billing id

                //var previousBillingPeriodForBalances = 0;
                //if (b.Count == 1)
                //    previousBillingPeriodForBalances = 0;
                //else
                //    previousBillingPeriodForBalances = previousBillingPeriod;
                var previousBalance = 0.0M;

                try
                {
                    previousBalance = db.Balances //get the Balance amount from previous balance
                    .Where(l => l.CurrentBillingPeriod == previousBillingPeriod)
                    .Where(n => n.BillingType.ToUpper() == BillingType.ToUpper())
                    .Where(v => v.CompanyId == CompanyId)
                    .Where(k => k.TransactionType == "BILLING")
                    .Where(o => o.BillingSubType == "BALANCE") //Remove if Type in Balances will not be used.
                    .Where(p => p.BillingReference == ZoneCode)
                    .Where(c => c.TransactionReference == a.TransactionReference).SingleOrDefault().Amount;
                }
                catch (Exception)
                {
                }
                

                var previousPrincipalAmount = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(l => l.Other == ZoneCode)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "PRINCIPAL").SingleOrDefault().DebitAmount;

                var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DateTo;
                var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DueDate;

                int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                if (previousBalance > 0)
                    interest = ((previousPrincipalAmount * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString()) + ((amount * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString());
                else
                {
                    interest = (previousPrincipalAmount * 0.01M) / daysInAMonth;
                    interest *= decimal.Parse(totalDaysLate.ToString());
                }

            }
            else
            {
                //No previous billing = No interest
            }

            return interest + (interest * 0.12M); //12% is an add on by Sir Rufel 02-07-2017
        }
    }
}