using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    //Compute the interest
    public class InterestComputation
    {
        private int MonthDays = 0;
        private DateTime BillDate;
        private DateTime CoverageFrom;
        private int CompanyId;
        public decimal Balance;
        public decimal interest;
        public decimal balanceOfInterest;
        private string BillingReference;
        public decimal balanceOfVat;
        private string BillingType;
        private BCS_Context db = new BCS_Context();
        public InterestComputation(DateTime CoverageFrom, DateTime BillDate, int CompanyId) //Constructor for Billing period based services
        {
            this.MonthDays = DateTime.DaysInMonth(CoverageFrom.Year, CoverageFrom.Month);
            this.BillDate = BillDate;
            this.CoverageFrom = CoverageFrom;
            this.CompanyId = CompanyId;
        }

        public InterestComputation(DateTime CoverageFrom, DateTime BillDate, int CompanyId, string BillingReference) //Constructor for Billing months based services
        {
            this.MonthDays = DateTime.DaysInMonth(CoverageFrom.Year, CoverageFrom.Month);
            this.BillDate = BillDate;
            this.CoverageFrom = CoverageFrom;
            this.CompanyId = CompanyId;
            this.BillingReference = BillingReference;
        }

        

        public InterestComputation()
        {

        }

        private decimal servicesInterestByBillingPeriod(string BillingType, int CompanyId, string billingReference, DateTime CoverageFrom, DateTime BillDate, decimal amount)
        {
            decimal interest = 0;
            var a = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId && m.TransactionType.ToUpper() == "BILLING" && m.BillingPeriod == db.SubsidiaryLedger.Where(k => k.CompanyId == CompanyId && k.BillingType == BillingType && k.TransactionType == "BILLING").Max(j => j.BillingPeriod))
                .Select(x => new { x.BillingPeriod, x.BillingReference, x.TransactionReference }).FirstOrDefault();            
            // will return the latest billing period and biling reference per company per billing reference

            var b = db.SubsidiaryLedger
                .Where(m => m.CompanyId == CompanyId)
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

                var previousBalance = db.Balances //get the Balance amount from previous balance
                    .Where(l => l.CurrentBillingPeriod == previousBillingPeriod)
                    .Where(n => n.BillingType.ToUpper() == BillingType.ToUpper())
                    .Where(v => v.CompanyId == CompanyId)
                    .Where(k => k.TransactionType == "BILLING")
                    .Where(o => o.BillingSubType == "PRINCIPAL") //Remove if Type in Balances will not be used.
                    .Where(c => c.TransactionReference == a.TransactionReference).SingleOrDefault().Amount;

                var previousPrincipalAmount = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
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
        private decimal servicesInterest(string BillingType, int CompanyId, string billingReference, DateTime CoverageFrom, DateTime BillDate, decimal amount, decimal prevbalance)
        {
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

                var previousBalance = db.Balances.Where(m => m.BillingReference == billingReference) //get the Balance amount from previous balance
                    .Where(l => l.CurrentBillingPeriod == previousBillingPeriod)
                    .Where(n => n.BillingType.ToUpper() == BillingType.ToUpper())
                    .Where(v => v.CompanyId == CompanyId)
                    .Where(k => k.TransactionType == "BILLING")
                    .Where(o => o.BillingSubType == "PRINCIPAL") //Remove if Type in Balances will not be used.
                    .Where(c => c.TransactionReference == a.TransactionReference).SingleOrDefault().Amount;

                //var previousBalance = prevbalance;

                var previousPrincipalAmount = db.SubsidiaryLedger.Where(m => m.BillingPeriod == previousBillingPeriod)
                        .Where(q => q.CompanyId == CompanyId)
                        .Where(w => w.BillingReference == BillingReference)
                        .Where(e => e.TransactionType.ToUpper() == "BILLING")
                        .Where(t => t.BillingType.ToUpper() == BillingType.ToUpper())
                        .Where(r => r.BillingSubType.ToUpper() == "PRINCIPAL").SingleOrDefault().DebitAmount;

                var previousBillingMonth = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DateTo;
                var previousDueDate = db.BillingPeriod.Where(m => m.BillingPeriodId == previousBillingPeriod).SingleOrDefault().DueDate;

                int daysInAMonth = System.DateTime.DaysInMonth(previousBillingMonth.Year, previousBillingMonth.Month);
                double totalDaysLate = (BillDate.Date - previousDueDate.Value).TotalDays;

                if (previousBalance > 0)
                    interest = ((prevbalance * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString()) + ((amount * 0.01M) / daysInAMonth) * decimal.Parse(totalDaysLate.ToString());
                else
                {
                    interest = (prevbalance * 0.01M);
                    interest /= daysInAMonth;
                    interest *= Math.Abs(decimal.Parse(totalDaysLate.ToString()));
                }

            }
            else
            {
                //No previous billing = No interest
            }

            return interest + (interest * 0.12M); //12% is an add on by Sir Rufel 02-07-2017
        }

        private Decimal _Rental;
        public Decimal Rental
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;
                
                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "RENTAL").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _Rental = Debit - Credit;

                if (_Rental > 0)
                {
                    Balance = _Rental;
                    interest = servicesInterest("RENTAL", CompanyId, BillingReference, CoverageFrom, BillDate, Balance, _Rental);
                }
                //else
                //    _Rental = 0;

                return _Rental;
            }
        }

        private Decimal _PoleRental;
        public Decimal PoleRental
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _PoleRental = Debit - Credit;

                if (_PoleRental > 0)
                {
                    Balance = _PoleRental;
                    interest = servicesInterest("POLE RENTAL", CompanyId, BillingReference, CoverageFrom, BillDate, Balance, _PoleRental);
                }
                //else
                //    _PoleRental = 0;

                return _PoleRental;
            }
        }

        private Decimal _Garbage;
        public Decimal Garbage
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _Garbage = Debit - Credit;

                if (_Garbage > 0)
                {
                    Balance = _Garbage;
                    interest = servicesInterestByBillingPeriod("GARBAGE", CompanyId, BillingReference, CoverageFrom, BillDate, Balance);
                }
                //else
                //    _Garbage = 0;

                return _Garbage;
            }
        }

        private Decimal _Water;
        public Decimal Water
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "WATER").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _Water = Debit - Credit;

                if (_Water > 0)
                {
                    Balance = _Water;
                    interest = servicesInterestByBillingPeriod("WATER", CompanyId, BillingReference, CoverageFrom, BillDate, Balance);
                }
                //else
                //    _Water = 0;

                return _Water;
            }
        }

        private Decimal _Franchise;
        public Decimal Franchise
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _Franchise = Debit - Credit;

                if (_Franchise > 0)
                {
                    Balance = _Franchise;
                    interest = servicesInterest("FRANCHISE", CompanyId, BillingReference, CoverageFrom, BillDate, Balance, _Franchise);
                }
                //else
                //    _Franchise = 0;

                return _Franchise;
            }
        }

        private Decimal _PassedOnBilling;
        public Decimal PassedOnBilling
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.Other.ToUpper() == BillingReference.ToUpper().ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _PassedOnBilling = Debit - Credit;

                if (_PassedOnBilling > 0)
                {
                    Balance = _PassedOnBilling;
                    //interest = servicesInterest("PASSED ON BILLING", CompanyId, BillingReference, CoverageFrom, BillDate, Balance);
                }
                //else
                //    _PassedOnBilling = 0;

                return _PassedOnBilling;
            }
        }

        private Decimal _Sewerage;
        public Decimal Sewerage
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "SEWERAGE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _Sewerage = Debit - Credit;

                if (_Sewerage > 0)
                {
                    Balance = _Sewerage;
                    interest = servicesInterestByBillingPeriod("SEWERAGE", CompanyId, BillingReference, CoverageFrom, BillDate, Balance);
                }
                //else
                //    _Sewerage = 0;

                return _Sewerage;
            }
        }

        private decimal _AdminFee;
        public Decimal AdminFee
        {
            get
            {
                var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "PRINCIPAL").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var interestCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var interestDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "INTEREST").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                var VatCredit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.CreditAmount) ?? 0;
                var VatDebit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == "ADMIN FEE").Where(m => m.BillingSubType.ToUpper() == "VAT").Where(m => m.BillingReference == BillingReference.ToString()).Where(m => m.CompanyId == CompanyId).Sum(m => (decimal?)m.DebitAmount) ?? 0;

                balanceOfVat = VatDebit - VatCredit;
                balanceOfInterest = interestDebit - interestCredit;

                _AdminFee = Debit - Credit;

                if (_AdminFee > 0)
                {
                    Balance = _AdminFee;
                    interest = servicesInterestByBillingPeriod("ADMIN FEE", CompanyId, BillingReference, CoverageFrom, BillDate, Balance);
                }
                //else
                //    _AdminFee = 0;

                return _AdminFee;
            }
        }
    }
}