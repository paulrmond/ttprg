namespace BCS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingPeriods",
                c => new
                    {
                        BillingPeriodId = c.Int(nullable: false, identity: true),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        PeriodText = c.String(),
                        Generated = c.String(),
                        Finalized = c.String(),
                    })
                .PrimaryKey(t => t.BillingPeriodId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        CompanyName = c.String(),
                        ZoneCode = c.String(),
                        Phase = c.String(),
                        Address = c.String(),
                        VatableItems = c.String(),
                        Vat = c.String(),
                        WithHolding = c.String(),
                        Status = c.String(),
                        EnterpriseType = c.String(),
                        CreatedBy = c.String(),
                        CreateDate = c.String(),
                        UpdatedBy = c.String(),
                        UpdateDate = c.String(),
                    })
                .PrimaryKey(t => t.CompanyID);
            
            CreateTable(
                "dbo.GeneralBillings",
                c => new
                    {
                        GeneralBillingId = c.Int(nullable: false, identity: true),
                        BillingNumber = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        BillingPeriod = c.Int(nullable: false),
                        BillingDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        BillingType = c.String(),
                        TransactionType = c.String(),
                        BillingReference = c.String(),
                        BillingAmount = c.Double(nullable: false),
                        GenerationDate = c.DateTime(nullable: false),
                        CoverageFrom = c.DateTime(nullable: false),
                        CoverageTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GeneralBillingId);
            
            CreateTable(
                "dbo.RentalInformations",
                c => new
                    {
                        RentalInformationId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Type = c.String(),
                        BillMode = c.Int(nullable: false),
                        DueOn = c.Int(nullable: false),
                        Area = c.Double(nullable: false),
                        Rate = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        BillingMonths = c.String(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RentalInformationId);
            
            CreateTable(
                "dbo.SubsidiaryLedgers",
                c => new
                    {
                        SubsidiaryLedgerId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        BillingPeriod = c.Int(nullable: false),
                        BillingDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        BillingType = c.String(),
                        TransactionType = c.String(),
                        BillingReference = c.String(),
                        TransactionReference = c.String(),
                        DebitAmount = c.Double(nullable: false),
                        CreditAmount = c.Double(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubsidiaryLedgerId);
            
            CreateTable(
                "dbo.WaterMeterAssignments",
                c => new
                    {
                        WaterMeterAssignmentId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        MeterNumber = c.String(),
                        Size = c.String(),
                        Phase = c.String(),
                        IncludeBilling = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Createdby = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.WaterMeterAssignmentId);
            
            CreateTable(
                "dbo.WaterMeterReadings",
                c => new
                    {
                        WaterMeterReadingId = c.Int(nullable: false, identity: true),
                        BillingPeriod = c.Int(nullable: false),
                        MeterNumber = c.String(),
                        CompanyId = c.Int(nullable: false),
                        PreviousReading = c.Int(nullable: false),
                        PresentReading = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.WaterMeterReadingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WaterMeterReadings");
            DropTable("dbo.WaterMeterAssignments");
            DropTable("dbo.SubsidiaryLedgers");
            DropTable("dbo.RentalInformations");
            DropTable("dbo.GeneralBillings");
            DropTable("dbo.Companies");
            DropTable("dbo.BillingPeriods");
        }
    }
}
