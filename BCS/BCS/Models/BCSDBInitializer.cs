using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public class BCSDBInitializer : DropCreateDatabaseIfModelChanges<BCS_Context>
    {
        protected override void Seed(BCS_Context context)
        {
            ////BILLING PERIOD
            //var BillingPeriods = new List<BillingPeriod>
            //{
            //    //new BillingPeriod { DateFrom=DateTime.Now, DateTo=DateTime.Now.AddMonths(1), PeriodText="For the month of..", Finalized="NO",Generated="NO" }
            //};
            //foreach (var temp1 in BillingPeriods)
            //{
            //    //context.BillingPeriod.Add(temp1);
            //}
            //context.SaveChanges();            

            //RATES
            var BillingRate = new List<BillingRate>
            {
                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate = 110.00M, ZoneGroup="01" },
                new BillingRate { Category = "Rental Fee", SubCategory = "USD Land", Rate = 120.00M, ZoneGroup="01" },
                new BillingRate { Category = "Pole Rental", SubCategory = "Pole", Rate = 0, ZoneGroup="01" },
                new BillingRate { Category = "Franchise", SubCategory = "Franchise", Rate = 0, ZoneGroup="01" },

                new BillingRate { Category = "Garbage Fee", SubCategory = "Segregated", Rate = 200.00M, ZoneGroup="01" },
                new BillingRate { Category = "Sewerage", SubCategory = "Sewerage", Rate = 4.8M, ZoneGroup="01" },

                new BillingRate { Category = "Water", SubCategory = "0 - 25", Rate = 185.00M, ZoneGroup="01" },
                new BillingRate { Category = "Water", SubCategory = "26 - 975", Rate = 7.4M, ZoneGroup="01" },

                new BillingRate { Category = "Water", SubCategory = "976 - 10000", Rate = 8.64M, ZoneGroup="01" },

                new BillingRate { Category = "Admin Fee", SubCategory = "Admin Fee", Rate=100.00M, ZoneGroup="01" },

                new BillingRate { Category = "Passed On Billing", SubCategory = "Power", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Water", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Janitorial", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Security Guard", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "System Loss", Rate=100.00M, ZoneGroup="01" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Concession Fee", Rate=100.00M, ZoneGroup="01" },

                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate = 110.00M, ZoneGroup="03" },
                new BillingRate { Category = "Rental Fee", SubCategory = "USD Land", Rate = 120.00M, ZoneGroup="03" },

                new BillingRate { Category = "Garbage Fee", SubCategory = "Segregated", Rate = 200.00M, ZoneGroup="03" },
                new BillingRate { Category = "Garbage Fee", SubCategory = "Fixed", Rate = 200.00M, ZoneGroup="03" },
                new BillingRate { Category = "Sewerage", SubCategory = "Sewerage", Rate = 4.8M, ZoneGroup="03" },
                new BillingRate { Category = "Pole Rental", SubCategory = "Pole", Rate = 0, ZoneGroup="03" },
                new BillingRate { Category = "Franchise", SubCategory = "Franchise", Rate = 0, ZoneGroup="03" },

                new BillingRate { Category = "Water", SubCategory = "0 - 25", Rate = 185.00M, ZoneGroup="03" },
                new BillingRate { Category = "Water", SubCategory = "26 - 975", Rate = 7.4M, ZoneGroup="03" },

                new BillingRate { Category = "Water", SubCategory = "976 - 10000", Rate = 8.64M, ZoneGroup="03" },

                new BillingRate { Category = "Admin Fee", SubCategory = "Admin Fee", Rate=100.00M, ZoneGroup="03" },

                new BillingRate { Category = "Passed On Billing", SubCategory = "Power", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Water", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Janitorial", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Security Guard", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "System Loss", Rate=100.00M, ZoneGroup="03" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Concession Fee", Rate=100.00M, ZoneGroup="03" },

                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate = 110.00M, ZoneGroup="06" },
                new BillingRate { Category = "Rental Fee", SubCategory = "USD Land", Rate = 120.00M, ZoneGroup="06" },
                new BillingRate { Category = "Pole Rental", SubCategory = "Pole", Rate = 0, ZoneGroup="06" },
                new BillingRate { Category = "Franchise", SubCategory = "Franchise", Rate = 0, ZoneGroup="06" },
                new BillingRate { Category = "Garbage Fee", SubCategory = "Segregated", Rate = 200.00M, ZoneGroup="06" },
                new BillingRate { Category = "Sewerage", SubCategory = "Sewerage", Rate = 4.8M, ZoneGroup="06" },

                new BillingRate { Category = "Water", SubCategory = "0 - 25", Rate = 185.00M, ZoneGroup="06" },
                new BillingRate { Category = "Water", SubCategory = "26 - 975", Rate = 7.4M, ZoneGroup="06" },

                new BillingRate { Category = "Water", SubCategory = "976 - 10000", Rate = 8.64M, ZoneGroup="06" },

                new BillingRate { Category = "Admin Fee", SubCategory = "Admin Fee", Rate=100.00M, ZoneGroup="06" },

                new BillingRate { Category = "Passed On Billing", SubCategory = "Power", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Water", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Janitorial", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Security Guard", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "System Loss", Rate=100.00M, ZoneGroup="06" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Concession Fee", Rate=100.00M, ZoneGroup="06" },

                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Rental Fee", SubCategory = "PHP Land", Rate = 110.00M, ZoneGroup="09" },
                new BillingRate { Category = "Rental Fee", SubCategory = "USD Land", Rate = 120.00M, ZoneGroup="09" },

                new BillingRate { Category = "Garbage Fee", SubCategory = "Segregated", Rate = 200.00M, ZoneGroup="09" },
                new BillingRate { Category = "Sewerage", SubCategory = "Sewerage", Rate = 4.8M, ZoneGroup="09" },
                new BillingRate { Category = "Pole Rental", SubCategory = "Pole", Rate = 0, ZoneGroup="09" },
                new BillingRate { Category = "Franchise", SubCategory = "Franchise", Rate = 0, ZoneGroup="09" },
                new BillingRate { Category = "Water", SubCategory = "0 - 25", Rate = 185.00M, ZoneGroup="09" },
                new BillingRate { Category = "Water", SubCategory = "26 - 975", Rate = 7.4M, ZoneGroup="09" },

                new BillingRate { Category = "Water", SubCategory = "976 - 10000", Rate = 8.64M, ZoneGroup="09" },

                new BillingRate { Category = "Admin Fee", SubCategory = "Admin Fee", Rate=100.00M, ZoneGroup="09" },

                new BillingRate { Category = "Passed On Billing", SubCategory = "Power", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Water", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Janitorial", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Security Guard", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "System Loss", Rate=100.00M, ZoneGroup="09" },
                new BillingRate { Category = "Passed On Billing", SubCategory = "Concession Fee", Rate=100.00M, ZoneGroup="09" }
            };
            foreach (var temp in BillingRate)
            {
                context.BillingRates.Add(temp);
            }
            context.SaveChanges();

            //Department
            var departmentList = new List<Division>
            {
                new Division { Code="EAD",Name="Enterprise Assistance Division(EAD)" },
                new Division {Code="EDD",Name="Ecozone Development Department(EDD)" },
                new Division {Code="OZA",Name="Office of the Zone Administrator(OZA)" },
                new Division {Code="ESG",Name="Environmental Safety Group(ESG)" },
                new Division {Code="ERD",Name="Enterprise Registration Division(ERD)" },
                new Division {Code="EOD",Name="Enterprise Operations Division(EOD)" },
                new Division {Code="IMD",Name="Incentives Management Division(IMD)" },
                new Division {Code="EHSD",Name="Environmental Health & Safety Division(EHSD)" },
                new Division {Code="FD",Name="Finance Department(FD)" },
                new Division {Code="IRD",Name="Industrial Relations Division(IRD)" },
                new Division {Code="BAC",Name="Bids & Awards Committee(BAC)" },
                new Division {Code="GSD",Name="General Services Division(GSD)" },
                new Division {Code="SSD",Name="Security Services Division(SSD)" },
                new Division {Code="PEP",Name="PEZA Police(PEP)" },
                new Division {Code="MISD",Name="Management Information System Department(MISD)" },
                new Division {Code="ASD",Name="Administrative Services Division(ASD)" },
                new Division {Code="EMD",Name="Engineering and Maintenance Division(EMD)" }
            };
            foreach (var temp in departmentList)
            {
                context.Division.Add(temp);
            }
            context.SaveChanges();

            //Zone group
            var zonegroup = new List<ZoneGroup>
            {
                new ZoneGroup { ZoneGroupCode="99", ZoneGroupName="Super User", ZoneGroupAddress="Bldg 4A PNOC-DOE Compound, Rizal Drive, Bonifacio Global City, Taguig", ZoneGroupRole="Super User", ZoneGroupInitials = "SUPER"},
                new ZoneGroup { ZoneGroupCode="01", ZoneGroupName="Head Office", ZoneGroupAddress="Bldg 4A PNOC-DOE Compound, Rizal Drive, Bonifacio Global City, Taguig", ZoneGroupRole="H.O Cluster Head", ZoneGroupInitials = "HO"},
                new ZoneGroup { ZoneGroupCode="03", ZoneGroupName="Cavite Economic Zone", ZoneGroupAddress="Administration Building, Rosario City, Cavite", ZoneGroupRole="Zone Cluster Head", ZoneGroupInitials = "CEZ"},
                new ZoneGroup { ZoneGroupCode="06", ZoneGroupName="Baguio City Economic Zone", ZoneGroupAddress="Loakan Road, Baguio City, Benguet", ZoneGroupRole="Zone Cluster Head", ZoneGroupInitials = "BCEZ"},
                new ZoneGroup { ZoneGroupCode="09", ZoneGroupName="Mactan Economic Zone", ZoneGroupAddress="Administration Building, 1st Avenue, Lapu-Lapu City, Cebu", ZoneGroupRole="Zone Cluster Head", ZoneGroupInitials = "MEZ"}
            };
            foreach (var temp in zonegroup)
            {
                context.ZoneGroup.Add(temp);
            }
            context.SaveChanges();

            //Billing Period
            var billingPeriod = new List<BillingPeriod>
            {
                new BillingPeriod {DateFrom = DateTime.Now, DateTo = DateTime.Now,PeriodText="Forwarded Balance",BillingDate = DateTime.Now,DueDate = DateTime.Now, Generated="YES",Finalized="YES",groupCode="01" },
                new BillingPeriod {DateFrom = DateTime.Now, DateTo = DateTime.Now,PeriodText="Forwarded Balance",BillingDate = DateTime.Now,DueDate = DateTime.Now, Generated="YES",Finalized="YES",groupCode="03" },
                new BillingPeriod {DateFrom = DateTime.Now, DateTo = DateTime.Now,PeriodText="Forwarded Balance",BillingDate = DateTime.Now,DueDate = DateTime.Now, Generated="YES",Finalized="YES",groupCode="06" },
                new BillingPeriod {DateFrom = DateTime.Now, DateTo = DateTime.Now,PeriodText="Forwarded Balance",BillingDate = DateTime.Now,DueDate = DateTime.Now, Generated="YES",Finalized="YES",groupCode="09" }
            };
            foreach (var item in billingPeriod)
            {
                context.BillingPeriod.Add(item);
            }
            context.SaveChanges();
            //Zones
            var zones = new List<Zone>
            {
                new Zone { ZoneCode="Code 1", ZoneName="Pacific Star", ZoneGroup="1"  },
                new Zone { ZoneCode="Code 2", ZoneName="DBP", ZoneGroup="1"  },
                new Zone { ZoneCode="Code 3", ZoneName="City Garden", ZoneGroup="2"  }
            };
            foreach (var temp in zones)
            {
                context.Zone.Add(temp);
            }

            //NGAS
            var ngas = new List<NGAS>
            {
                new NGAS { NGASCode="616-1",NGASAccount="Garbage Fee",NGASRateType="Garbage Fee" },
                new NGAS { NGASCode="795-2",NGASAccount="Garbage Collection",NGASRateType="Garbage Fee" },
                new NGAS { NGASCode="642-1",NGASAccount="Rent Income - Land",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-2",NGASAccount="Rent Income - SFB",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-3",NGASAccount="Rent Income - Office Space",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-4",NGASAccount="Rent Income - Appartment",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-5",NGASAccount="Rent Income - Dormitory",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-6",NGASAccount="Rent Income - Equipment",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="642-7",NGASAccount="Rent Income - Miscellaneous",NGASRateType="Rental Fee" },
                new NGAS { NGASCode="616-2",NGASAccount="Sewerage Fee",NGASRateType="Sewerage" },
                new NGAS { NGASCode="628-4",NGASAccount="Utilities - Water",NGASRateType="Water" },
                new NGAS { NGASCode="766",NGASAccount="Utilities Expense - Water",NGASRateType="Water" },
            };
            foreach (var temp in ngas)
            {
                context.NGAS.Add(temp);
            }

            //Role Assignment
            var roleassignment = new List<RoleAssignmentMatrix>
            {
                new RoleAssignmentMatrix { UserName="SU", Administrative=true, Aging=true, HO=true, Billing=true, Collection=true
                , Company=true, Franchise=true, Garbage=true, JBR=true, Payment=true, Period=true, Pole=true, Rate=true
                , Rentals=true, PassedOnBilling=true, Sewerage=true, SubsidiaryLedger=true, Water=true, ZoneGroup = "99"
                },

                new RoleAssignmentMatrix { UserName="HO", Administrative=true, Aging=true, HO=true, Billing=true, Collection=true
                , Company=true, Franchise=true, Garbage=true, JBR=true, Payment=true, Period=true, Pole=true, Rate=true
                , Rentals=true, PassedOnBilling=true, Sewerage=true, SubsidiaryLedger=true, Water=true, ZoneGroup = "01"
                },

                new RoleAssignmentMatrix { UserName="CEZ", Administrative=true, Aging=true, HO=true, Billing=true, Collection=true
                , Company=true, Franchise=true, Garbage=true, JBR=true, Payment=true, Period=true, Pole=true, Rate=true
                , Rentals=true, PassedOnBilling=true, Sewerage=true, SubsidiaryLedger=true, Water=true,ZoneGroup = "03"
                },

                new RoleAssignmentMatrix { UserName="BCEZ", Administrative=true, Aging=true, HO=true, Billing=true, Collection=true
                , Company=true, Franchise=true, Garbage=true, JBR=true, Payment=true, Period=true, Pole=true, Rate=true
                , Rentals=true, PassedOnBilling=true, Sewerage=true, SubsidiaryLedger=true, Water=true,ZoneGroup = "06"
                },

                new RoleAssignmentMatrix { UserName="MEZ", Administrative=true, Aging=true, HO=true, Billing=true, Collection=true
                , Company=true, Franchise=true, Garbage=true, JBR=true, Payment=true, Period=true, Pole=true, Rate=true
                , Rentals=true, PassedOnBilling=true, Sewerage=true, SubsidiaryLedger=true, Water=true,ZoneGroup = "09"
                }
            };
            foreach (var temp in roleassignment)
            {
                context.RoleAssignmentMatrix.Add(temp);
            }

            var company = new List<Company>
            {
                new Company { CompanyCode = "ARA14173",CompanyName="Comp 1",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "BAL12406",CompanyName="Comp 2",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "CPV35878",CompanyName="Comp 3",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "EVE15262",CompanyName="Comp 4",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "FIL35894",CompanyName="Comp 5",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "FPI35897",CompanyName="Comp 6",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "L2L35918",CompanyName="Comp 7",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "LUG35919",CompanyName="Comp 8",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "PEZ35946",CompanyName="Comp 9",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now},

                new Company { CompanyCode = "ROB36206",CompanyName="Comp 10",ZoneCode="Code 3",Phase="Phase",Address="Add"
                , VatableItems="YES",Vat=12,Status="Active",EnterpriseType="Zone Enterprise",CreatedBy="",CreateDate=DateTime.Now
                ,OwnershipType="Individual", SendEmail="YES",PrimaryEmailAddress="asdas@asdasd.com",DateOfRegistration=DateTime.Now}
            };
            foreach (var temp in company)
            {
                context.Company.Add(temp);
            }

            context.SaveChanges();
        }
    }
}