using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Controllers
{
    public class InterestModel
    {
        private int MonthDays = 0;
        private DateTime GenerationDate;
        private DateTime DueDate;
        private int CompanyId;
        public decimal Balance;
        public decimal interest;
        private BCS_Context db = new BCS_Context();
        public InterestModel(DateTime CoverageFrom,DateTime GenerationDate,DateTime DueDate,int CompanyId)
        {
            this.MonthDays = DateTime.DaysInMonth(CoverageFrom.Year, CoverageFrom.Month);
            this.GenerationDate = GenerationDate;
            this.DueDate = DueDate;
            this.CompanyId = CompanyId;
        }

        public InterestModel()
        {

        }                       

        private Decimal _Rental;
        public Decimal Rental
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Rental' and TransactionType = 'BILLING' and CompanyId = '"+ CompanyId +"'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();

                _Rental = Credit - Debit;

                if (_Rental > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _Rental;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _Rental = ((_Rental * 0.01M) / MonthDays) * daysLate;
                    interest = _Rental;
                }
                else
                    _Rental = 0;

                return _Rental;
            }
        }

        private Decimal _PoleRental;
        public Decimal PoleRental
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Pole Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Pole Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Pole Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Pole Rental' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _PoleRental = Credit - Debit;

                if (_PoleRental > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _PoleRental;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _PoleRental = ((_PoleRental * 0.01M) / MonthDays) * daysLate;
                    interest = _PoleRental;
                }
                else
                    _PoleRental = 0;

                return _PoleRental;
            }
        }

        private Decimal _Garbage;
        public Decimal Garbage
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _Garbage = Credit - Debit;

                if (_Garbage > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _Garbage;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _Garbage = ((_Garbage * 0.01M) / MonthDays) * daysLate;
                    interest = _Garbage;
                }
                else
                    _Garbage = 0;

                return _Garbage;
            }
        }

        private Decimal _Water;
        public Decimal Water
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Garbage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _Water = Credit - Debit;

                if (_Water > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _Water;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _Water = ((_Water * 0.01M) / MonthDays) * daysLate;
                    interest = _Water;
                }
                else
                    _Water = 0;

                return _Water;
            }
        }

        private Decimal _Franchise;
        public Decimal Franchise
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Franchise' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Franchise' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Franchise' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Franchise' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _Franchise = Credit - Debit;

                if (_Franchise > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _Franchise;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _Franchise = ((_Franchise * 0.01M) / MonthDays) * daysLate;
                    interest = _Franchise;
                }
                else
                    _Franchise = 0;

                return _Franchise;
            }
        }

        private Decimal _SecurityGuard;
        public Decimal SecurityGuard
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Security Guard' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Security Guard' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Security Guard' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Security Guard' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _SecurityGuard = Credit - Debit;

                if (_SecurityGuard > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _SecurityGuard;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _SecurityGuard = ((_SecurityGuard * 0.01M) / MonthDays) * daysLate;
                    interest = _SecurityGuard;
                }
                else
                    _SecurityGuard = 0;

                return _SecurityGuard;
            }
        }

        private Decimal _Sewerage;
        public Decimal Sewerage
        {
            get
            {
                var Credit = db.Database.ExecuteSqlCommand("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Sewerage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                var Debit = db.Database.ExecuteSqlCommand("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Sewerage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'");
                //var Credit = db.Database.SqlQuery<decimal>("select sum(CreditAmount) as a from SubsidiaryLedgers where BillingType = 'Sewerage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                //var Debit = db.Database.SqlQuery<decimal>("select sum(DebitAmount) as a from SubsidiaryLedgers where BillingType = 'Sewerage' and TransactionType = 'BILLING' and CompanyId = '" + CompanyId + "'").Single();
                _Sewerage = Credit - Debit;

                if (_Sewerage > 0)
                {
                    //DateTime newDueDate = new DateTime(GenerationDate.Year, GenerationDate.AddMonths(-1).Month, DueDate);
                    Balance = _Sewerage;
                    double DaysLate = (GenerationDate.Date - DueDate.AddMonths(-1).Date).TotalDays;
                    decimal daysLate = Convert.ToDecimal(DaysLate);
                    _Sewerage = ((_Sewerage * 0.01M) / MonthDays) * daysLate;
                    interest = _Sewerage;
                }
                else
                    _Sewerage = 0;

                return _Sewerage;
            }
        }
    }
}