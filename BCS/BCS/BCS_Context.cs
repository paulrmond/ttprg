using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class BCS_Context : DbContext
    {

        public BCS_Context() : base()
        { }

        //public DbSet<Entity> Entities { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<RentalInformation> RentalInformation { get; set; }
        public DbSet<GeneralBilling> GeneralBilling { get; set; }
        public DbSet<SubsidiaryLedger> SubsidiaryLedger { get; set; }
        public DbSet<BillingPeriod> BillingPeriod { get; set; }
        public DbSet<WaterMeterAssignment> WaterMeterAssignment { get; set; }
        public DbSet<WaterMeterReading> WaterMeterReading { get; set; }
        public DbSet<FranchiseFeeInformation> FranchiseFeeInformation { get; set; }
        public DbSet<BillingRate> BillingRates { get; set; }
        public DbSet<GarbageInformation> GarbageInformations { get; set; }
        public DbSet<HolidayTable> HolidayTable { get; set; }
        public DbSet<PoleInformation> PoleInformation { get; set; }
        public DbSet<PassedOnBillingInformation> PassedOnBillingInformation { get; set; }
        public DbSet<OrderOfPayment> OrderOfPayment { get; set; }
        public DbSet<OrderOfPaymentDetail> OrderOfPaymentDetail { get; set; }
        public DbSet<OPDetail> Detail { get; set; }
        public DbSet<Balances> Balances { get; set; }
        public DbSet<OPAccount> OPAccount { get; set; }
        public DbSet<OPASubItems> OPASubItems { get; set; }
        public DbSet<ZoneGroup> ZoneGroup { get; set; }
        public DbSet<Zone> Zone { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<AdminFee> AdminFee { get; set; }
        public DbSet<RoleAssignmentMatrix> RoleAssignmentMatrix { get; set; }
        public DbSet<EOMProcessing> EOMProcessing { get; set; }
        public DbSet<NGAS> NGAS { get; set; }
        public DbSet<systemlogs> systemlogs { get; set; }
        public DbSet<Assessment> Assessment { get; set; }
        public DbSet<BCSAgingOutput> BCSAgingOutput { get; set; }
        public DbSet<AssessmentBilling> AssessmentBilling { get; set; }
        public DbSet<AssessmentPayment> AssessmentPayment { get; set; }

        internal void SubmitChanges()
        {
            throw new NotImplementedException();
        }

        //public System.Data.Entity.DbSet<BCS.Models.ViewRentals> ViewRentals { get; set; }
    }
}