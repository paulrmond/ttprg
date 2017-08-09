using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public class BillingRegenerateBLL
    {
        int id;
        int companyId;
        string EnterpriseType;
        public BillingRegenerateBLL(int id, int companyId)
        {
            this.id = id;
            this.companyId = companyId;
        }

        public BillingRegenerateBLL(int id)
        {
            this.id = id;
        }

        public BillingRegenerateBLL(string EnterpriseType)
        {
            this.EnterpriseType = EnterpriseType;
        }

        public BillingRegenerateBLL(int id,string EnterpriseType)
        {
            this.id = id;
            this.EnterpriseType = EnterpriseType;
        }

        BCS_Context db = new BCS_Context();
        public bool RegenerateBilling()
        {
            bool isDeleted = false;

            try
            {
                //db.SubsidiaryLedger.RemoveRange(db.SubsidiaryLedger.Where(m => m.BillingPeriod == id));
                db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and TransactionType = 'BILLING'");
                //db.Database.ExecuteSqlCommand("Delete from GeneralBillings where BillingPeriod = '" + id + "' and TransactionType = 'BILLING'");
                db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "'"); //and TransactionType = 'BILLING'");
                db.SaveChanges();
                isDeleted = true;
            }
            catch (Exception)
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool RegenerateBillingPerCompany()
        {
            bool isDeleted = false;

            try
            {
                //db.SubsidiaryLedger.RemoveRange(db.SubsidiaryLedger.Where(m => m.BillingPeriod == id));
                db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and CompanyId = '" + companyId + "' and TransactionType = 'BILLING'");
                //db.Database.ExecuteSqlCommand("Delete from GeneralBillings where BillingPeriod = '" + id + "' and CompanyId = '" + companyId + "' and TransactionType = 'BILLING'");
                db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "' and CompanyId = '" + companyId + "'");// and TransactionType = 'BILLING'");
                db.SaveChanges();
                isDeleted = true;
            }
            catch (Exception)
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool RegenerateBillingPerEnterprise()
        {
            bool isDeleted = false;

            try
            {
                BCS_Context db = new BCS_Context();
                List<Company> Companies = new List<Company>();
                Companies = db.Company.Where(m => m.EnterpriseType.ToUpper() == EnterpriseType.ToUpper()).ToList();
                foreach (var item in Companies)
                {
                    //db.SubsidiaryLedger.RemoveRange(db.SubsidiaryLedger.Where(m => m.BillingPeriod == id));
                    db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and CompanyId = '" + item.CompanyID + "' and TransactionType = 'BILLING'");
                    //db.Database.ExecuteSqlCommand("Delete from GeneralBillings where BillingPeriod = '" + id + "' and CompanyId = '" + companyId + "' and TransactionType = 'BILLING'");
                    db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "' and CompanyId = '" + item.CompanyID + "'");// and TransactionType = 'BILLING'");
                }

                db.SaveChanges();
                isDeleted = true;
            }
            catch (Exception)
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool RegenerateBillingPerBillingType()
        {
            bool isDeleted = false;

            string BillingType = "";
            if (EnterpriseType.ToUpper() == "POLE")
                BillingType = "POLE RENTAL";
            else
                BillingType = EnterpriseType.ToUpper();

            try
            {
                if(BillingType.ToUpper() != "WATER")
                {
                    //db.SubsidiaryLedger.RemoveRange(db.SubsidiaryLedger.Where(m => m.BillingPeriod == id));
                    db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and BillingType = '" + BillingType + "' and TransactionType = 'BILLING'");
                    //db.Database.ExecuteSqlCommand("Delete from GeneralBillings where BillingPeriod = '" + id + "' and CompanyId = '" + companyId + "' and TransactionType = 'BILLING'");
                    db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "' and BillingType = '" + BillingType + "'");// and TransactionType = 'BILLING'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and BillingType = '" + BillingType + "' and TransactionType = 'BILLING'");
                    db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "' and BillingType = '" + BillingType + "'");// and TransactionType = 'BILLING'");

                    db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and BillingType = 'SEWERAGE' and TransactionType = 'BILLING'");
                    db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "' and BillingType = 'SEWERAGE'");// and TransactionType = 'BILLING'");
                }

                db.SaveChanges();
                isDeleted = true;
            }
            catch (Exception)
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool RegenerateBillingPerAdminFee()
        {
            bool isDeleted = false;
            List<int?> devId = new List<int?>();
            List<AdminFee> AdminFees = new List<AdminFee>();
            AdminFees = db.AdminFee.Where(m => m.BillingPeriodId == id).ToList();
            AdminFeeEnterpriseTypeBLL adminFeeEnterpriseTypeBLL = new AdminFeeEnterpriseTypeBLL(AdminFees, EnterpriseType);
            AdminFees = adminFeeEnterpriseTypeBLL.AdminFeePerEnterprise();
            var devCompCode = AdminFees.Select(m => m.Dev_Comp_Code).Distinct().ToList();
            foreach (var item in devCompCode)
            {
                int? i = db.Company.Where(m => m.CompanyCode == item.ToString()).Select(x => (int?)x.CompanyID).FirstOrDefault() ?? 0;
                devId.Add(i);
            }

            try
            {
                foreach (var item in devId)
                {
                    //db.SubsidiaryLedger.RemoveRange(db.SubsidiaryLedger.Where(m => m.BillingPeriod == id));
                    db.Database.ExecuteSqlCommand("Delete from SubsidiaryLedgers where BillingPeriod = '" + id + "' and CompanyId = '"+ item +"' and BillingType = 'ADMIN FEE' and TransactionType = 'BILLING'");
                    //db.Database.ExecuteSqlCommand("Delete from GeneralBillings where BillingPeriod = '" + id + "' and CompanyId = '" + companyId + "' and TransactionType = 'BILLING'");
                    db.Database.ExecuteSqlCommand("Delete from Balances where CurrentBillingPeriod = '" + id + "'  and CompanyId = '" + item + "' and BillingType = 'ADMIN FEE'");// and TransactionType = 'BILLING'");
                }                
                db.SaveChanges();
                isDeleted = true;
            }
            catch (Exception)
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool FinalizeBilling()
        {
            bool isFinalize = false;
            using (var dbtransaction = db.Database.BeginTransaction())
            {
                BillingPeriod billingPeriod = db.BillingPeriod.Find(id);
                billingPeriod.Finalized = "Yes";
                db.Entry(billingPeriod).State = System.Data.Entity.EntityState.Modified;

                List<SubsidiaryLedger> rental = new List<SubsidiaryLedger>();
                List<SubsidiaryLedger> pole = new List<SubsidiaryLedger>();
                List<SubsidiaryLedger> garbage = new List<SubsidiaryLedger>();
                List<SubsidiaryLedger> water = new List<SubsidiaryLedger>();
                List<SubsidiaryLedger> franchise = new List<SubsidiaryLedger>();
                List<SubsidiaryLedger> passedonbilling = new List<SubsidiaryLedger>();

                List<SubsidiaryLedger> subsidiaryLedger = db.SubsidiaryLedger.Where(m => m.BillingPeriod == id).ToList();
                rental = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "rental").ToList();
                pole = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "pole rental").ToList();
                garbage = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "garbage").ToList();
                water = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "water").ToList();
                franchise = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "franchise").ToList();
                passedonbilling = subsidiaryLedger.Where(m => m.BillingType.ToLower() == "passed on billing").ToList();

                foreach (var item in rental)
                {
                    RentalInformation rentalinfo = db.RentalInformation.Find(int.Parse(item.BillingReference));
                    rentalinfo.BillingStatus = "Finalized";
                    db.Entry(rentalinfo).State = System.Data.Entity.EntityState.Modified;
                }
                foreach (var item in pole)
                {
                    PoleInformation poleinfo = db.PoleInformation.Find(int.Parse(item.BillingReference));
                    poleinfo.BillingStatus = "Finalized";
                    db.Entry(poleinfo).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var item in garbage)
                {
                    GarbageInformation garbageinfo = db.GarbageInformations.Find(int.Parse(item.BillingReference));
                    garbageinfo.BillingStatus = "Finalized";
                    db.Entry(garbageinfo).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var item in water)
                {
                    WaterMeterReading waterinfo = db.WaterMeterReading.Where(m => m.MeterNumber == item.BillingReference).FirstOrDefault();
                    waterinfo.BillingStatus = "Finalized";
                    db.Entry(waterinfo).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var item in franchise)
                {
                    FranchiseFeeInformation franchiseinfo = db.FranchiseFeeInformation.Find(int.Parse(item.BillingReference));
                    franchiseinfo.BillingStatus = "Finalized";
                    db.Entry(franchiseinfo).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var item in passedonbilling)
                {
                    PassedOnBillingInformation passedonbillinginfo = db.PassedOnBillingInformation.Find(int.Parse(item.BillingReference));
                    passedonbillinginfo.BillingStatus = "Finalized";
                    db.Entry(passedonbillinginfo).State = System.Data.Entity.EntityState.Modified;
                }

                isFinalize = true;
                db.SaveChanges();
                dbtransaction.Commit();
            }

            return isFinalize;
        }
    }
}