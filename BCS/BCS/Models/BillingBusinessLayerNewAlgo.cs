using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models.Balance;
using BCS.Models.Interest;

namespace BCS.Models
{
    public class BillingBusinessLayerNewAlgo
    {
        BCS_Context db = new BCS_Context();
        List<BillingCompanyList> CompanyList = new List<BillingCompanyList>();
        List<Company> companies = new List<Company>();
        List<RentalInformation> RentalInformations = new List<RentalInformation>();
        List<PoleInformation> PoleInformations = new List<PoleInformation>();
        List<WaterMeterAssignment> WaterMeterAssignments = new List<WaterMeterAssignment>();
        List<FranchiseFeeInformation> FranchiseFeeInformations = new List<FranchiseFeeInformation>();
        List<GarbageInformation> GarbageInformations = new List<GarbageInformation>();
        List<PassedOnBillingInformation> PassedOnBillingInformations = new List<PassedOnBillingInformation>();
        //List<WaterMeterReading> WaterMeterReadings = new List<WaterMeterReading>();

        public GeneralBilling generalBilling = new GeneralBilling();
        public SubsidiaryLedger subsidiaryLedger = new SubsidiaryLedger();
        public SubsidiaryLedger interestSubsidiaryLedger = new SubsidiaryLedger();
        public Balances balances = new Balances();
        public SubsidiaryLedger sewerageSubsidiaryLedger = new SubsidiaryLedger();
        public SubsidiaryLedger sewerageInterestSubsidiaryLedger = new SubsidiaryLedger();
        //public BCSAgingOutput bCSAgingOutput;

        GeneralBilling MasterGeneralBilling = new GeneralBilling();
        SubsidiaryLedger MasterSubsidiaryLedger = new SubsidiaryLedger();

        DateTime PeriodDateFrom;
        DateTime PeriodDateTo;
        DateTime CoverageFrom;
        DateTime CoverageTo;
        DateTime BillingDate;
        DateTime BillingDue;
        int billingPeriodId;
        string fxRate;
        string GroupName;
        int MaxId;
        string userid;
        string TypeOfBillingGenerate;
        int generatePerCompanyId;
        string TypeOfBillingGenerateValue;
        public BillingBusinessLayerNewAlgo(string GroupName, DateTime CoverageFrom, DateTime CoverageTo, DateTime BillingDate, DateTime BillingDue, int billingPeriodId, string userid, string fxRate, int generatePerCompanyId, string TypeOfBillingGenerate, string TypeOfBillingGenerateValue)
        {
            if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERCOMPANY") //Generate billing per company.
            {
                List<Company> listOfCompanies = new List<Company>();
                Company comp = db.Company.Where(m => m.CompanyID == generatePerCompanyId).SingleOrDefault();
                listOfCompanies.Add(comp);
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(listOfCompanies, GroupName, CoverageFrom, CoverageTo, billingPeriodId, TypeOfBillingGenerate, generatePerCompanyId, TypeOfBillingGenerateValue);
                companies = searchCompanyPerGroup.Companies;
                //RentalInformations = searchCompanyPerGroup.RentalInformations;
                //PoleInformations = searchCompanyPerGroup.PoleInformations;
                //WaterMeterAssignments = searchCompanyPerGroup.WaterInformations;
                //FranchiseFeeInformations = searchCompanyPerGroup.FranchiseInformations;
                //GarbageInformations = searchCompanyPerGroup.GarbageInformations;
                if(TypeOfBillingGenerateValue != "AFITPark" && TypeOfBillingGenerateValue != "AFManufacturing" && TypeOfBillingGenerateValue != "AFOthers")
                    PassedOnBillingInformations = searchCompanyPerGroup.PassedOnBillingInformations;
                //Below code not available. do not uncomment-------------
                //WaterMeterReadings = searchCompanyPerGroup.WaterReading;
            }
            else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERPOB")
            {
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(GroupName, CoverageFrom, CoverageTo, billingPeriodId, TypeOfBillingGenerate, TypeOfBillingGenerateValue);
                companies = searchCompanyPerGroup.Companies;                
                PassedOnBillingInformations = searchCompanyPerGroup.PassedOnBillingInformations;
            }
            else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERBILLINGTYPE")
            {
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(GroupName, CoverageFrom, CoverageTo, billingPeriodId);
                companies = searchCompanyPerGroup.Companies;
                if (TypeOfBillingGenerateValue.ToUpper() == "RENTAL")
                    RentalInformations = searchCompanyPerGroup.RentalInformations;
                if (TypeOfBillingGenerateValue.ToUpper() == "POLE")
                    PoleInformations = searchCompanyPerGroup.PoleInformations;
                if (TypeOfBillingGenerateValue.ToUpper() == "WATER")
                    WaterMeterAssignments = searchCompanyPerGroup.WaterInformations;
                if (TypeOfBillingGenerateValue.ToUpper() == "FRANCHISE")
                    FranchiseFeeInformations = searchCompanyPerGroup.FranchiseInformations;
                if (TypeOfBillingGenerateValue.ToUpper() == "GARBAGE")
                    GarbageInformations = searchCompanyPerGroup.GarbageInformations;
                if (TypeOfBillingGenerateValue.ToUpper() == "PASSED ON BILLING")
                    PassedOnBillingInformations = searchCompanyPerGroup.PassedOnBillingInformations;
            }
            else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERADMINFEE")
            {
                
            }
            else
            {
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(GroupName, CoverageFrom, CoverageTo, billingPeriodId); //Generate all billing
                companies = searchCompanyPerGroup.Companies;
                RentalInformations = searchCompanyPerGroup.RentalInformations;
                PoleInformations = searchCompanyPerGroup.PoleInformations;
                WaterMeterAssignments = searchCompanyPerGroup.WaterInformations;
                FranchiseFeeInformations = searchCompanyPerGroup.FranchiseInformations;
                GarbageInformations = searchCompanyPerGroup.GarbageInformations;
                PassedOnBillingInformations = searchCompanyPerGroup.PassedOnBillingInformations;
                //WaterMeterReadings = searchCompanyPerGroup.WaterReading;
            }

            this.CoverageFrom = CoverageFrom;
            this.CoverageTo = CoverageTo;
            this.BillingDate = BillingDate;
            this.BillingDue = BillingDue;
            this.billingPeriodId = billingPeriodId;
            this.fxRate = fxRate;
            this.GroupName = GroupName;
            this.userid = userid;
            this.TypeOfBillingGenerate = TypeOfBillingGenerate;
            this.generatePerCompanyId = generatePerCompanyId;
            this.TypeOfBillingGenerateValue = TypeOfBillingGenerateValue;

            MasterGeneralBilling.BillingPeriod = billingPeriodId;
            MasterGeneralBilling.BillingDate = BillingDate;
            MasterGeneralBilling.DueDate = BillingDue;
            MasterGeneralBilling.TransactionType = "BILLING";
            MasterGeneralBilling.GenerationDate = DateTime.Now;
            MasterGeneralBilling.CoverageFrom = CoverageFrom;
            MasterGeneralBilling.CoverageTo = CoverageTo;
            MasterGeneralBilling.CreatedBy = userid;
            MasterGeneralBilling.CreateDate = DateTime.Now;
            MasterGeneralBilling.UpdateDate = DateTime.Now;

            var genStatus = db.BillingPeriod.Where(m => m.BillingPeriodId == billingPeriodId).FirstOrDefault().Generated ?? "";
            if (genStatus.ToUpper() == "YES")
                MasterGeneralBilling.Status = "Generated";
            else
                MasterGeneralBilling.Status = "Regenerated";

            MasterSubsidiaryLedger.BillingPeriod = billingPeriodId;
            MasterSubsidiaryLedger.BillingDate = BillingDate;
            MasterSubsidiaryLedger.DueDate = BillingDue;
            MasterSubsidiaryLedger.TransactionType = "BILLING";
            MasterSubsidiaryLedger.BillingSubType = "PRINCIPAL";
            MasterSubsidiaryLedger.CreditAmount = 0;
            MasterSubsidiaryLedger.TransactionDate = DateTime.Now;
            MasterSubsidiaryLedger.CreatedBy = userid;
            MasterSubsidiaryLedger.CreateDate = DateTime.Now;
            MasterSubsidiaryLedger.UpdateDate = DateTime.Now;

            MaxId = Convert.ToInt32(getMaxBillingNumber());
            PeriodDateFrom = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == billingPeriodId).DateFrom;
            PeriodDateTo = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == billingPeriodId).DateTo;
        }
        //END -------------------------------------------------------------
        private string getMaxBillingNumber()
        {
            string maxBillingNumber;
            var yr = DateTime.Now.ToString("yy");
            var billingNumbers = db.SubsidiaryLedger.Where(m => m.TransactionReference.Substring(3, 2) == yr)
                .Select(g => new { series = g.TransactionReference.Substring(5, 4).ToString() }).ToList();

            if (billingNumbers != null || billingNumbers.Count > 0) //Check if year exist
            {
                if (DateTime.Now.Month == 1) // check if January
                {
                    //If yes. roll back to zero
                    maxBillingNumber = "0";
                }
                else
                {
                    //if no. increment max series
                    maxBillingNumber = billingNumbers.Max(m => m.series);
                }
            }
            else
            {
                maxBillingNumber = "0";
            }
            return maxBillingNumber;
        }
        private string padBillingNumber(int billnum)
        {
            ++billnum;
            string strBillingNum = billnum.ToString().PadLeft(4, '0');

            return strBillingNum;
        }
        public bool isValidFxRate(string InputFxRate)
        {
            decimal fxrate = 0;

            bool isCurrencyValid = true;
            foreach (var rental in RentalInformations)
            {
                if (rental.Currency.ToUpper().Trim() == "USD") //Check for USD currency
                {
                    decimal outFxRate;
                    if (decimal.TryParse(InputFxRate, out outFxRate)) //Try to parse  returned FX rate value
                    {
                        if (outFxRate <= 0) //if value is less than equal to zero NOT VALID
                        {
                            isCurrencyValid = false;
                            fxrate = decimal.Parse(InputFxRate);
                            break;
                        }
                        else
                        {
                            isCurrencyValid = true;
                            break;
                        }
                    }
                    else //if parse is failed NOT VALID
                        isCurrencyValid = false;
                }
            }
            return isCurrencyValid;
        }
        private int generateBillingNumber(int companyId) //Function will return 1 billing reference number per company in all services...
        {
            int repMaxId = 0;

            //This block will prevent multiple REFERENCE NUMBER per Company in "Generate billing per Company"
            if (TypeOfBillingGenerate.ToUpper() == "PERCOMPANY")
            {
                int? maxid = db.SubsidiaryLedger.Max(m => (int?)m.SubsidiaryLedgerId);
                if (maxid != null)
                {
                    //Substring and Parse the list of Transactionreference
                    try
                    {
                        var m1 = db.SubsidiaryLedger.Where(m => m.TransactionType == "BILLING").Select(m => m.TransactionReference.Substring(5, 4)).Select(int.Parse).ToList();
                        int i = m1.Count();
                        return (i + 1);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }

                }
            }


            bool addNew = true;
            if (MaxId > 0)
            {
                foreach (var item in CompanyList) //Check if Company Id is already in the list
                {
                    if (item.CompanyId == companyId || TypeOfBillingGenerate.ToUpper() == "PERCOMPANY")
                    {
                        repMaxId = item.BillingId; //If in the list. Copy the BillingId and return
                        addNew = false;
                        break;
                    }
                }
            }
            else //If not in the list generate new BillingId and include in the list.
            {
                MaxId++;
                repMaxId = MaxId;
                addNew = false;
                CompanyList.Add(new BillingCompanyList { CompanyId = companyId, BillingId = MaxId });
            }

            if (addNew)
            {
                MaxId++;
                repMaxId = MaxId;
                CompanyList.Add(new BillingCompanyList { CompanyId = companyId, BillingId = MaxId });
            }

            return repMaxId;
        }

        public void generateBilling()
        {
            BillingPeriod billingPeriod = db.BillingPeriod.Find(billingPeriodId);
            string isgenerated = billingPeriod.Generated;

            using (var dbtransaction = db.Database.BeginTransaction())
            {
                #region pole
                //-----------------------------START POLE---------------------------------------
                decimal PoleAmount = 0;
                foreach (var PoleInformation in PoleInformations)
                {

                    ValidateServicesInformations validateServicesInformations = new ValidateServicesInformations(PoleInformation.BillingMonths, PoleInformation.StartDate, PoleInformation.EndDate, CoverageFrom, CoverageTo);

                    //Get the previous Billing period of current company-----------------------------------------------------------------------------------------
                    var lastid = "0";
                    IEnumerable<Balances> ISub;
                    //ISub = db.SubsidiaryLedger.Where(k => k.CompanyId == PoleInformation.CompanyId && k.BillingType == "POLE RENTAL" && k.BillingReference == PoleInformation.PoleInformationId.ToString()).ToList();
                    ISub = db.Balances.Where(k => k.CompanyId == PoleInformation.CompanyId && k.BillingType == "POLE RENTAL" && k.BillingReference == PoleInformation.PoleInformationId.ToString()).ToList();

                    if (ISub.Count() > 0)
                        lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                    //--------------Start Changes 1-3-2017
                    bool isValid = false;
                    bool isValidBillingMonths = false;
                    int multiplier = 0;

                    isValidBillingMonths = validateServicesInformations.ValidateBillingMonths();
                    isValid = validateServicesInformations.isValidInformation();
                    multiplier = validateServicesInformations.multiplier;

                    if (isValid == false)
                    {
                        //InterestComputation ValidateInterestComputation = new InterestComputation(CoverageFrom, BillingDate, PoleInformation.CompanyId, PoleInformation.PoleInformationId.ToString());
                        PrincipalBalance PolePrincipal = new PrincipalBalance();
                        var polePrincipalBalance = PolePrincipal.Balance("POLE RENTAL", PoleInformation.PoleInformationId.ToString(), PoleInformation.CompanyId, int.Parse(lastid));
                        if (polePrincipalBalance > 0)
                        {
                            isValid = true;
                            multiplier = 0;
                        }

                    }
                    //---------End Changes 1-3-2017    

                    if (isValid && isValidBillingMonths)
                    {
                        GeneralBilling localGeneralBilling = new GeneralBilling();
                        GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                        SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                        Balances localBalances = new Balances();
                        Balances localBalancesAdvance = new Balances();
                        VatValidation vat = new VatValidation(PoleInformation.CompanyId);
                        ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                        ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                        //var PoleBillingNum = db.GeneralBilling.Where(m => m.CompanyId == PoleInformation.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                        var PoleBillingNum = generateBillingNumber(PoleInformation.CompanyId);

                        //Start Using new computation 02-24-17
                        //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, PoleInformation.CompanyId, PoleInformation.PoleInformationId.ToString());
                        PrincipalBalance PolePrincipal = new PrincipalBalance();
                        VatBalance PoleVAT = new VatBalance();
                        InterestBalance PoleInterestBalance = new InterestBalance();
                        InterestWithoutBillingPeriod PoleInterest = new InterestWithoutBillingPeriod();
                        var polePrincipalBalance = PolePrincipal.Balance("POLE RENTAL", PoleInformation.PoleInformationId.ToString(), PoleInformation.CompanyId, int.Parse(lastid));
                        var polePrincipalInterest = polePrincipalBalance > 0 ? PoleInterest.Interest("POLE RENTAL", PoleInformation.CompanyId, PoleInformation.PoleInformationId.ToString(), CoverageFrom, BillingDate, polePrincipalBalance, polePrincipalBalance, lastid, PoleInformation.Amount) : 0;
                        var poleVatBalance = PoleVAT.Balance("POLE RENTAL", PoleInformation.PoleInformationId.ToString(), PoleInformation.CompanyId, int.Parse(lastid));
                        var poleInterest = PoleInterestBalance.Balance("POLE RENTAL", PoleInformation.PoleInformationId.ToString(), PoleInformation.CompanyId, int.Parse(lastid));
                        //End Using new computation 02-24-17

                        //PoleAmount = interestComputation.PoleRental > 0 ? PoleInformation.Amount * interestComputation.interest : PoleInformation.Amount;
                        //PoleAmount *= validateServicesInformations.multiplier;
                        localGeneralBilling.CompanyId = PoleInformation.CompanyId;
                        localGeneralBilling.BillingType = "POLE RENTAL";//Pole Rental
                        localGeneralBilling.BillingReference = PoleInformation.PoleInformationId.ToString();
                        localGeneralBilling.BillingAmount = PoleInformation.Amount * multiplier;//Changes 1-3-2017
                        localGeneralBilling.Currency = "PHP";
                        localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(PoleBillingNum));

                        //Saving of Advances in Generalledger------------------------------------------------------------------------------------------------------
                        if (polePrincipalBalance < 0)
                        {
                            ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                            localGeneralBillingAdvance.BillingAmount = Math.Abs(polePrincipalInterest);
                            localGeneralBillingAdvance.TransactionType = "Advance";
                            db.GeneralBilling.Add(localGeneralBillingAdvance);
                        }

                        localSubsidiary.BillingType = "POLE RENTAL";
                        localSubsidiary.CompanyId = PoleInformation.CompanyId;
                        localSubsidiary.BillingReference = PoleInformation.PoleInformationId.ToString();
                        localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(PoleBillingNum);
                        localSubsidiary.Currency = "PHP";
                        localSubsidiary.DebitAmount = PoleInformation.Amount * multiplier; //Changes 1-3-2017
                        localSubsidiary.DollarAmount = 0;

                        //Save Advances in Balance table ---------------------------------------------------------------------------------------------------------

                        //Billing Sub Type = BALANCE
                        ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, polePrincipalBalance, poleInterest, poleVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                        db.Balances.Add(localBalancesAdvance);


                        //BILLING SUB TYPE = PRINCIPAL
                        ClassAssignment.AssignBalance(localSubsidiary, localBalances, polePrincipalBalance, poleInterest, poleVatBalance, lastid.ToString());

                        //Save interest ------------------------------------------------------------------------------------------------------------------------------
                        if (polePrincipalBalance > 0)
                        {
                            ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                            localInterestSubsidiary.DebitAmount = polePrincipalInterest;
                            db.SubsidiaryLedger.Add(localInterestSubsidiary);
                        }

                        if (vat.hasVat())
                        {
                            ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                            db.SubsidiaryLedger.Add(localVatSubsidiary);
                        }

                        PoleInformation info = new PoleInformation();
                        info = db.PoleInformation.Find(PoleInformation.PoleInformationId);

                        if (isgenerated.ToUpper() == "NO")
                            info.BillingStatus = "Billed";
                        else
                            info.BillingStatus = "Regenerated";

                        db.Entry(info).State = System.Data.Entity.EntityState.Modified;

                        db.GeneralBilling.Add(localGeneralBilling);
                        db.SubsidiaryLedger.Add(localSubsidiary);

                        localBalances.PeriodDateFrom = PeriodDateFrom;
                        localBalances.PeriodDateTo = PeriodDateTo;
                        localBalances.DueDate = BillingDue;
                        db.Balances.Add(localBalances);


                        //Start of changes 02-9-17. Remove if Type in Balances will not be used
                        Balances vatBalance = new Balances();
                        Balances intBalance = new Balances();
                        ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                        ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                        db.Balances.Add(vatBalance);
                        db.Balances.Add(intBalance);
                        //Start of changes 02-9-17. Remove if Type in Balances will not be used

                        //db.BCSAgingOutput.Add(bCSAgingOutput);
                    }
                }//End of foreach
                #endregion pole
                //------------END POLE-------------------------------------------------------------------------------------

                //------------START RENTAL---------------------------------------------------------------------------------

                decimal RentalAmount = 0;
                if (isValidFxRate(fxRate))
                {
                    foreach (var rentalInformation in RentalInformations)
                    {
                        ValidateServicesInformations validateServicesInformations = new ValidateServicesInformations(rentalInformation.BillingMonths, rentalInformation.StartDate, rentalInformation.EndDate, CoverageFrom, CoverageTo);

                        var lastid = "0";
                        IEnumerable<Balances> ISub;
                        //ISub = db.SubsidiaryLedger.Where(k => k.CompanyId == rentalInformation.CompanyId && k.BillingType == "RENTAL" && k.BillingReference == rentalInformation.RentalInformationId.ToString()).ToList();
                        ISub = db.Balances.Where(k => k.CompanyId == rentalInformation.CompanyId && k.BillingType == "RENTAL" && k.BillingReference == rentalInformation.RentalInformationId.ToString()).ToList();

                        if (ISub.Count() > 0)
                            lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                        //--------------Start Changes 1-3-2017
                        bool isValid = false;
                        int multiplier = 0;

                        isValid = validateServicesInformations.isValidInformation();
                        multiplier = validateServicesInformations.multiplier;

                        if (isValid == false)
                        {
                            //InterestComputation ValidateInterestComputation = new InterestComputation(CoverageFrom, BillingDate, rentalInformation.CompanyId, rentalInformation.RentalInformationId.ToString());
                            PrincipalBalance RentalPrincipal = new PrincipalBalance();
                            var rentalPrincipalBalance = RentalPrincipal.Balance("RENTAL", rentalInformation.RentalInformationId.ToString(), rentalInformation.CompanyId, int.Parse(lastid));
                            if (rentalPrincipalBalance > 0)
                            {
                                isValid = true;
                                multiplier = 0;
                            }
                        }
                        //---------End Changes 1-3-2017  

                        if (isValid)
                        {
                            GeneralBilling localGeneralBilling = new GeneralBilling();
                            GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                            SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                            SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                            Balances localBalances = new Balances();
                            Balances localBalancesAdvance = new Balances();
                            SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                            VatValidation vat = new VatValidation(rentalInformation.CompanyId);
                            ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                            ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                            //var RentalBillingNum = db.GeneralBilling.Where(m => m.CompanyId == rentalInformation.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                            var RentalBillingNum = generateBillingNumber(rentalInformation.CompanyId);

                            //Start Using new computation 02-24-17
                            //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, rentalInformation.CompanyId, rentalInformation.RentalInformationId.ToString());
                            PrincipalBalance RentalPrincipal = new PrincipalBalance();
                            VatBalance RentalVAT = new VatBalance();
                            InterestBalance RentalInterestBalance = new InterestBalance();
                            InterestWithoutBillingPeriod RentalInterest = new InterestWithoutBillingPeriod();
                            var rentalPrincipalBalance = RentalPrincipal.Balance("RENTAL", rentalInformation.RentalInformationId.ToString(), rentalInformation.CompanyId, int.Parse(lastid));
                            var rentalPrincipalInterest = rentalPrincipalBalance > 0 ? RentalInterest.Interest("RENTAL", rentalInformation.CompanyId, rentalInformation.RentalInformationId.ToString(), CoverageFrom, BillingDate, rentalPrincipalBalance, rentalPrincipalBalance, lastid, rentalInformation.Amount) : 0;
                            var rentalVatBalance = RentalVAT.Balance("RENTAL", rentalInformation.RentalInformationId.ToString(), rentalInformation.CompanyId, int.Parse(lastid));
                            var rentalInterest = RentalInterestBalance.Balance("RENTAL", rentalInformation.RentalInformationId.ToString(), rentalInformation.CompanyId, int.Parse(lastid));
                            //End Using new computation 02-24-17

                            //RentalAmount = interestComputation.Rental > 0 ? (rentalInformation.Area * rentalInformation.Rate) * interestComputation.interest : rentalInformation.Area * rentalInformation.Rate;
                            //RentalAmount *= validateServicesInformations.multiplier;
                            localGeneralBilling.CompanyId = rentalInformation.CompanyId;
                            localGeneralBilling.BillingType = "RENTAL";
                            localGeneralBilling.BillingReference = rentalInformation.RentalInformationId.ToString();
                            localGeneralBilling.BillingAmount = rentalInformation.Amount * multiplier;
                            localGeneralBilling.Currency = rentalInformation.Currency;
                            localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(RentalBillingNum));

                            if (rentalPrincipalBalance < 0)
                            {
                                ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                                localGeneralBillingAdvance.BillingAmount = Math.Abs(rentalPrincipalInterest);
                                localGeneralBillingAdvance.TransactionType = "Advance";
                                db.GeneralBilling.Add(localGeneralBillingAdvance);
                            }

                            localSubsidiary.BillingType = "RENTAL";
                            localSubsidiary.CompanyId = rentalInformation.CompanyId;
                            localSubsidiary.BillingReference = rentalInformation.RentalInformationId.ToString();
                            localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(RentalBillingNum);
                            localSubsidiary.Currency = rentalInformation.Currency;
                            //localSubsidiary.DebitAmount = rentalInformation.Currency.ToUpper().Trim() == "USD" ? (rentalInformation.Amount * decimal.Parse(fxRate)) * validateServicesInformations.multiplier : rentalInformation.Amount * validateServicesInformations.multiplier;
                            //localSubsidiary.DollarAmount = rentalInformation.Currency.ToUpper().Trim() == "USD" ? rentalInformation.Amount * validateServicesInformations.multiplier : 0;
                            localSubsidiary.DebitAmount = rentalInformation.Amount * multiplier;
                            localSubsidiary.DollarAmount = rentalInformation.Currency.ToUpper().Trim() == "USD" ? rentalInformation.Amount * multiplier : 0;

                            if (rentalInformation.Currency.ToUpper().Trim() == "USD")
                            {
                                decimal? fxr = Convert.ToDecimal(fxRate);
                                localSubsidiary.FXRate = fxr;
                                localSubsidiary.Peso = fxr * (rentalInformation.Amount * multiplier);
                            }




                            ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, rentalPrincipalBalance, rentalInterest, rentalVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                            db.Balances.Add(localBalancesAdvance);



                            ClassAssignment.AssignBalance(localSubsidiary, localBalances, rentalPrincipalBalance, rentalInterest, rentalVatBalance, lastid.ToString());

                            if (rentalPrincipalBalance > 0)
                            {
                                ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                                //localInterestSubsidiary.DebitAmount = rentalInformation.Currency.ToUpper().Trim() == "USD" ? interestComputation.interest * decimal.Parse(fxRate) : interestComputation.interest;
                                //localInterestSubsidiary.DollarAmount = rentalInformation.Currency.ToUpper().Trim() == "USD" ? interestComputation.interest : 0;
                                localInterestSubsidiary.DebitAmount = rentalPrincipalInterest;

                                if (rentalInformation.Currency.ToUpper().Trim() == "USD")
                                {
                                    decimal? fxr = Convert.ToDecimal(fxRate);
                                    localInterestSubsidiary.FXRate = fxr;
                                    localInterestSubsidiary.Peso = fxr * rentalPrincipalInterest;
                                    localInterestSubsidiary.DollarAmount = rentalPrincipalInterest;
                                }


                                db.SubsidiaryLedger.Add(localInterestSubsidiary);
                            }

                            if (vat.hasVat())
                            {
                                if (rentalInformation.Currency.ToUpper().Trim() == "USD")
                                {
                                    decimal? fxr = Convert.ToDecimal(fxRate);
                                    localVatSubsidiary.FXRate = fxr;
                                    localVatSubsidiary.Peso = fxr * localVatSubsidiary.DebitAmount;
                                    localVatSubsidiary.DollarAmount = localVatSubsidiary.DebitAmount;
                                }

                                ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);

                                db.SubsidiaryLedger.Add(localVatSubsidiary);
                            }

                            RentalInformation info = new RentalInformation();
                            info = db.RentalInformation.Find(rentalInformation.RentalInformationId);

                            if (isgenerated.ToUpper() == "NO")
                                info.BillingStatus = "Billed";
                            else
                                info.BillingStatus = "Regenerated";

                            db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                            db.GeneralBilling.Add(localGeneralBilling);
                            db.SubsidiaryLedger.Add(localSubsidiary);

                            localBalances.PeriodDateFrom = PeriodDateFrom;
                            localBalances.PeriodDateTo = PeriodDateTo;
                            //localBalances.VAT = interestComputation.balanceOfVat;
                            localBalances.DueDate = BillingDue;
                            db.Balances.Add(localBalances);

                            Balances vatBalance = new Balances();
                            Balances intBalance = new Balances();
                            ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                            ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                            db.Balances.Add(vatBalance);
                            db.Balances.Add(intBalance);
                            //bCSAgingOutput = new BCSAgingOutput();
                            //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                            //db.BCSAgingOutput.Add(bCSAgingOutput);
                        }
                    }//End of foreach
                }
                //--------------END RENTAL------------------------------------------------------------------------------------------

                //--------------START WATER------------------------------------------------------------------------------------------

                decimal? WaterAmount = 0;
                decimal? InterestWaterAmount = 0;
                decimal? SewerageAmount = db.BillingRates.Single(m => m.Category == "Sewerage" && m.ZoneGroup == GroupName).Rate;
                decimal? InterestSewerageAmount = 0;
                foreach (var waterMeterAssignments in WaterMeterAssignments)
                {
                    //--------------Start Changes 1-3-2017
                    bool isValidWater = true;
                    int multiplierWater = 1;
                    bool isValidSewerage = true;
                    int multiplierSewerage = 1;

                    if (waterMeterAssignments.CompanyId == 321)
                    {

                    }

                    //var lastid = db.SubsidiaryLedger.Where(k => k.CompanyId == reading.CompanyId && k.BillingType == "WATER" && k.BillingReference == reading.MeterNumber.ToString()).Max(j => j.BillingPeriod);
                    var lastid = "0";
                    IEnumerable<Balances> ISub;
                    ISub = db.Balances.Where(k => k.CompanyId == waterMeterAssignments.CompanyId && k.BillingType == "WATER" && k.BillingReference == waterMeterAssignments.MeterNumber.ToString()).ToList();

                    if (ISub.Count() > 0)
                        lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                    PrincipalBalanceWaterAndSewerage waterPrincipalBalance = new PrincipalBalanceWaterAndSewerage();
                    PrincipalBalanceWaterAndSewerage seweragePrincipalBalance = new PrincipalBalanceWaterAndSewerage();
                    VatBalanceWaterAndSewerage waterVatBalance = new VatBalanceWaterAndSewerage();
                    VatBalanceWaterAndSewerage sewerageVatBalance = new VatBalanceWaterAndSewerage();
                    InterestBalanceWaterAndSewerage waterInterestBalance = new InterestBalanceWaterAndSewerage();
                    InterestBalanceWaterAndSewerage sewerageInterestBalance = new InterestBalanceWaterAndSewerage();
                    InterestWaterAndSewerage interestWithBillingPeriod = new InterestWaterAndSewerage();

                    var _waterPrincipalBalance = waterPrincipalBalance.Balance("WATER", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));
                    var _seweragePrincipalBalance = seweragePrincipalBalance.Balance("SEWERAGE", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));
                    var _waterVatBalance = waterVatBalance.Balance("WATER", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));
                    var _sewerageVatBalance = sewerageVatBalance.Balance("SEWERAGE", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));
                    var _waterInterestBalance = waterInterestBalance.Balance("WATER", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));
                    var _sewerageInterestBalance = sewerageInterestBalance.Balance("SEWERAGE", waterMeterAssignments.MeterNumber, waterMeterAssignments.CompanyId, int.Parse(lastid));

                    //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber);
                    //InterestComputation sewerageInterest = new InterestComputation(CoverageFrom, BillingDate, waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber);
                    List<WaterMeterReading> waterMeterReading = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Where(m => m.BillingPeriod == billingPeriodId).ToList();

                    if (waterMeterReading.Count == 0)
                    {
                        if (_waterPrincipalBalance > 0)
                        {
                            isValidWater = true;
                            multiplierWater = 0;
                            var i = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Max(m => m.WaterMeterReadingId);
                            waterMeterReading = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Where(m => m.WaterMeterReadingId == i).ToList();
                        }
                        else
                        {
                            isValidWater = true;
                            multiplierWater = 0;
                            var i = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Max(m => m.WaterMeterReadingId);
                            waterMeterReading = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Where(m => m.WaterMeterReadingId == i).ToList();
                        }

                        if (_seweragePrincipalBalance > 0)
                        {
                            isValidSewerage = true;
                            multiplierSewerage = 0;
                            var i = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Max(m => m.WaterMeterReadingId);
                            waterMeterReading = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Where(m => m.WaterMeterReadingId == i).ToList();
                        }
                        else
                        {
                            isValidSewerage = true;
                            multiplierSewerage = 0;
                            var i = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Max(m => m.WaterMeterReadingId);
                            waterMeterReading = db.WaterMeterReading.Where(m => m.MeterNumber == waterMeterAssignments.MeterNumber).Where(m => m.WaterMeterReadingId == i).ToList();
                        }
                    }
                    //---------End Changes 1-3-2017   
                    foreach (var reading in waterMeterReading)
                    {
                        if (isValidWater || isValidSewerage)
                        {
                            GeneralBilling localGeneralBilling = new GeneralBilling();
                            GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                            SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                            SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                            SubsidiaryLedger localSewerageSubsidiary = new SubsidiaryLedger();
                            SubsidiaryLedger localInterestSewerageSubsidiary = new SubsidiaryLedger();
                            Balances localBalances = new Balances();
                            Balances localSewerageBalances = new Balances();
                            Balances localBalancesAdvance = new Balances();
                            Balances localSewerageBalancesAdvance = new Balances();
                            SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                            SubsidiaryLedger localInterestVatSubsidiary = new SubsidiaryLedger();

                            ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                            ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                            //var WaterlBillingNum = db.GeneralBilling.Where(m => m.CompanyId == waterMeterAssignments.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                            var WaterlBillingNum = generateBillingNumber(waterMeterAssignments.CompanyId);
                            //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber);
                            //InterestComputation sewerageInterest = new InterestComputation(CoverageFrom, BillingDate, waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber);

                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            VatValidation vat = new VatValidation(reading.CompanyId);
                            WaterAmount = ClassAssignment.GetWaterAmount((reading.PresentReading - reading.PreviousReading), userid);
                            //InterestWaterAmount = _waterPrincipalBalance > 0 ? WaterAmount * _waterPrincipalBalance : WaterAmount;
                            //InterestSewerageAmount = _seweragePrincipalBalance > 0 ? ((reading.PresentReading - reading.PreviousReading) * (SewerageAmount / 100)) * _seweragePrincipalBalance : ((reading.PresentReading - reading.PreviousReading) * (SewerageAmount / 100));
                            var waterInterest = _waterPrincipalBalance > 0 ? interestWithBillingPeriod.Interest("WATER", waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber, CoverageFrom, BillingDate, _waterPrincipalBalance, lastid, (decimal)WaterAmount) : 0;
                            var sewerageInterest = _seweragePrincipalBalance > 0 ? interestWithBillingPeriod.Interest("SEWERAGE", waterMeterAssignments.CompanyId, waterMeterAssignments.MeterNumber, CoverageFrom, BillingDate, _seweragePrincipalBalance, lastid, (decimal)WaterAmount) : 0;

                            localGeneralBilling.CompanyId = reading.CompanyId;
                            localGeneralBilling.BillingType = "WATER";
                            localGeneralBilling.BillingReference = reading.MeterNumber;
                            localGeneralBilling.BillingAmount = decimal.Parse(WaterAmount.ToString()) * multiplierWater;
                            localGeneralBilling.Currency = "PHP";
                            localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(WaterlBillingNum));

                            if (_waterPrincipalBalance < 0)
                            {
                                ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                                localGeneralBillingAdvance.BillingAmount = Math.Abs(_waterPrincipalBalance);
                                localGeneralBillingAdvance.TransactionType = "Advance";
                                db.GeneralBilling.Add(localGeneralBillingAdvance);
                            }

                            localSubsidiary.BillingType = "WATER";
                            localSubsidiary.CompanyId = reading.CompanyId;
                            localSubsidiary.BillingReference = reading.MeterNumber;
                            localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(WaterlBillingNum);
                            localSubsidiary.Currency = "PHP";
                            localSubsidiary.DebitAmount = decimal.Parse(WaterAmount.ToString()) * multiplierWater;
                            localSubsidiary.DollarAmount = 0;
                            localSubsidiary.Other = reading.MeterNumber;

                            ClassAssignment.AssignInterest(localSubsidiary, localSewerageSubsidiary);
                            localSewerageSubsidiary.DebitAmount = (decimal.Parse(((reading.PresentReading - reading.PreviousReading) * SewerageAmount).ToString())) * multiplierSewerage;
                            localSewerageSubsidiary.BillingType = "SEWERAGE";
                            localSewerageSubsidiary.UpdateDate = DateTime.Now;
                            localSewerageSubsidiary.TransactionType = "BILLING";
                            localSewerageSubsidiary.BillingSubType = "PRINCIPAL";

                            ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, _waterPrincipalBalance, _waterInterestBalance, _waterVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                            db.Balances.Add(localBalancesAdvance);

                            ClassAssignment.AssignBalance(localSubsidiary, localBalances, _waterPrincipalBalance, _waterInterestBalance, _waterVatBalance, lastid.ToString());

                            if (_waterPrincipalBalance > 0)
                            {
                                ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                                localInterestSubsidiary.DebitAmount = Math.Abs(waterInterest);
                                db.SubsidiaryLedger.Add(localInterestSubsidiary);
                            }

                            if (vat.hasVat())
                            {
                                ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                                db.SubsidiaryLedger.Add(localVatSubsidiary);

                                ClassAssignment.AssignVat(localSewerageSubsidiary, localInterestVatSubsidiary); //SEWERAGE-------------------------------------
                                db.SubsidiaryLedger.Add(localInterestVatSubsidiary);
                            }

                            WaterMeterReading info = new WaterMeterReading();
                            info = db.WaterMeterReading.Find(reading.WaterMeterReadingId);

                            if (isgenerated.ToUpper() == "NO")
                                info.BillingStatus = "Billed";
                            else
                                info.BillingStatus = "Regenerated";

                            db.Entry(info).State = System.Data.Entity.EntityState.Modified;

                            db.GeneralBilling.Add(localGeneralBilling);
                            db.SubsidiaryLedger.Add(localSubsidiary);
                            db.SubsidiaryLedger.Add(localSewerageSubsidiary);

                            localBalances.PeriodDateFrom = PeriodDateFrom;
                            localBalances.PeriodDateTo = PeriodDateTo;
                            //localBalances.VAT = localSubsidiary.DebitAmount * 0.12M;
                            localBalances.DueDate = BillingDue;
                            db.Balances.Add(localBalances);

                            Balances vatBalance = new Balances();
                            Balances intBalance = new Balances();
                            ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                            ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                            db.Balances.Add(vatBalance);
                            db.Balances.Add(intBalance);
                            //bCSAgingOutput = new BCSAgingOutput();
                            //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                            //db.BCSAgingOutput.Add(bCSAgingOutput);

                            //SEWERAGE---------------------------------------------------------

                            ClassAssignment.AssignBalanceAdvance(localSewerageSubsidiary, localSewerageBalancesAdvance, _seweragePrincipalBalance, _sewerageInterestBalance, _sewerageVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                            db.Balances.Add(localSewerageBalancesAdvance);

                            ClassAssignment.AssignBalance(localSewerageSubsidiary, localSewerageBalances, _seweragePrincipalBalance, _sewerageInterestBalance, _sewerageVatBalance, lastid.ToString());
                            localSewerageBalances.PeriodDateFrom = PeriodDateFrom;
                            localSewerageBalances.PeriodDateTo = PeriodDateTo;
                            //localSewerageBalances.VAT = localSewerageSubsidiary.DebitAmount * 0.12M;
                            localSewerageBalances.DueDate = BillingDue;
                            localSewerageBalances.BillingType = "SEWERAGE";
                            db.Balances.Add(localSewerageBalances);


                            Balances vatBalanceSewerage = new Balances();
                            Balances intBalanceSewerage = new Balances();
                            ClassAssignment.AssignBalanceCompleteVat(localSewerageBalances, vatBalanceSewerage);
                            ClassAssignment.AssignBalanceCompleteInterest(localSewerageBalances, intBalanceSewerage);
                            db.Balances.Add(vatBalanceSewerage);
                            db.Balances.Add(intBalanceSewerage);
                            //BCSAgingOutput bCSAgingOutputSewerage = new BCSAgingOutput();
                            //ClassAssignment.AssignBalanceComplete(localSewerageBalances, bCSAgingOutputSewerage);
                            //db.BCSAgingOutput.Add(bCSAgingOutputSewerage);

                            if (_seweragePrincipalBalance > 0)
                            {
                                ClassAssignment.AssignInterest(localSewerageSubsidiary, localInterestSewerageSubsidiary);
                                localInterestSewerageSubsidiary.DebitAmount = Math.Abs(sewerageInterest);
                                db.SubsidiaryLedger.Add(localInterestSewerageSubsidiary);
                            }
                        }
                    }
                }
                //---------------END WATER--------------------------------------------------------------------------------------------

                //---------------START FRANCHISE---------------------------------------------------------------------------------------

                decimal FranchiseAmount = 0;

                foreach (var franchiseFeeInformation in FranchiseFeeInformations)
                {
                    ValidateServicesInformations validateServicesInformations = new ValidateServicesInformations(franchiseFeeInformation.BillingMonths, franchiseFeeInformation.StartDate, franchiseFeeInformation.EndDate, CoverageFrom, CoverageTo, "Franchise");
                    var lastid = "0";
                    IEnumerable<Balances> ISub;
                    ISub = db.Balances.Where(k => k.CompanyId == franchiseFeeInformation.CompanyId && k.BillingType == "FRANCHISE" && k.BillingReference == franchiseFeeInformation.FranchiseFeeInformationId.ToString()).ToList();

                    if (ISub.Count() > 0)
                        lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                    //--------------Start Changes 1-3-2017
                    bool isValid = false;
                    int multiplier = 0;

                    isValid = validateServicesInformations.isValidInformationFranchise();
                    multiplier = validateServicesInformations.multiplier;

                    if (isValid == false) //If not valid service information. Save only the balance and compute interest. Make multiplier 0.
                    {
                        PrincipalBalance FranchisePrincipal = new PrincipalBalance();
                        var franchisePrincipalBalance = FranchisePrincipal.Balance("FRANCHISE", franchiseFeeInformation.FranchiseFeeInformationId.ToString(), franchiseFeeInformation.CompanyId, int.Parse(lastid));

                        //InterestComputation ValidateInterestComputation = new InterestComputation(CoverageFrom, BillingDate, franchiseFeeInformation.CompanyId, franchiseFeeInformation.FranchiseFeeInformationId.ToString());
                        if (franchisePrincipalBalance > 0)
                        {
                            isValid = true;
                            multiplier = 0;
                        }

                    }
                    //---------End Changes 1-3-2017   

                    if (isValid)
                    {
                        GeneralBilling localGeneralBilling = new GeneralBilling();
                        GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                        SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                        Balances localBalances = new Balances();
                        Balances localBalancesAdvance = new Balances();
                        SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                        VatValidation vat = new VatValidation(franchiseFeeInformation.CompanyId);
                        ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                        ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                        //var FranchiseBillingNum = db.GeneralBilling.Where(m => m.CompanyId == franchiseFeeInformation.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                        var FranchiseBillingNum = generateBillingNumber(franchiseFeeInformation.CompanyId);



                        //Start Using new computation 02-24-17
                        //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, franchiseFeeInformation.CompanyId, franchiseFeeInformation.FranchiseFeeInformationId.ToString());
                        PrincipalBalance FranchisePrincipal = new PrincipalBalance();
                        VatBalance FranchiseVAT = new VatBalance();
                        InterestBalance FranchiseInterestBalance = new InterestBalance();
                        InterestWithoutBillingPeriod FranchiseInterest = new InterestWithoutBillingPeriod();
                        var franchisePrincipalBalance = FranchisePrincipal.Balance("FRANCHISE", franchiseFeeInformation.FranchiseFeeInformationId.ToString(), franchiseFeeInformation.CompanyId, int.Parse(lastid));
                        var franchisePrincipalInterest = franchisePrincipalBalance > 0 ? FranchiseInterest.Interest("FRANCHISE", franchiseFeeInformation.CompanyId, franchiseFeeInformation.FranchiseFeeInformationId.ToString(), CoverageFrom, BillingDate, franchisePrincipalBalance, franchisePrincipalBalance, lastid, franchiseFeeInformation.Amount) : 0;
                        var franchiseVatBalance = FranchiseVAT.Balance("FRANCHISE", franchiseFeeInformation.FranchiseFeeInformationId.ToString(), franchiseFeeInformation.CompanyId, int.Parse(lastid));
                        var franchiseInterest = FranchiseInterestBalance.Balance("FRANCHISE", franchiseFeeInformation.FranchiseFeeInformationId.ToString(), franchiseFeeInformation.CompanyId, int.Parse(lastid));
                        //End Using new computation 02-24-17

                        //FranchiseAmount = interestComputation.Franchise > 0 ? franchiseFeeInformation.Amount * interestComputation.interest : franchiseFeeInformation.Amount;
                        localGeneralBilling.CompanyId = franchiseFeeInformation.CompanyId;
                        localGeneralBilling.BillingType = "FRANCHISE";//Franchise
                        localGeneralBilling.BillingReference = franchiseFeeInformation.FranchiseFeeInformationId.ToString();
                        localGeneralBilling.BillingAmount = franchiseFeeInformation.Amount * multiplier;
                        localGeneralBilling.Currency = "PHP";
                        localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(FranchiseBillingNum));

                        if (franchisePrincipalBalance < 0)
                        {
                            ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                            localGeneralBillingAdvance.BillingAmount = Math.Abs(franchisePrincipalInterest);
                            localGeneralBillingAdvance.TransactionType = "Advance";
                            db.GeneralBilling.Add(localGeneralBillingAdvance);
                        }

                        localSubsidiary.BillingType = "FRANCHISE";
                        localSubsidiary.CompanyId = franchiseFeeInformation.CompanyId;
                        localSubsidiary.BillingReference = franchiseFeeInformation.FranchiseFeeInformationId.ToString();
                        localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(FranchiseBillingNum);
                        localSubsidiary.Currency = "PHP";
                        localSubsidiary.DebitAmount = franchiseFeeInformation.Amount * multiplier;
                        localSubsidiary.DollarAmount = 0;


                        ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, franchisePrincipalBalance, franchiseInterest, franchiseVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                        db.Balances.Add(localBalancesAdvance);

                        ClassAssignment.AssignBalance(localSubsidiary, localBalances, franchisePrincipalBalance, franchiseInterest, franchiseVatBalance, lastid.ToString());
                        if (franchisePrincipalBalance > 0)
                        {
                            ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                            localInterestSubsidiary.DebitAmount = franchisePrincipalInterest;
                            db.SubsidiaryLedger.Add(localInterestSubsidiary);
                        }

                        if (vat.hasVat())
                        {
                            ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                            db.SubsidiaryLedger.Add(localVatSubsidiary);
                        }

                        FranchiseFeeInformation info = new FranchiseFeeInformation();
                        info = db.FranchiseFeeInformation.Find(franchiseFeeInformation.FranchiseFeeInformationId);

                        if (isgenerated.ToUpper() == "NO")
                            info.BillingStatus = "Billed";
                        else
                            info.BillingStatus = "Regenerated";

                        db.Entry(info).State = System.Data.Entity.EntityState.Modified;

                        db.GeneralBilling.Add(localGeneralBilling);
                        db.SubsidiaryLedger.Add(localSubsidiary);
                        localBalances.PeriodDateFrom = PeriodDateFrom;
                        localBalances.PeriodDateTo = PeriodDateTo;
                        //localBalances.VAT = localSubsidiary.DebitAmount * 0.12M;
                        localBalances.DueDate = BillingDue;
                        db.Balances.Add(localBalances);


                        Balances vatBalance = new Balances();
                        Balances intBalance = new Balances();
                        ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                        ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                        db.Balances.Add(vatBalance);
                        db.Balances.Add(intBalance);
                        //bCSAgingOutput = new BCSAgingOutput();
                        //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                        //db.BCSAgingOutput.Add(bCSAgingOutput);
                    }
                }//End of foreach

                //---------------END FRANCHISE----------------------------------------------------------------

                //---------------START GARBAGE----------------------------------------------------------------

                decimal GarbageAmount = 0;

                foreach (var garbageInformation in GarbageInformations)
                {
                    var lastid = "0";
                    IEnumerable<Balances> ISub;
                    //ISub = db.SubsidiaryLedger.Where(k => k.CompanyId == garbageInformation.CompanyId && k.BillingType == "GARBAGE" && k.TransactionType == "BILLING").ToList();
                    ISub = db.Balances.Where(k => k.CompanyId == garbageInformation.CompanyId && k.BillingType == "GARBAGE" && k.TransactionType == "BILLING").ToList();

                    if (ISub.Count() > 0)
                    {
                        lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();
                    }

                    //--------------Start Changes 1-3-2017
                    bool isValid = true;
                    int multiplier = 1;
                    //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, garbageInformation.CompanyId, garbageInformation.GarbageInformationId.ToString());
                    if (garbageInformation.BillingPeriod != billingPeriodId)
                    {
                        PrincipalBalanceBillingPeriodBased GarbagePrincipal = new PrincipalBalanceBillingPeriodBased();
                        var garbagePrincipalBalance = GarbagePrincipal.Balance("GARBAGE", garbageInformation.CompanyId, int.Parse(lastid));
                        if (garbagePrincipalBalance > 0)
                        {
                            isValid = true;
                            multiplier = 0;
                        }
                        else
                        {
                            multiplier = 0;
                            isValid = true;
                        }
                    }
                    //---------End Changes 1-3-2017   

                    if (isValid)
                    {
                        GeneralBilling localGeneralBilling = new GeneralBilling();
                        GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                        SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                        Balances localBalances = new Balances();
                        Balances localBalancesAdvance = new Balances();
                        SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                        VatValidation vat = new VatValidation(garbageInformation.CompanyId);
                        ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                        ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                        //var GarbageBillingNum = db.GeneralBilling.Where(m => m.CompanyId == garbageInformation.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                        var GarbageBillingNum = generateBillingNumber(garbageInformation.CompanyId);

                        //Start Using new computation 02-24-17
                        //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, garbageInformation.CompanyId, garbageInformation.GarbageInformationId.ToString());
                        PrincipalBalanceBillingPeriodBased GarbagePrincipal = new PrincipalBalanceBillingPeriodBased();
                        VatBalanceBillingPeriodBased GarbageVAT = new VatBalanceBillingPeriodBased();
                        InterestBalanceBillingPeriodBased GarbageInterestBalance = new InterestBalanceBillingPeriodBased();
                        InterestWithBillingPeriod GarbageInterest = new InterestWithBillingPeriod();
                        var garbagePrincipalBalance = GarbagePrincipal.Balance("GARBAGE", garbageInformation.CompanyId, int.Parse(lastid));
                        var garbagePrincipalInterest = garbagePrincipalBalance > 0 ? GarbageInterest.Interest("GARBAGE", garbageInformation.CompanyId, garbageInformation.GarbageInformationId.ToString(), CoverageFrom, BillingDate, garbagePrincipalBalance, lastid, (garbageInformation.Rate * garbageInformation.Weight)) : 0;
                        var garbageVatBalance = GarbageVAT.Balance("GARBAGE", garbageInformation.CompanyId, int.Parse(lastid));
                        var garbageInterest = GarbageInterestBalance.Balance("GARBAGE", garbageInformation.CompanyId, int.Parse(lastid));
                        //End Using new computation 02-24-17

                        //GarbageAmount = interestComputation.Garbage > 0 ? (garbageInformation.Rate * garbageInformation.Weight) * interestComputation.interest : garbageInformation.Rate * garbageInformation.Weight;
                        localGeneralBilling.CompanyId = garbageInformation.CompanyId;
                        localGeneralBilling.BillingType = "GARBAGE";//Garbage
                        localGeneralBilling.BillingReference = garbageInformation.GarbageInformationId.ToString();
                        localGeneralBilling.BillingAmount = (garbageInformation.Rate * garbageInformation.Weight) * multiplier;
                        localGeneralBilling.Currency = "PHP";
                        localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(GarbageBillingNum));

                        if (garbagePrincipalBalance < 0)
                        {
                            ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                            localGeneralBillingAdvance.BillingAmount = Math.Abs(garbagePrincipalInterest);
                            localGeneralBillingAdvance.TransactionType = "Advance";
                            db.GeneralBilling.Add(localGeneralBillingAdvance);
                        }

                        localSubsidiary.BillingType = "GARBAGE";
                        localSubsidiary.CompanyId = garbageInformation.CompanyId;
                        localSubsidiary.BillingReference = garbageInformation.GarbageInformationId.ToString();
                        localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(GarbageBillingNum);
                        localSubsidiary.Currency = "PHP";
                        localSubsidiary.DebitAmount = (garbageInformation.Rate * garbageInformation.Weight) * multiplier;
                        localSubsidiary.DollarAmount = 0;


                        ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, garbagePrincipalBalance, garbageInterest, garbageVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                        db.Balances.Add(localBalancesAdvance);


                        ClassAssignment.AssignBalance(localSubsidiary, localBalances, garbagePrincipalBalance, garbageInterest, garbageVatBalance, lastid.ToString());
                        if (garbagePrincipalBalance > 0)
                        {
                            ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                            localInterestSubsidiary.DebitAmount = garbagePrincipalInterest;
                            db.SubsidiaryLedger.Add(localInterestSubsidiary);
                        }

                        if (vat.hasVat())
                        {
                            ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                            db.SubsidiaryLedger.Add(localVatSubsidiary);
                        }

                        GarbageInformation info = new GarbageInformation();
                        info = db.GarbageInformations.Find(garbageInformation.GarbageInformationId);

                        if (isgenerated.ToUpper() == "NO")
                            info.BillingStatus = "Billed";
                        else
                            info.BillingStatus = "Regenerated";

                        db.Entry(info).State = System.Data.Entity.EntityState.Modified;

                        db.GeneralBilling.Add(localGeneralBilling);
                        db.SubsidiaryLedger.Add(localSubsidiary);

                        localBalances.PeriodDateFrom = PeriodDateFrom;
                        localBalances.PeriodDateTo = PeriodDateTo;
                        //localBalances.VAT = localSubsidiary.DebitAmount * 0.12M;
                        localBalances.DueDate = BillingDue;
                        db.Balances.Add(localBalances);


                        Balances vatBalance = new Balances();
                        ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                        db.Balances.Add(vatBalance);

                        if (localBalances.Interest >= 0)
                        {
                            Balances intBalance = new Balances();
                            ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                            db.Balances.Add(intBalance);
                        }

                        //bCSAgingOutput = new BCSAgingOutput();
                        //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                        //db.BCSAgingOutput.Add(bCSAgingOutput);
                    }
                }

                //---------------END GARBAGE----------------------------------------------------------------

                //---------------START PASSED ON BILLING----------------------------------------------------------------

                decimal SecurityGuardAmount = 0;

                foreach (var passedonbillingInformation in PassedOnBillingInformations)
                {
                    if (passedonbillingInformation.CompanyId == 3180 && passedonbillingInformation.Type == "System Loss")
                    {

                    }
                    //--------------Start Changes 1-3-2017
                    bool isValid = true;
                    int multiplier = 1;

                    var lastid = "0";
                    IEnumerable<Balances> ISub;
                    //ISub = db.SubsidiaryLedger.Where(k => k.CompanyId == passedonbillingInformation.CompanyId && k.BillingType == "PASSED ON BILLING" && k.Other.ToUpper() == passedonbillingInformation.Type.ToUpper()).ToList();
                    ISub = db.Balances.Where(k => k.CompanyId == passedonbillingInformation.CompanyId && k.BillingType.ToUpper() == "PASSED ON BILLING" && k.BillingReference.ToUpper() == passedonbillingInformation.Type.ToUpper()).ToList();

                    if (ISub.Count() > 0)
                        lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                    //Start Using new computation 02-24-17
                    //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, passedonbillingInformation.CompanyId, passedonbillingInformation.Type);
                    PrincipalBalancePOB POBPrincipal = new PrincipalBalancePOB();
                    VatBalancePOB POBVAT = new VatBalancePOB();
                    InterestBalancePOB POBInterestBalance = new InterestBalancePOB();
                    //InterestWithBillingPeriod POBInterest = new InterestWithBillingPeriod();
                    var pobPrincipalBalance = POBPrincipal.Balance("PASSED ON BILLING", passedonbillingInformation.Type, passedonbillingInformation.CompanyId, int.Parse(lastid));
                    var pobPrincipalInterest = 0;//pobPrincipalBalance > 0 ? POBInterest.Interest("PASSED ON BILLING", passedonbillingInformation.CompanyId, passedonbillingInformation.PassedOnBillingInformationId.ToString(), CoverageFrom, BillingDate, pobPrincipalBalance) : 0;
                    var pobVatBalance = POBVAT.Balance("PASSED ON BILLING", passedonbillingInformation.Type, passedonbillingInformation.CompanyId, int.Parse(lastid));
                    var pobInterest = POBInterestBalance.Balance("PASSED ON BILLING", passedonbillingInformation.Type, passedonbillingInformation.CompanyId, int.Parse(lastid));
                    //End Using new computation 02-24-17

                    if (passedonbillingInformation.BillingPeriod != billingPeriodId)
                    {
                        if (pobPrincipalBalance > 0)
                        {
                            //No principal, Has balance
                            isValid = true;
                            multiplier = 0;
                        }
                        else
                        {
                            //No principal, Has overpayment
                            isValid = true;
                            multiplier = 0;
                        }
                    }
                    //---------End Changes 1-3-2017   

                    if (isValid)
                    {
                        GeneralBilling localGeneralBilling = new GeneralBilling();
                        GeneralBilling localGeneralBillingAdvance = new GeneralBilling();
                        SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                        Balances localBalances = new Balances();
                        Balances localBalancesAdvance = new Balances();
                        SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                        VatValidation vat = new VatValidation(passedonbillingInformation.CompanyId);
                        ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                        ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);
                        //var SecurityGuardBillingNum = db.GeneralBilling.Where(m => m.CompanyId == securityGuardFeeInformation.CompanyId).Max(m => (int?)m.BillingNumber) ?? 0;
                        var PassedOnBillingBillingNum = generateBillingNumber(passedonbillingInformation.CompanyId);

                        //SecurityGuardAmount = interestComputation.PassedOnBilling > 0 ? passedonbillingInformation.Amount * interestComputation.interest : passedonbillingInformation.Amount;
                        localGeneralBilling.CompanyId = passedonbillingInformation.CompanyId;
                        localGeneralBilling.BillingType = "PASSED ON BILLING";//passed on billing
                        localGeneralBilling.BillingReference = passedonbillingInformation.PassedOnBillingInformationId.ToString();
                        localGeneralBilling.BillingAmount = passedonbillingInformation.Amount * multiplier;
                        localGeneralBilling.Currency = "PHP";
                        localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(PassedOnBillingBillingNum));

                        if (pobPrincipalBalance > 0)
                        {
                            ClassAssignment.CompleteGeneralLedger(localGeneralBilling, localGeneralBillingAdvance);
                            localGeneralBillingAdvance.BillingAmount = Math.Abs(pobPrincipalInterest);
                            localGeneralBillingAdvance.TransactionType = "Advance";
                            db.GeneralBilling.Add(localGeneralBillingAdvance);
                        }

                        localSubsidiary.BillingType = "PASSED ON BILLING";
                        localSubsidiary.CompanyId = passedonbillingInformation.CompanyId;
                        localSubsidiary.BillingReference = passedonbillingInformation.PassedOnBillingInformationId.ToString();
                        localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(PassedOnBillingBillingNum);
                        localSubsidiary.Currency = "PHP";
                        localSubsidiary.DebitAmount = passedonbillingInformation.Amount * multiplier;
                        localSubsidiary.DollarAmount = 0;
                        localSubsidiary.Other = passedonbillingInformation.Type;


                        ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, pobPrincipalBalance, pobInterest, pobVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                        localBalancesAdvance.BillingReference = passedonbillingInformation.Type;
                        db.Balances.Add(localBalancesAdvance);


                        ClassAssignment.AssignBalance(localSubsidiary, localBalances, pobPrincipalBalance, pobInterest, pobVatBalance, lastid.ToString());
                        //if (interestComputation.PassedOnBilling > 0)
                        //{
                        //    ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                        //    localInterestSubsidiary.DebitAmount = interestComputation.interest;
                        //    db.SubsidiaryLedger.Add(localInterestSubsidiary);
                        //}

                        if (vat.hasVat())
                        {
                            ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                            db.SubsidiaryLedger.Add(localVatSubsidiary);
                        }

                        PassedOnBillingInformation info = new PassedOnBillingInformation();
                        info = db.PassedOnBillingInformation.Find(passedonbillingInformation.PassedOnBillingInformationId);

                        if (isgenerated.ToUpper() == "NO")
                            info.BillingStatus = "Billed";
                        else
                            info.BillingStatus = "Regenerated";

                        db.Entry(info).State = System.Data.Entity.EntityState.Modified;

                        db.GeneralBilling.Add(localGeneralBilling);
                        db.SubsidiaryLedger.Add(localSubsidiary);

                        localBalances.PeriodDateFrom = PeriodDateFrom;
                        localBalances.PeriodDateTo = PeriodDateTo;
                        //localBalances.VAT = localSubsidiary.DebitAmount * 0.12M;
                        localBalances.DueDate = BillingDue;
                        localBalances.BillingReference = passedonbillingInformation.Type;
                        db.Balances.Add(localBalances);


                        Balances vatBalance = new Balances();
                        //Balances intBalance = new Balances();
                        ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                        //ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                        vatBalance.BillingReference = passedonbillingInformation.Type;
                        db.Balances.Add(vatBalance);
                        //db.Balances.Add(intBalance);
                        //bCSAgingOutput = new BCSAgingOutput();
                        //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                        //db.BCSAgingOutput.Add(bCSAgingOutput);
                    }
                }
                //---------------END PASSED ON BILLING----------------------------------------------------------------


                //---------------START ADMIN FEE----------------------------------------------------------------

                //*********************************************************************************************************************************************************************
                //*************************Note:Computation of admin fee is per billing period... While uploading of admin fee is per Zonegroup of logged user*************************
                //*********************************************************************************************************************************************************************

                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(GroupName);
                List<Company> adminFeeCompanies = new List<Company>();
                adminFeeCompanies = searchCompanyPerGroup.Companies;

                decimal AdminFeeAmount = 3750;
                List<DeveloperClass> ListdeveloperClass = new List<DeveloperClass>();
                List<AdminFee> AdminFees = new List<AdminFee>();
                List<TempAdminFeeData> TempAdminFeeList = new List<TempAdminFeeData>();
                AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == billingPeriodId).ToList();
                //if (AdminFees.Count > 0)
                //{                    
                //Get the adminfees with balance
                var adminfeeBalances = db.Balances.Where(m => m.TransactionType == "BILLING").Where(m => m.BillingType == "ADMIN FEE").Where(m => m.BillingSubType == "BALANCE").Where(m => m.Amount > 0).ToList().Select(k => new { k.BillingReference, k.CompanyId, k.CompCode }).Distinct().ToList();

                var adminfeeBalancesTemp = db.Balances.Where(m => m.TransactionType == "BILLING").Where(m => m.BillingType == "ADMIN FEE").Where(m => m.BillingSubType == "BALANCE").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList().Select(x => new { x.BillingReference, x.CompanyId, x.CompCode }).Distinct().ToList();

                //consolidate both balances from different origin
                foreach (var item in adminfeeBalancesTemp)
                {
                    var isExist = adminfeeBalances.Where(m => m.BillingReference == item.BillingReference).Where(m => m.CompanyId == item.CompanyId).Where(m => m.CompCode == item.CompCode).FirstOrDefault();
                    if (isExist == null)
                    {
                        var tempItem = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference == item.BillingReference).Where(m => m.CompCode == item.CompCode)
                        .Where(m => m.TransactionType == "BILLING").Where(m => m.BillingType == "ADMIN FEE").Where(m => m.BillingSubType == "BALANCE").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();

                        if (tempItem.Count == 1)
                        {
                            adminfeeBalances.Add(item);
                        }
                    }
                }
                //Add the adminfees with balance in the loop
                foreach (var item in adminfeeBalances)
                {
                    TempAdminFeeData temp = new TempAdminFeeData();
                    temp.CompanyId = item.CompanyId;
                    temp.Dev_Comp_Code = item.CompCode;
                    temp.Zone_Code = item.BillingReference;
                    TempAdminFeeList.Add(temp);
                    try
                    {
                        var tempAdminFee = db.AdminFee.Where(m => m.Zone_Code == item.BillingReference).Where(m => m.Dev_Comp_Code == item.CompCode).FirstOrDefault();
                        if (!AdminFees.Any(m => m.Dev_Comp_Code == tempAdminFee.Dev_Comp_Code && m.Zone_Code == tempAdminFee.Zone_Code))
                        {
                            if (adminFeeCompanies.Any(m => m.CompanyID == item.CompanyId))
                                AdminFees.Add(tempAdminFee);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
                //}

                if (TypeOfBillingGenerate.ToUpper() == "PERCOMPANY" || TypeOfBillingGenerate.ToUpper() == "PERADMINFEE")
                {
                    var compcode = db.Company.FirstOrDefault(m => m.CompanyID == generatePerCompanyId).CompanyCode;
                    if (TypeOfBillingGenerateValue == "AFITPark")
                    {
                        AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == billingPeriodId).Where(m => m.Dev_Comp_Code == compcode).ToList();
                        AdminFeeEnterpriseTypeBLL adminFeeEnterpriseTypeBLL = new AdminFeeEnterpriseTypeBLL(AdminFees, "IT");
                        AdminFees = adminFeeEnterpriseTypeBLL.AdminFeePerEnterprise();
                    }
                    else if (TypeOfBillingGenerateValue == "AFManufacturing")
                    {
                        AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == billingPeriodId).Where(m => m.Dev_Comp_Code == compcode).ToList();
                        AdminFeeEnterpriseTypeBLL adminFeeEnterpriseTypeBLL = new AdminFeeEnterpriseTypeBLL(AdminFees, "SEZ");
                        AdminFees = adminFeeEnterpriseTypeBLL.AdminFeePerEnterprise();
                    }
                    else if (TypeOfBillingGenerateValue == "AFOthers")
                    {
                        AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == billingPeriodId).Where(m => m.Dev_Comp_Code == compcode).ToList();
                        AdminFeeEnterpriseTypeBLL adminFeeEnterpriseTypeBLL = new AdminFeeEnterpriseTypeBLL(AdminFees, "OTHERS");
                        AdminFees = adminFeeEnterpriseTypeBLL.AdminFeePerEnterprise();
                    }
                    //else if (TypeOfBillingGenerateValue == "All")
                    //{
                    //    AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == billingPeriodId).Where(m => m.Dev_Comp_Code == compcode).ToList();
                    //}
                    //else
                    //    AdminFees.RemoveRange(0, AdminFees.Count);
                }
                else if (TypeOfBillingGenerate.ToUpper() == "PERBILLINGTYPE")
                {                    
                    AdminFees.RemoveRange(0, AdminFees.Count);
                }
                else if (TypeOfBillingGenerate.ToUpper() == "PERPOB")
                {
                    AdminFees.RemoveRange(0, AdminFees.Count);
                }




                var a = AdminFees.Select(x => new { x.Dev_Comp_Code, x.Zone_Code }).GroupBy(n => new { n.Zone_Code, n.Dev_Comp_Code }).ToList();
                //get selected column in db (will return multi dimension array)
                foreach (var item in a) //transfer to model
                {
                    DeveloperClass dev = new DeveloperClass();
                    string dev_compCode = item.Select(m => m.Dev_Comp_Code).FirstOrDefault();
                    dev.Developer = AdminFees.FirstOrDefault(m => m.Dev_Comp_Code == dev_compCode).Developer;
                    dev.Dev_Comp_Code = item.Select(m => m.Dev_Comp_Code).FirstOrDefault();
                    dev.Comp_Code = AdminFees.FirstOrDefault(m => m.Dev_Comp_Code == dev_compCode).Comp_Code;
                    dev.Zone_Code = item.Select(m => m.Zone_Code).FirstOrDefault();
                    ListdeveloperClass.Add(dev);
                }
                a = null;

                //var adminFees = AdminFees.Select(m => new { m.Comp_Code, m.Developer, m.Dev_Comp_Code, m.Zone_Type, m.Employment }).ToList(); //compute amount per developer (1 developer -> many billing, reason to loop each record)

                foreach (var item in AdminFees)
                {

                    string[] splitZonetype = item.Zone_Type.Split(' ');
                    CheckITEnterpriseAdminFee checkITEnterpriseAdminFee = new CheckITEnterpriseAdminFee(splitZonetype);
                    string zoneType = "";
                    string zoneNature = "";
                    try
                    {
                        zoneType = db.Zone.FirstOrDefault(m => m.ZoneCode == item.Zone_Code).ZoneType == null ? "" : db.Zone.FirstOrDefault(m => m.ZoneCode == item.Zone_Code).ZoneType;
                        zoneNature = db.Zone.FirstOrDefault(m => m.ZoneCode == item.Zone_Code).ZoneNature == null ? "" : db.Zone.FirstOrDefault(m => m.ZoneCode == item.Zone_Code).ZoneNature;
                    }
                    catch (Exception)
                    {
                    }

                    string newEmployment = "0";
                    try
                    {
                        string[] splitEmployment = item.Employment.Split(',');
                        //string newEmployment = "";
                        foreach (var emp in splitEmployment)
                        {
                            newEmployment += emp;
                        }
                    }
                    catch (Exception)
                    {
                    }


                    if (checkITEnterpriseAdminFee.HasITWord())

                        //06212017 - changes - jc - ian 

                        //paul code                 
                        //AdminFeeAmount = int.Parse(newEmployment) >= 500 ? 3000 : 1000;

                        //jc code
                        // AdminFeeAmount = int.Parse(newEmployment) == 0 ? 0 : int.Parse(newEmployment) >= 500 ? 3000 : 1000;


                        //ian basic code
                        if (int.Parse(newEmployment) == 0 || newEmployment == null || newEmployment == "")
                        {
                            AdminFeeAmount = 0;
                        }
                        else
                        {
                            AdminFeeAmount = int.Parse(newEmployment) >= 500 ? 3000 : 1000;
                        }

                    //AdminFeeAmount = int.Parse(newEmployment) < 1 ? 0 : int.Parse(newEmployment) <= 500 && int.Parse(newEmployment) >= 1 ? 1000 : 3000;

                    if (zoneType.ToUpper() == "PRIVATE ECONOMIC ZONE")
                        AdminFeeAmount = 3750;

                    if (zoneNature.ToUpper() == "MANUFACTURING SEZ" || zoneNature.ToUpper() == "AGRO-INDUSTRIAL ECOZONE" || zoneNature.ToUpper() == "TOURISM SEZ")
                        AdminFeeAmount = 3750;


                    int indexOfDeveloper = ListdeveloperClass.FindIndex(m => m.Dev_Comp_Code == item.Dev_Comp_Code && m.Zone_Code == item.Zone_Code);
                    //locate and save/add amount to developer..
                    ListdeveloperClass[indexOfDeveloper].Amount += AdminFeeAmount;

                }
                AdminFees = null;

                //Start of transaction-------------------------------------------------------------------------------------------------------------------
                foreach (var item in ListdeveloperClass)
                {
                    bool companyCodeExist = false;
                    bool fromBalanceTable = true;
                    int CompanyId = 0;

                    //Check if company exist. If false do not process
                    try
                    {
                        CompanyId = db.Company.SingleOrDefault(m => m.CompanyCode == item.Dev_Comp_Code).CompanyID; //Developer is also a member of a company
                        companyCodeExist = true;
                    }
                    catch (Exception)
                    {
                        companyCodeExist = false;
                    }

                    //Check if company is from balances. if true zero the principal amount. only balances and compute interest
                    try
                    {
                        var checkIfExist = db.AdminFee.Where(m => m.Dev_Comp_Code == item.Dev_Comp_Code && m.Zone_Code == item.Zone_Code && m.BillingPeriodId == billingPeriodId).FirstOrDefault();
                        if (checkIfExist != null)
                            fromBalanceTable = false;
                    }
                    catch (Exception)
                    {
                    }

                    if (companyCodeExist)
                    {

                        GeneralBilling localGeneralBilling = new GeneralBilling();
                        SubsidiaryLedger localSubsidiary = new SubsidiaryLedger();
                        SubsidiaryLedger localInterestSubsidiary = new SubsidiaryLedger();
                        Balances localBalances = new Balances();
                        Balances localBalancesAdvance = new Balances();
                        SubsidiaryLedger localVatSubsidiary = new SubsidiaryLedger();
                        VatValidation vat = new VatValidation(CompanyId);
                        ClassAssignment.AssignGeneralLedger(MasterGeneralBilling, localGeneralBilling);
                        ClassAssignment.AssignSubsidiaryLedger(MasterSubsidiaryLedger, localSubsidiary);

                        //************************************************************************************
                        //Changes 04/18/2017 As per Sir Rufel. Separate the reference number of Adminfee
                        //var AdminFeeBillingNum = generateBillingNumber(CompanyId);
                        int AdminFeeBillingNum = MaxId++;
                        //************************************************************************************

                        var lastid = "0";
                        IEnumerable<Balances> ISub;
                        ISub = db.Balances.Where(k => k.CompanyId == CompanyId && k.BillingType == "ADMIN FEE" && k.BillingReference == item.Zone_Code && k.CompCode == item.Dev_Comp_Code).ToList();

                        if (ISub.Count() > 0)
                            lastid = ISub.Max(m => m.CurrentBillingPeriod).ToString();

                        //Admin Fee mapping
                        //Balances.BillingReference == Dev_Comp_Code
                        //Balances.Comp_Code == Zone_Code

                        //Sub.BillingReference == Dev_Comp_Code
                        //Sub.Other == ZoneCode

                        //Start Using new computation 02-24-17
                        //InterestComputation interestComputation = new InterestComputation(CoverageFrom, BillingDate, CompanyId);
                        PrincipalBalanceAdminFee AdminFeePrincipal = new PrincipalBalanceAdminFee();
                        VatBalanceAdminFee AdminFeeVAT = new VatBalanceAdminFee();
                        InterestBalanceAdminFee AdminFeeInterestBalance = new InterestBalanceAdminFee();
                        InterestAdminFee AdminFeeInterest = new InterestAdminFee();
                        var adminFeePrincipalBalance = AdminFeePrincipal.Balance("ADMIN FEE", item.Dev_Comp_Code, item.Zone_Code, int.Parse(lastid));
                        var adminFeePrincipalInterest = adminFeePrincipalBalance > 0 ? AdminFeeInterest.Interest("ADMIN FEE", CompanyId, item.Dev_Comp_Code, item.Zone_Code, CoverageFrom, BillingDate, adminFeePrincipalBalance) : 0;
                        var adminFeeVatBalance = AdminFeeVAT.Balance("ADMIN FEE", item.Dev_Comp_Code, item.Zone_Code, int.Parse(lastid));
                        var adminFeeInterest = AdminFeeInterestBalance.Balance("ADMIN FEE", item.Dev_Comp_Code, item.Zone_Code, int.Parse(lastid));
                        //End Using new computation 02-24-17

                        decimal adminfeeamount = fromBalanceTable == true ? 0 : item.Amount;

                        localGeneralBilling.CompanyId = CompanyId;
                        localGeneralBilling.BillingType = "ADMIN FEE";//Admin Fee
                        localGeneralBilling.BillingReference = item.Dev_Comp_Code;
                        localGeneralBilling.BillingAmount = adminfeeamount;
                        localGeneralBilling.Currency = "PHP";
                        localGeneralBilling.BillingNumber = Convert.ToInt32(DateTime.Now.ToString("yy") + padBillingNumber(AdminFeeBillingNum));

                        localSubsidiary.BillingType = "ADMIN FEE";
                        localSubsidiary.CompanyId = CompanyId;
                        localSubsidiary.BillingReference = item.Dev_Comp_Code;
                        localSubsidiary.Other = item.Zone_Code;
                        localSubsidiary.TransactionReference = "1" + GroupName + DateTime.Now.ToString("yy") + padBillingNumber(AdminFeeBillingNum);
                        localSubsidiary.Currency = "PHP";
                        localSubsidiary.DebitAmount = adminfeeamount;
                        localSubsidiary.DollarAmount = 0;


                        ClassAssignment.AssignBalanceAdvance(localSubsidiary, localBalancesAdvance, adminFeePrincipalBalance, adminFeeInterest, adminFeeVatBalance, lastid.ToString(), PeriodDateFrom, PeriodDateTo);
                        localBalancesAdvance.BillingReference = item.Zone_Code;
                        localBalancesAdvance.CompCode = item.Dev_Comp_Code;
                        db.Balances.Add(localBalancesAdvance);


                        ClassAssignment.AssignBalance(localSubsidiary, localBalances, adminFeePrincipalBalance, adminFeeInterest, adminFeeVatBalance, lastid.ToString());

                        if (adminFeePrincipalBalance > 0)
                        {
                            ClassAssignment.AssignInterest(localSubsidiary, localInterestSubsidiary);
                            localInterestSubsidiary.DebitAmount = adminFeePrincipalInterest;
                            //localGeneralBillingAdvance.TransactionType = "Advance";
                            db.SubsidiaryLedger.Add(localInterestSubsidiary);
                        }

                        if (vat.hasVat())
                        {
                            ClassAssignment.AssignVat(localSubsidiary, localVatSubsidiary);
                            db.SubsidiaryLedger.Add(localVatSubsidiary);
                        }
                        //////////////////////////////////////////////////////////////////////////////////////////////////////
                        db.GeneralBilling.Add(localGeneralBilling);
                        db.SubsidiaryLedger.Add(localSubsidiary);

                        localBalances.PeriodDateFrom = PeriodDateFrom;
                        localBalances.PeriodDateTo = PeriodDateTo;
                        //localBalances.VAT = localSubsidiary.DebitAmount * 0.12M;
                        localBalances.DueDate = BillingDue;
                        localBalances.BillingReference = item.Zone_Code;
                        localBalances.CompCode = item.Dev_Comp_Code;
                        db.Balances.Add(localBalances);


                        Balances vatBalance = new Balances();
                        Balances intBalance = new Balances();
                        ClassAssignment.AssignBalanceCompleteVat(localBalances, vatBalance);
                        ClassAssignment.AssignBalanceCompleteInterest(localBalances, intBalance);
                        db.Balances.Add(vatBalance);
                        db.Balances.Add(intBalance);

                        //bCSAgingOutput = new BCSAgingOutput();
                        //ClassAssignment.AssignBalanceComplete(localBalances, bCSAgingOutput);
                        //db.BCSAgingOutput.Add(bCSAgingOutput);
                    }
                }

                //---------------END ADMIN FEE----------------------------------------------------------------


                BillingPeriod newbilling = new BillingPeriod();
                newbilling = db.BillingPeriod.Single(m => m.BillingPeriodId == billingPeriodId);
                newbilling.Generated = "YES";
                newbilling.BillingDate = BillingDate;
                newbilling.DueDate = BillingDue;
                newbilling.IsPaymentOpen = true;
                db.Entry(newbilling).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                dbtransaction.Commit();

            }
        }

        public class DeveloperClass
        {
            public string Developer { get; set; }
            public string Dev_Comp_Code { get; set; }
            public decimal Amount { get; set; }
            public string Comp_Code { get; set; }
            public string Zone_Code { get; set; }
        }

        public class TempAdminFeeData
        {
            public string Zone_Code { get; set; }
            public int CompanyId { get; set; }
            public string Dev_Comp_Code { get; set; }
        }

    }
}