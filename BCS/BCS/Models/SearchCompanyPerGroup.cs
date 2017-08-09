using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections;

namespace BCS.Models
{
    //Will return informations

    //Company gourp = within user group
    //Company status = ACTIVE
    //Informations = Within coverage date
    public class SearchCompanyPerGroup
    {
        BCS_Context db = new BCS_Context();
        private string GroupName;
        private int billingPeriodId;
        DateTime CoverageFrom;
        DateTime CoverageTo;
        private List<Company> NewCompany = null;
        private string TypeOfBillingGenerate;
        int generatePerCompanyId;
        string TypeOfBillingGenerateValue;
        //Get only the List of Companies per group. USE FOR PER COMPANY BILLING GENERATE
        public SearchCompanyPerGroup(List<Company> company, string GroupName, DateTime CoverageFrom, DateTime CoverageTo, int billingPeriodId, string TypeOfBillingGenerate, int generatePerCompanyId)
        {
            this.GroupName = GroupName;
            this.NewCompany = company;
            this.CoverageFrom = CoverageFrom;
            this.CoverageTo = CoverageTo;
            this.billingPeriodId = billingPeriodId;
            this.TypeOfBillingGenerate = TypeOfBillingGenerate;
            this.generatePerCompanyId = generatePerCompanyId;
        }

        public SearchCompanyPerGroup(List<Company> company, string GroupName, DateTime CoverageFrom, DateTime CoverageTo, int billingPeriodId, string TypeOfBillingGenerate, int generatePerCompanyId, string TypeOfBillingGenerateValue)
        {
            this.GroupName = GroupName;
            this.NewCompany = company;
            this.CoverageFrom = CoverageFrom;
            this.CoverageTo = CoverageTo;
            this.billingPeriodId = billingPeriodId;
            this.TypeOfBillingGenerate = TypeOfBillingGenerate;
            this.generatePerCompanyId = generatePerCompanyId;
            this.TypeOfBillingGenerateValue = TypeOfBillingGenerateValue;
        }

        public SearchCompanyPerGroup(string GroupName, DateTime CoverageFrom, DateTime CoverageTo, int billingPeriodId, string TypeOfBillingGenerate, string TypeOfBillingGenerateValue)
        {
            this.GroupName = GroupName;
            this.CoverageFrom = CoverageFrom;
            this.CoverageTo = CoverageTo;
            this.billingPeriodId = billingPeriodId;
            this.TypeOfBillingGenerate = TypeOfBillingGenerate;
            this.TypeOfBillingGenerateValue = TypeOfBillingGenerateValue;
        }

        public SearchCompanyPerGroup(List<Company> company, string GroupName) //Get only the List of Companies per group (COMPANIES MODULE)
        {
            this.GroupName = GroupName;
            this.NewCompany = company;
            this.TypeOfBillingGenerate = "ALL";
        }

        public SearchCompanyPerGroup(string GroupName) //Get only the List of Companies per group
        {
            this.GroupName = GroupName;
            this.TypeOfBillingGenerate = "ALL";
        }

        public SearchCompanyPerGroup(string GroupName, DateTime CoverageFrom, DateTime CoverageTo, int billingPeriodId) //Get the list of services information per group
        {
            this.billingPeriodId = billingPeriodId;
            this.GroupName = GroupName;
            this.CoverageFrom = CoverageFrom;
            this.CoverageTo = CoverageTo;
            NewCompany = null;
            this.TypeOfBillingGenerate = "ALL";
        }
        public List<Company> Companies
        {
            get
            {
                if (TypeOfBillingGenerate.ToUpper() == "PERENTERPRISE")
                {
                    List<Company> companies = new List<Company>();
                    //List<Company> concatCompanies = new List<Company>();
                    List<Zone> zones = new List<Zone>();

                    int ZoneGroupId = db.ZoneGroup.Single(m => m.ZoneGroupCode == GroupName).ZoneGroupId;

                    zones = db.Zone.Where(m => m.ZoneGroup == ZoneGroupId.ToString()).ToList();

                    foreach (var item in zones)//we need to loop through each zones. coz companies are mapped thru ZONE CODE
                    {
                        List<Company> comp = new List<Company>();


                        if (NewCompany != null)
                            comp = NewCompany.Where(m => m.ZoneCode == item.ZoneCode).Where(m => m.Status == "Active").Where(m => m.EnterpriseType.ToUpper() == TypeOfBillingGenerateValue.ToUpper()).ToList();//get all companies in zonegroup
                        else
                            comp = db.Company.Where(m => m.ZoneCode == item.ZoneCode).Where(m => m.Status == "Active").Where(m => m.EnterpriseType.ToUpper() == TypeOfBillingGenerateValue.ToUpper()).ToList();//get all companies in zonegroup

                        //concatCompanies = companies.Concat(comp).ToList();
                        foreach (var item2 in comp)
                        {
                            if (!companies.Contains(item2)) //if already in the list DO NOT SAVE                   
                                companies.Add(item2);
                        }
                    }

                    List<Company> newCompanies = new List<Company>();
                    //newCompanies = companies.OrderBy(m => m.CompanyName.Length).ThenBy(m => m.CompanyName).ToList();
                    newCompanies = companies.OrderBy(m => m.CompanyName).ToList();
                    return newCompanies;
                }

                else if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //If PER COMPANY RETURN ONLY THE SELECTED COMPANY
                {
                    List<Company> companies = new List<Company>();
                    //List<Company> concatCompanies = new List<Company>();
                    List<Zone> zones = new List<Zone>();

                    int ZoneGroupId = db.ZoneGroup.Single(m => m.ZoneGroupCode == GroupName).ZoneGroupId;

                    zones = db.Zone.Where(m => m.ZoneGroup == ZoneGroupId.ToString()).ToList();

                    foreach (var item in zones)//we need to loop through each zones. coz companies are mapped thru ZONE CODE
                    {
                        List<Company> comp = new List<Company>();


                        if (NewCompany != null)
                            comp = NewCompany.Where(m => m.ZoneCode == item.ZoneCode).Where(m => m.Status == "Active").ToList();//get all companies in zonegroup
                        else
                            comp = db.Company.Where(m => m.ZoneCode == item.ZoneCode).Where(m => m.Status == "Active").ToList();//get all companies in zonegroup

                        //concatCompanies = companies.Concat(comp).ToList();
                        foreach (var item2 in comp)
                        {
                            if (!companies.Contains(item2)) //if already in the list DO NOT SAVE                   
                                companies.Add(item2);
                        }
                    }

                    List<Company> newCompanies = new List<Company>();
                    //newCompanies = companies.OrderBy(m => m.CompanyName.Length).ThenBy(m => m.CompanyName).ToList();
                    newCompanies = companies.OrderBy(m => m.CompanyName).ToList();
                    return newCompanies;
                }

                else
                {
                    return NewCompany;
                }
            }
        }

        public List<RentalInformation> RentalInformations
        {
            get
            {
                List<Company> companies = Companies;
                List<RentalInformation> listOfRentalInformtations = new List<RentalInformation>();
                List<RentalInformation> rentalInformations = new List<RentalInformation>();
                List<string> balanceList = new List<string>();
                //----Start Changes 1-3-2017
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //Do not load the informations with balances if GENERATE PER COMPANY
                {
                    var tempRentalList = db.RentalInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).ToList().Select(x => x.RentalInformationId).ToList();
                    //List<Balances> bal = new List<Balances>();
                    //bal = db.Balances.Where(m => m.BillingType == "RENTAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                    //var tempBal = db.Balances.Where(m => m.BillingType == "RENTAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    //bal.AddRange(tempBal.ToList());
                    //bal = bal.Distinct().ToList();

                    var sureBalance = db.Balances.Where(m => m.BillingType == "RENTAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList().Select(x => x.BillingReference).ToList();
                    var fbBalance = db.Balances.Where(m => m.BillingType == "RENTAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList().Select(x => x.BillingReference).ToList();

                    balanceList.AddRange(tempRentalList.ConvertAll<string>(delegate (int i) { return i.ToString(); }));
                    balanceList.AddRange(sureBalance.ToList());
                    balanceList.AddRange(fbBalance.ToList());
                    balanceList = balanceList.Distinct().ToList();

                    foreach (var item in balanceList)
                    {
                        RentalInformation rental = new RentalInformation();
                        int i = int.Parse(item);
                        rental = db.RentalInformation.Where(m => m.RentalInformationId == i).FirstOrDefault();
                        rentalInformations.Add(rental);
                    }
                }
                else if (TypeOfBillingGenerate.ToUpper() != "do not execute this code no per company for rental")
                {
                    var tempRentalList = db.RentalInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).Where(m => m.CompanyId == generatePerCompanyId).ToList().Select(x => x.RentalInformationId).ToList();
                    var sureBalance = db.Balances.Where(m => m.BillingType == "RENTAL" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList().Select(x => x.BillingReference).ToList();
                    var fbBalance = db.Balances.Where(m => m.BillingType == "RENTAL" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList().Select(x => x.BillingReference).ToList();

                    balanceList.AddRange(tempRentalList.ConvertAll<string>(delegate (int i) { return i.ToString(); }));
                    balanceList.AddRange(sureBalance.ToList());
                    balanceList.AddRange(fbBalance.ToList());
                    balanceList = balanceList.Distinct().ToList();

                    foreach (var item in balanceList)
                    {
                        RentalInformation rental = new RentalInformation();
                        int i = int.Parse(item);
                        rental = db.RentalInformation.Where(m => m.RentalInformationId == i).FirstOrDefault();
                        rentalInformations.Add(rental);
                    }
                }
                //----End Changes 1-3-2017

                foreach (var rentalInformation in rentalInformations)
                {
                    //----Changes 1-3-2017
                    var a = listOfRentalInformtations.Where(m => m.RentalInformationId == rentalInformation.RentalInformationId).FirstOrDefault();
                    if (a == null)
                    {
                        var companyId = rentalInformation.CompanyId;
                        foreach (var company in companies)
                        {
                            if (companyId == company.CompanyID)
                            {
                                listOfRentalInformtations.Add(rentalInformation);
                                break;
                            }
                        }
                    }
                }
                return listOfRentalInformtations;
            }
        }

        public List<WaterMeterAssignment> WaterInformations
        {
            get
            {
                List<Company> companies = Companies;
                List<WaterMeterAssignment> waterMeterAssignment = new List<WaterMeterAssignment>();
                WaterMeterAssignment waterAssignment = new WaterMeterAssignment();
                List<String> arrlist = new List<string>();
                List<String> Firstarrlist = new List<string>();
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY")
                {
                    var currentBillingPeriod = db.WaterMeterReading.Where(m => m.BillingPeriod == billingPeriodId).ToList().Select(m => m.MeterNumber).Distinct();
                    arrlist.AddRange(currentBillingPeriod.ToList());

                    var balWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList().Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(balWater.ToList());

                    var balSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList().Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(balSewerage.ToList());

                    var fbWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").Select(m => m.BillingReference).Distinct();
                    var fbSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(fbWater.ToList());
                    arrlist.AddRange(fbSewerage.ToList());

                    arrlist = arrlist.Distinct().ToList();

                    var FirstbalWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE").ToList().Select(m => m.BillingReference).Distinct();
                    var FirstbalSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE").ToList().Select(m => m.BillingReference).Distinct();
                    Firstarrlist.AddRange(FirstbalWater);
                    Firstarrlist.AddRange(FirstbalSewerage);
                    Firstarrlist = Firstarrlist.Distinct().ToList();

                    foreach (var item in Firstarrlist)
                    {
                        if (!arrlist.Contains(item))
                        {
                            var tempData = db.WaterMeterReading.Where(m => m.MeterNumber == item).ToList();
                            if (tempData.Count == 1)
                                arrlist.Add(item);
                        }
                    }

                    currentBillingPeriod = null;
                    balWater = null;
                    balSewerage = null;
                }
                else if (TypeOfBillingGenerate.ToUpper() != "do not execute this code no per company for water")
                {
                    var currentBillingPeriod = db.WaterMeterReading.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId).ToList().Select(m => m.MeterNumber).Distinct();
                    arrlist.AddRange(currentBillingPeriod.ToList());

                    var balWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.CompanyId == generatePerCompanyId && m.Amount > 0).ToList().Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(balWater.ToList());

                    var balSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.CompanyId == generatePerCompanyId && m.Amount > 0).ToList().Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(balSewerage.ToList());

                    var fbWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.CompanyId == generatePerCompanyId && m.BalanceType == "FORWARDED BALANCE").Select(m => m.BillingReference).Distinct();
                    var fbSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.CompanyId == generatePerCompanyId && m.BalanceType == "FORWARDED BALANCE").Select(m => m.BillingReference).Distinct();
                    arrlist.AddRange(fbWater.ToList());
                    arrlist.AddRange(fbSewerage.ToList());

                    arrlist = arrlist.Distinct().ToList();

                    var FirstbalWater = db.Balances.Where(m => m.BillingType == "WATER" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE").ToList().Select(m => m.BillingReference).Distinct();
                    var FirstbalSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE").ToList().Select(m => m.BillingReference).Distinct();
                    Firstarrlist.AddRange(FirstbalWater);
                    Firstarrlist.AddRange(FirstbalSewerage);
                    Firstarrlist = Firstarrlist.Distinct().ToList();

                    foreach (var item in Firstarrlist)
                    {
                        if (!arrlist.Contains(item))
                        {
                            var tempData = db.WaterMeterReading.Where(m => m.MeterNumber == item).ToList();
                            if (tempData.Count == 1)
                                arrlist.Add(item);
                        }
                    }

                    currentBillingPeriod = null;
                    balWater = null;
                    balSewerage = null;
                }

                foreach (var waterInformation in arrlist)
                {
                    //Check if exist in listOfWaterAssignment, Include in Billing == 1 and CompanyExist                    
                    waterAssignment = db.WaterMeterAssignment.Where(m => m.MeterNumber == waterInformation).FirstOrDefault();
                    if (waterAssignment.IncludeBilling == 1)
                    {
                        var isMeterExist = waterMeterAssignment.Where(m => m.MeterNumber == waterInformation).FirstOrDefault();
                        var isCompanyExist = db.Company.Any(m => m.CompanyID == waterAssignment.CompanyId);
                        if (isMeterExist == null && isCompanyExist == true)
                        {
                            waterMeterAssignment.Add(waterAssignment);
                        }
                    }
                }

                return waterMeterAssignment;
            }
        }

        //public List<WaterMeterReading> WaterReading
        //{
        //    get
        //    {
        //        List<Company> companies = Companies;
        //        List<WaterMeterAssignment> waterMeterAssignment = new List<WaterMeterAssignment>();
        //        List<WaterMeterReading> listOfWaterReading = new List<WaterMeterReading>();
        //        List<WaterMeterReading> WaterReading = db.WaterMeterReading.Where(m => m.BillingPeriod == billingPeriodId).ToList();

        //        //----Start Changes 1-3-2017
        //        List<Balances> balWater = new List<Balances>();
        //        List<Balances> balSewerage = new List<Balances>();
        //        balWater = db.Balances.Where(m => m.BillingType == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
        //        balSewerage = db.Balances.Where(m => m.BillingType == "SEWERAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
        //        foreach (var item in balWater)
        //        {
        //            List<WaterMeterReading> reading = new List<WaterMeterReading>();
        //            reading = db.WaterMeterReading.Where(m => m.MeterNumber == item.BillingReference).ToList();

        //            foreach (var read in reading)
        //            {
        //                WaterReading.Add(read);
        //            }
        //        }
        //        foreach (var item in balSewerage)
        //        {
        //            List<WaterMeterReading> reading = new List<WaterMeterReading>();
        //            reading = db.WaterMeterReading.Where(m => m.MeterNumber == item.BillingReference).ToList();

        //            foreach (var read in reading)
        //            {
        //                WaterReading.Add(read);
        //            }
        //        }
        //        //----End Changes 1-3-2017
        //        balWater = null;
        //        balSewerage = null;

        //        foreach (var waterInformation in WaterReading)
        //        {
        //            foreach (var company in companies)
        //            {
        //                //----Changes 1-3-2017
        //                var a = listOfWaterReading.Where(m => m.WaterMeterReadingId == waterInformation.WaterMeterReadingId).FirstOrDefault();
        //                WaterMeterAssignment waterAssignment = new WaterMeterAssignment();
        //                var meterNumber = db.WaterMeterReading.FirstOrDefault(m => m.WaterMeterReadingId == waterInformation.WaterMeterReadingId).MeterNumber;
        //                waterAssignment = db.WaterMeterAssignment.Where(m => m.MeterNumber == meterNumber).FirstOrDefault();
        //                if (waterAssignment.IncludeBilling == 1)
        //                {
        //                    if (a == null)
        //                    {
        //                        var companyId = waterInformation.CompanyId;
        //                        if (companyId == company.CompanyID)
        //                        {
        //                            listOfWaterReading.Add(waterInformation);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var waterInformation in listOfWaterReading)
        //        {
        //            var a = listOfWaterReading.Where(m => m.WaterMeterReadingId == waterInformation.WaterMeterReadingId).FirstOrDefault();
        //            WaterMeterAssignment waterAssignment = new WaterMeterAssignment();
        //            var meterNumber = db.WaterMeterReading.FirstOrDefault(m => m.WaterMeterReadingId == waterInformation.WaterMeterReadingId).MeterNumber;
        //            var isExist = waterMeterAssignment.Where(m => m.MeterNumber == meterNumber).FirstOrDefault();
        //            if (isExist == null)
        //            {
        //                waterAssignment = db.WaterMeterAssignment.Where(m => m.MeterNumber == meterNumber).FirstOrDefault();
        //                waterMeterAssignment.Add(waterAssignment);
        //            }
        //        }
        //        return listOfWaterReading;
        //    }
        //}

        public List<PoleInformation> PoleInformations //OK 1-3-2017
        {
            get
            {
                List<Company> companies = Companies;
                List<PoleInformation> listOfPoleInformations = new List<PoleInformation>();
                List<PoleInformation> poleInformations = new List<PoleInformation>();

                //----Start Changes 1-3-2017
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //Do not load the informations with balances if GENERATE PER COMPANY
                {
                    poleInformations = db.PoleInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "POLE RENTAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "POLE RENTAL" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference == item.BillingReference).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    foreach (var item in bal)
                    {
                        PoleInformation pole = new PoleInformation();
                        int i = int.Parse(item.BillingReference);
                        pole = db.PoleInformation.Where(m => m.PoleInformationId == i).FirstOrDefault();
                        poleInformations.Add(pole);
                    }
                }
                else if (TypeOfBillingGenerate.ToUpper() != "do not execute this code no per company for pole")
                {
                    poleInformations = db.PoleInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).Where(m => m.CompanyId == generatePerCompanyId).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "POLE RENTAL" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "POLE RENTAL" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference == item.BillingReference).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "POLE RENTAL").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    foreach (var item in bal)
                    {
                        PoleInformation pole = new PoleInformation();
                        int i = int.Parse(item.BillingReference);
                        pole = db.PoleInformation.Where(m => m.PoleInformationId == i).FirstOrDefault();
                        poleInformations.Add(pole);
                    }
                }
                //----End Changes 1-3-2017

                foreach (var poleInformation in poleInformations)
                {
                    //----Changes 1-3-2017
                    var a = listOfPoleInformations.Where(m => m.PoleInformationId == poleInformation.PoleInformationId).FirstOrDefault();
                    if (a == null)
                    {
                        var companyId = poleInformation.CompanyId;
                        foreach (var company in companies)
                        {
                            if (companyId == company.CompanyID)
                            {
                                listOfPoleInformations.Add(poleInformation);
                                break;
                            }
                        }
                    }
                }
                return listOfPoleInformations;
            }
        }

        public List<FranchiseFeeInformation> FranchiseInformations
        {
            get
            {
                List<Company> companies = Companies;
                List<FranchiseFeeInformation> listOfFranchiseInformations = new List<FranchiseFeeInformation>();
                List<FranchiseFeeInformation> FranchiseInformations = new List<FranchiseFeeInformation>();

                //----Start Changes 1-3-2017
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //Do not load the informations with balances if GENERATE PER COMPANY
                {
                    FranchiseInformations = db.FranchiseFeeInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "FRANCHISE" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "FRANCHISE" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference == item.BillingReference).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        FranchiseFeeInformation franchise = new FranchiseFeeInformation();
                        int i = int.Parse(item.BillingReference);
                        franchise = db.FranchiseFeeInformation.Where(m => m.FranchiseFeeInformationId == i).FirstOrDefault();
                        FranchiseInformations.Add(franchise);
                    }
                }
                else if (TypeOfBillingGenerate.ToUpper() != "do not execute this code no per company for franchise")
                {
                    FranchiseInformations = db.FranchiseFeeInformation.Where(m => m.StartDate <= CoverageTo).Where(m => m.EndDate >= CoverageFrom).Where(m => m.CompanyId == generatePerCompanyId).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "FRANCHISE" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "FRANCHISE" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference == item.BillingReference).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "FRANCHISE").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        FranchiseFeeInformation franchise = new FranchiseFeeInformation();
                        int i = int.Parse(item.BillingReference);
                        franchise = db.FranchiseFeeInformation.Where(m => m.FranchiseFeeInformationId == i).FirstOrDefault();
                        FranchiseInformations.Add(franchise);
                    }
                }
                //----End Changes 1-3-2017

                foreach (var franchiseInformation in FranchiseInformations)
                {

                    foreach (var company in companies)
                    {
                        //----Changes 1-3-2017
                        var a = listOfFranchiseInformations.Where(m => m.FranchiseFeeInformationId == franchiseInformation.FranchiseFeeInformationId).FirstOrDefault();
                        if (a == null)
                        {
                            var companyId = franchiseInformation.CompanyId;
                            if (companyId == company.CompanyID)
                            {
                                listOfFranchiseInformations.Add(franchiseInformation);
                                break;
                            }
                        }
                    }
                }
                return listOfFranchiseInformations;
            }
        }

        public List<GarbageInformation> GarbageInformations
        {
            get
            {
                List<Company> companies = Companies;
                List<GarbageInformation> listOfGarbageInformations = new List<GarbageInformation>();
                List<GarbageInformation> GarbageInformations = new List<GarbageInformation>();

                //----Start Changes 1-3-2017
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //Do not load the informations with balances if GENERATE PER COMPANY
                {
                    GarbageInformations = db.GarbageInformations.Where(m => m.BillingPeriod == billingPeriodId).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "GARBAGE" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "GARBAGE" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId)
                                .Where(m => m.BillingType == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                        else
                        {
                            //First record in balances should be subject for unpaid billing               
                            var firstBalance = db.Balances.Where(m => m.CompanyId == item.CompanyId && m.TransactionType == "BILLING" && m.BillingType == "GARBAGE" && m.BillingSubType == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (firstBalance.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        GarbageInformation garbage = new GarbageInformation();
                        int i = int.Parse(item.BillingReference);
                        garbage = db.GarbageInformations.Where(m => m.GarbageInformationId == i).FirstOrDefault();
                        if (!GarbageInformations.Any(m => m.CompanyId == garbage.CompanyId))
                            GarbageInformations.Add(garbage);
                    }
                }
                else if (TypeOfBillingGenerate.ToUpper() != "do not execute this code no per company for garbage")
                {
                    GarbageInformations = db.GarbageInformations.Where(m => m.BillingPeriod == billingPeriodId).Where(m => m.CompanyId == generatePerCompanyId).ToList();
                    List<Balances> bal = new List<Balances>();
                    bal = db.Balances.Where(m => m.BillingType == "GARBAGE" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();

                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    var tempBal = db.Balances.Where(m => m.BillingType == "GARBAGE" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    //var tempBal1 = db.Database.SqlQuery<Tuple<string, string>>("select distinct count(CompanyId), CompanyId from Balances where BillingType = 'GARBAGE' and BillingSubType = 'PRINCIPAL' group by CompanyId").ToList();
                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId).FirstOrDefault();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId)
                                .Where(m => m.BillingType == "GARBAGE").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                        else
                        {
                            //First record in balances should be subject for unpaid billing               
                            var firstBalance = db.Balances.Where(m => m.CompanyId == item.CompanyId && m.TransactionType == "BILLING" && m.BillingType == "GARBAGE" && m.BillingSubType == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (firstBalance.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        //GarbageInformation garbage = new GarbageInformation();
                        //int i = int.Parse(item.BillingReference);
                        //garbage = db.GarbageInformations.Where(m => m.GarbageInformationId == i).FirstOrDefault();
                        //GarbageInformations.Add(garbage);

                        GarbageInformation garbage = new GarbageInformation();
                        int i = int.Parse(item.BillingReference);
                        garbage = db.GarbageInformations.Where(m => m.GarbageInformationId == i).FirstOrDefault();
                        if (!GarbageInformations.Any(m => m.CompanyId == garbage.CompanyId))
                            GarbageInformations.Add(garbage);
                    }
                }
                //----End Changes 1-3-2017

                foreach (var garbageInformation in GarbageInformations)
                {
                    foreach (var company in companies)
                    {
                        //----Changes 1-3-2017
                        var a = listOfGarbageInformations.Where(m => m.GarbageInformationId == garbageInformation.GarbageInformationId).FirstOrDefault();
                        if (a == null)
                        {
                            var companyId = garbageInformation.CompanyId;
                            if (companyId == company.CompanyID)
                            {
                                listOfGarbageInformations.Add(garbageInformation);
                                break;
                            }
                        }
                    }
                }
                return listOfGarbageInformations;
            }
        }

        public List<PassedOnBillingInformation> PassedOnBillingInformations
        {
            get
            {
                List<Company> companies = Companies;
                List<PassedOnBillingInformation> listOfPassedOnBillingInformations = new List<PassedOnBillingInformation>();
                List<PassedOnBillingInformation> PassedOnBillingInformations = new List<PassedOnBillingInformation>();

                //----Start Changes 1-3-2017
                if (TypeOfBillingGenerate.ToUpper() != "PERCOMPANY") //Do not load the informations with balances if GENERATE PER COMPANY
                {
                    List<Balances> bal = new List<Balances>();
                    List<Balances> tempBal = new List<Balances>();
                    if (TypeOfBillingGenerateValue == "POBWater")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "WATER").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "WATER" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "WATER" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBPower")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "POWER").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "POWER" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "POWER" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBJanitorial")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "JANITORIAL").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "JANITORIAL" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "JANITORIAL" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBSecurityGuard")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "SECURITY GUARD").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "SECURITY GUARD" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "SECURITY GUARD" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBSystemLoss")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "SYSTEM LOSS").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "SYSTEM LOSS" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "SYSTEM LOSS" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBConcessionFee")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.Type.ToUpper() == "CONCESSION FEE").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "CONCESSION FEE" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.BillingReference == "CONCESSION FEE" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId).ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }

 
                    //List<Tuple<int,string,int>> FirstBalance = new List<Tuple<int,string,int>>();
                    
                    //var balanceForFirstGeneration = db.Balances.SqlQuery("select distinct(CompanyId),BillingReference,count(billingtype) as cnt from balances where billingtype = 'PASSED ON BILLING' and BillingSubType = 'BALANCE' group by CompanyId,BillingReference order by CompanyId").ToList();
                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    //var tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference.ToUpper() == item.BillingReference.ToUpper()).ToList();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                        else
                        {
                            //First record in balances should be subject for unpaid billing               
                            var firstBalance = db.Balances.Where(m => m.CompanyId == item.CompanyId && m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == item.BillingReference.ToUpper() && m.BillingSubType == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (firstBalance.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        var isExist = PassedOnBillingInformations.Any(m => m.CompanyId == item.CompanyId && m.Type.ToUpper() == item.BillingReference.ToUpper());
                        if (!isExist)
                        {
                            PassedOnBillingInformation passedOnBilling = new PassedOnBillingInformation();
                            passedOnBilling = db.PassedOnBillingInformation.Where(m => m.CompanyId == item.CompanyId).Where(m => m.Type.ToUpper() == item.BillingReference.ToUpper()).FirstOrDefault();
                            PassedOnBillingInformations.Add(passedOnBilling);
                        }
                    }
                }
                else
                {
                    List<Balances> bal = new List<Balances>();
                    List<Balances> tempBal = new List<Balances>();
                    if (TypeOfBillingGenerateValue == "POBWater")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "WATER").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "WATER" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "WATER" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBPower")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "POWER").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "POWER" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "POWER" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBJanitorial")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "JANITORIAL").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "JANITORIAL" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "JANITORIAL" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBSecurityGuard")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "SECURITY GUARD").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "SECURITY GUARD" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "SECURITY GUARD" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBSystemLoss")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "SYSTEM LOSS").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "SYSTEM LOSS" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "SYSTEM LOSS" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else if (TypeOfBillingGenerateValue == "POBConcessionFee")
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId && m.CompanyId == generatePerCompanyId && m.Type.ToUpper() == "CONCESSION FEE").ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "CONCESSION FEE" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == "CONCESSION FEE" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }
                    else
                    {
                        PassedOnBillingInformations = db.PassedOnBillingInformation.Where(m => m.BillingPeriod == billingPeriodId).Where(m => m.CompanyId == generatePerCompanyId).ToList();
                        bal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.CompanyId == generatePerCompanyId && m.BillingSubType.ToUpper() == "BALANCE" && m.Amount > 0).ToList();
                        tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                    }


                    //List<Tuple<int,string,int>> FirstBalance = new List<Tuple<int,string,int>>();

                    //var balanceForFirstGeneration = db.Balances.SqlQuery("select distinct(CompanyId),BillingReference,count(billingtype) as cnt from balances where billingtype = 'PASSED ON BILLING' and BillingSubType = 'BALANCE' group by CompanyId,BillingReference order by CompanyId").ToList();
                    //Changes 3-06-17 - Compute balances for 1st billing generation of service
                    //Problem solved: 1st billing generation has 0 balance in BALANCE table... this should have interest in next billing generate
                    //Get all forwarded balance data.
                    //var tempBal = db.Balances.Where(m => m.BillingType == "PASSED ON BILLING" && m.CompanyId == generatePerCompanyId && m.TransactionType == "BILLING" && m.BillingSubType.ToUpper() == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();

                    foreach (var item in tempBal)
                    {
                        //check if exist in current POB data with balances
                        var isExist = bal.Where(m => m.CompanyId == item.CompanyId && m.BillingReference.ToUpper() == item.BillingReference.ToUpper()).ToList();
                        if (isExist == null)
                        {
                            //If exist. count the number of records. If 1 add to record.
                            var countRecord = db.Balances.Where(m => m.CompanyId == item.CompanyId).Where(m => m.BillingReference.ToUpper() == item.BillingReference.ToUpper())
                                .Where(m => m.BillingType == "PASSED ON BILLING").Where(m => m.BillingSubType.ToUpper() == "BALANCE")
                                .Where(m => m.TransactionType == "BILLING").Where(m => m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (countRecord.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                        else
                        {
                            //First record in balances should be subjected for unpaid billing               
                            var firstBalance = db.Balances.Where(m => m.CompanyId == item.CompanyId && m.BillingType == "PASSED ON BILLING" && m.TransactionType == "BILLING" && m.BillingReference.ToUpper() == item.BillingReference.ToUpper() && m.BillingSubType == "BALANCE" && m.BalanceType == "FORWARDED BALANCE").ToList();
                            if (firstBalance.Count == 1)
                            {
                                bal.Add(item);
                            }
                        }
                    }
                    //End changes 3-06-17

                    foreach (var item in bal)
                    {
                        var isExist = PassedOnBillingInformations.Any(m => m.CompanyId == item.CompanyId && m.Type.ToUpper() == item.BillingReference.ToUpper());
                        if (!isExist)
                        {
                            PassedOnBillingInformation passedOnBilling = new PassedOnBillingInformation();
                            passedOnBilling = db.PassedOnBillingInformation.Where(m => m.CompanyId == item.CompanyId).Where(m => m.Type.ToUpper() == item.BillingReference.ToUpper()).FirstOrDefault();
                            PassedOnBillingInformations.Add(passedOnBilling);
                        }
                    }
                }
                //----End Changes 1-3-2017

                foreach (var passedOnBillingInformations in PassedOnBillingInformations)
                {
                    foreach (var company in companies)
                    {
                        //----Changes 1-3-2017
                        var a = listOfPassedOnBillingInformations.Where(m => m.CompanyId == passedOnBillingInformations.CompanyId && m.Type == passedOnBillingInformations.Type).FirstOrDefault();
                        if (a == null)
                        {
                            var companyId = passedOnBillingInformations.CompanyId;
                            if (companyId == company.CompanyID)
                            {
                                listOfPassedOnBillingInformations.Add(passedOnBillingInformations);
                                break;
                            }
                        }
                    }
                }
                return listOfPassedOnBillingInformations;
            }
        }
    }
}