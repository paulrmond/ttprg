using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using BCS.Models;

namespace BCS.Models
{
    public static class ClassAssignment
    {
        public static void AssignGeneralLedger(GeneralBilling baseValue, GeneralBilling assignValue)
        {
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.GenerationDate = baseValue.GenerationDate;
            assignValue.CoverageFrom = baseValue.CoverageFrom;
            assignValue.CoverageTo = baseValue.CoverageTo;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.UpdateDate = baseValue.UpdateDate;
            assignValue.Status = baseValue.Status;
        }

        public static void AssignSubsidiaryLedger(SubsidiaryLedger baseValue, SubsidiaryLedger assignValue)
        {
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.DebitAmount = baseValue.DebitAmount;
            assignValue.TransactionDate = baseValue.TransactionDate;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.UpdateDate = baseValue.UpdateDate;
            assignValue.BillingSubType = baseValue.BillingSubType;
            assignValue.Other = baseValue.Other;
        }

        public static void AssignInterest(SubsidiaryLedger baseValue, SubsidiaryLedger assignValue)
        {
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.DebitAmount = baseValue.DebitAmount;
            assignValue.TransactionDate = baseValue.TransactionDate;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.BillingSubType = "INTEREST";

            assignValue.BillingType = baseValue.BillingType;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.Currency = baseValue.Currency;
            assignValue.DebitAmount = baseValue.DebitAmount;
            assignValue.DollarAmount = baseValue.DollarAmount;
            assignValue.Other = baseValue.Other;
        }

        public static void AssignVat(SubsidiaryLedger baseValue, SubsidiaryLedger assignValue)
        {
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.DebitAmount = baseValue.DebitAmount;
            assignValue.TransactionDate = baseValue.TransactionDate;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.BillingSubType = "VAT";

            assignValue.BillingType = baseValue.BillingType;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.Currency = baseValue.Currency;
            assignValue.DebitAmount = baseValue.DebitAmount * 0.12M;
            assignValue.DollarAmount = baseValue.DollarAmount;
            assignValue.UpdateDate = baseValue.UpdateDate;
            assignValue.Other = baseValue.Other;
        }

        public static void AssignBalanceCompleteVat(Balances baseValue, Balances assignValue)
        {
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.Amount = baseValue.VAT;
            assignValue.BillingSubType = "VAT";
            assignValue.BillingPeriodId = baseValue.BillingPeriodId;
            assignValue.BillingGenerationDate = baseValue.BillingGenerationDate;
            assignValue.BillingReference = baseValue.BillingReference;
            //assignValue.BillingSubType = baseValue.BillingSubType;
            assignValue.Interest = 0;
            assignValue.VAT = 0;
            assignValue.PrincipalRemaining = baseValue.PrincipalRemaining;
            assignValue.InterestBasis = baseValue.InterestBasis;
            assignValue.OriginalPrincipal = baseValue.OriginalPrincipal;
            assignValue.ComputeInterestFromDate = baseValue.ComputeInterestFromDate;
            assignValue.LastPaymentDate = baseValue.LastPaymentDate;
            assignValue.CompCode = baseValue.CompCode;
            assignValue.PeriodDateFrom = baseValue.PeriodDateFrom;
            assignValue.PeriodDateTo = baseValue.PeriodDateTo;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.CurrentBillingPeriod = baseValue.CurrentBillingPeriod;
            assignValue.BalanceType = "FORWARDED BALANCE";
        }

        public static void AssignBalanceCompleteInterest(Balances baseValue, Balances assignValue)
        {
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.Amount = Math.Abs(baseValue.Interest);
            assignValue.BillingSubType = "INTEREST";
            assignValue.BillingPeriodId = baseValue.BillingPeriodId;
            assignValue.BillingGenerationDate = baseValue.BillingGenerationDate;
            assignValue.BillingReference = baseValue.BillingReference;
            //assignValue.BillingSubType = baseValue.BillingSubType;
            assignValue.Interest = 0;
            assignValue.VAT = 0;
            assignValue.PrincipalRemaining = baseValue.PrincipalRemaining;
            assignValue.InterestBasis = baseValue.InterestBasis;
            assignValue.OriginalPrincipal = baseValue.OriginalPrincipal;
            assignValue.ComputeInterestFromDate = baseValue.ComputeInterestFromDate;
            assignValue.LastPaymentDate = baseValue.LastPaymentDate;
            assignValue.CompCode = baseValue.CompCode;
            assignValue.PeriodDateFrom = baseValue.PeriodDateFrom;
            assignValue.PeriodDateTo = baseValue.PeriodDateTo;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.CurrentBillingPeriod = baseValue.CurrentBillingPeriod;
            assignValue.BalanceType = "FORWARDED BALANCE";
        }

        public static void AssignBalance(SubsidiaryLedger baseValue, Balances assignValue, decimal balance, decimal interest, decimal vat, string lastid)
        {
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionReference = baseValue.TransactionReference;
            //assignValue.BillingSubType = "BALANCE";
            assignValue.BillingSubType = "PRINCIPAL";
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.TransactionType = baseValue.TransactionType;

            int _lastid = 0;
            if (lastid != null)
            {
                try
                {
                    BCS_Context db = new BCS_Context();
                    ApplicationDbContext context = new ApplicationDbContext();
                    var uname = HttpContext.Current.User.Identity.Name;
                    var zonegroup = context.Users.FirstOrDefault(m => m.UserName == uname).ZoneGroup;
                    
                    _lastid = db.BillingPeriod.FirstOrDefault(m=>m.groupCode == zonegroup).BillingPeriodId;
                }
                catch (Exception ex)
                {                    
                    _lastid = 0;
                }
            }


            assignValue.DueDate = baseValue.DueDate;
            assignValue.VAT = Math.Abs(vat);
            assignValue.Amount = baseValue.DebitAmount;
            assignValue.BillingPeriodId = _lastid;
            assignValue.BillingGenerationDate = DateTime.Now;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.CurrentBillingPeriod = baseValue.BillingPeriod;
            assignValue.Interest = Math.Abs(interest);
            assignValue.BalanceType = "FORWARDED BALANCE";
        }

        public static void AssignBalanceAdvance(SubsidiaryLedger baseValue, Balances assignValue, decimal balance, decimal interest, decimal vat, string lastid,DateTime billingPeriodDateFrom,DateTime billingPeriodDateTo)
        {
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.BillingSubType = "BALANCE";
            assignValue.CompanyId = baseValue.CompanyId;

            if (balance < 0)
            {
                assignValue.TransactionType = "CREDIT";
                balance = Math.Abs(balance);
            }
                
            else
                assignValue.TransactionType = baseValue.TransactionType;
            int _lastid = 0;
            if (lastid != null)
            {
                try
                {
                    _lastid = int.Parse(lastid);
                }
                catch (Exception ex)
                {
                    _lastid = 0;
                }
            }
            assignValue.DueDate = baseValue.DueDate;
            assignValue.VAT = Math.Abs(vat);
            assignValue.Amount = Math.Abs(balance);
            assignValue.BillingPeriodId = _lastid;
            assignValue.BillingGenerationDate = DateTime.Now;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.CurrentBillingPeriod = baseValue.BillingPeriod;
            assignValue.Interest = Math.Abs(interest);
            assignValue.PeriodDateFrom = billingPeriodDateFrom;
            assignValue.PeriodDateTo = billingPeriodDateTo;
            assignValue.BalanceType = "FORWARDED BALANCE";
        }

        

        public static decimal? GetWaterAmount(decimal WaterConsumption, string userid)
        {
            BCS_Context db = new BCS_Context();
            ApplicationDbContext context = new ApplicationDbContext();

            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            decimal? waterAmount = 0;
            List<BillingRate> BillingRates = new List<BillingRate>();
            BillingRates = db.BillingRates.Where(m => m.Category.ToUpper().Trim() == "WATER").Where(m => m.ZoneGroup == ZoneGroup).ToList();
            int i = BillingRates.Count;
            BillingRate tempBillRate = new BillingRate();
            List<BillingRate> tempBillingRates = new List<BillingRate>();
            bool isTrue = true;
            bool isSorted = false;

            while (isTrue)
            {
                isTrue = false;
                isSorted = true;
                for (int j = 0; j < i - 1; j++)
                {

                    string[] first = BillingRates[j].SubCategory.Split('-');
                    string[] sec = BillingRates[j + 1].SubCategory.Split('-');
                    double dblFirst = Convert.ToDouble(first[0]);
                    double dblSec = Convert.ToDouble(sec[0]);

                    if (dblFirst > dblSec)
                    {
                        tempBillRate = BillingRates[j + 1];
                        BillingRates[j + 1] = BillingRates[j];
                        BillingRates[j] = tempBillRate;

                        isTrue = true;
                        isSorted = false;
                    }
                }
            }

            if (isSorted)
            {
                decimal? AddToTotal = BillingRates[0].Rate; //If belong to first consumption range. save this value.
                //double PrevRange = 0;
                decimal AddToAmount = 0;
                for (int j = 0; j < i; j++)
                {
                    string[] SubcatRange = BillingRates[j].SubCategory.Split('-');
                    decimal StartRange = decimal.Parse(SubcatRange[0]);
                    decimal EndRange = decimal.Parse(SubcatRange[1]);
                    decimal? Rate = BillingRates[j].Rate;
                    AddToAmount += EndRange; //accumulated "end cu. m".

                    if (WaterConsumption >= StartRange && WaterConsumption <= EndRange || j == (i - 1))
                    {
                        if (j > 0) //If not within the range of first consumption range.
                        {
                            decimal newWaterConsumption = Convert.ToDecimal(WaterConsumption);
                            decimal cuM = (AddToAmount - EndRange); //accumulated "end cu. m" minus present end range.

                            cuM = (newWaterConsumption - cuM) * decimal.Parse(Rate.ToString());
                            cuM += decimal.Parse(AddToTotal.ToString());
                            waterAmount = Math.Round(cuM, 2);

                            break;
                        }
                        else
                        {
                            waterAmount = Rate; //Return this if consumption belong to first rate.
                            break;
                        }
                    }
                                      
                    AddToTotal = j > 0 ? AddToTotal + (EndRange * Rate) : BillingRates[j].Rate;
                    //PrevRange = EndRange;
                }
            }
            return waterAmount;
        }

        public static void CompleteGeneralLedger(GeneralBilling baseValue, GeneralBilling assignValue)
        {
            assignValue.GeneralBillingId = baseValue.GeneralBillingId;
            assignValue.BillingNumber = baseValue.BillingNumber;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.BillingAmount = baseValue.BillingAmount;
            assignValue.GenerationDate = baseValue.GenerationDate;
            assignValue.CoverageFrom = baseValue.CoverageFrom;
            assignValue.CoverageTo = baseValue.CoverageTo;
            assignValue.Currency = baseValue.Currency;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.UpdatedBy = baseValue.UpdatedBy;
            assignValue.UpdateDate = baseValue.UpdateDate;
        }

        public static void CompleteSubsidiaryLedger(SubsidiaryLedger baseValue, SubsidiaryLedger assignValue)
        {
            assignValue.SubsidiaryLedgerId = baseValue.SubsidiaryLedgerId;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.BillingPeriod = baseValue.BillingPeriod;
            assignValue.BillingDate = baseValue.BillingDate;
            assignValue.DueDate = baseValue.DueDate;
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.BillingReference = baseValue.BillingReference;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.DebitAmount = baseValue.DebitAmount;
            assignValue.CreditAmount = baseValue.CreditAmount;
            assignValue.TransactionDate = baseValue.TransactionDate;
            assignValue.Remarks = baseValue.Remarks;
            assignValue.DollarAmount = baseValue.DollarAmount;
            assignValue.Currency = baseValue.Currency;
            assignValue.CreatedBy = baseValue.CreatedBy;
            assignValue.CreateDate = baseValue.CreateDate;
            assignValue.UpdatedBy = baseValue.UpdatedBy;
            assignValue.UpdateDate = baseValue.UpdateDate;
            assignValue.BillingSubType = baseValue.BillingSubType;
            assignValue.Other = baseValue.Other;
        }

        public static void CompleteBalances(Balances baseValue, Balances assignValue)
        {
            assignValue.BalancesId = baseValue.BalancesId;
            assignValue.BillingType = baseValue.BillingType;
            assignValue.TransactionType = baseValue.TransactionType;
            assignValue.TransactionReference = baseValue.TransactionReference;
            assignValue.CompanyId = baseValue.CompanyId;
            assignValue.Amount = baseValue.Amount;
            assignValue.BalanceType = "FORWARDED BALANCE";
        }

    }
}