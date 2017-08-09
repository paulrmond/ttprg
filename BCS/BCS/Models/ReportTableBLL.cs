using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public  class ReportTableBLL
    {
        //ApplicationDbContext context = new ApplicationDbContext();
        //BCS_Context db = new BCS_Context();
        //List<ReportTable> listOfReportTable = new List<ReportTable>();
        //List<ReportTableType> listOfReportTableType = new List<ReportTableType>();

        //ReportTableType rpt = new ReportTableType();
        //Zones


        public List<ReportTable> RTTables()
        {
            List<ReportTable> listOfReportTable = new List<ReportTable>
            {
            //ReportTableTypeId 1 = Collection 2 = Official Receipt 3 = Others
                //1
                new ReportTable { ReportTableId=1, ReportTableName="Monthly Report Collection", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=2, ReportTableName="Monthly Summary Collection Per Account", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=3, ReportTableName="Final Value Added Tax Withheld By Payor (Form 2306)", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=4, ReportTableName="Collection Report GrossNet", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = true, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                new ReportTable { ReportTableId=5, ReportTableName="Collection Report Interest", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = true, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                new ReportTable { ReportTableId=6, ReportTableName="Expanded WithHolding Tax WithHeld By Payor", ReportTableTypeId=1  ,NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=7, ReportTableName="Collection Report VAT", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                //new ReportTable { ReportTableId=8, ReportTableName="Account Description", ReportTableTypeId=1  , BillingPeriodTB=false,CreditARTB=false,TransactionDateTB=false},
                //new ReportTable { ReportTableId=9, ReportTableName="Summary Collection(Daily Summary in a Month)", ReportTableTypeId=1  , BillingPeriodTB=false,CreditARTB=true,TransactionDateTB=false},
                new ReportTable { ReportTableId=10, ReportTableName="Monthly Summary of Deposits", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=11, ReportTableName="Monthly Summary Report Collection Per Revenue Item", ReportTableTypeId=1  ,NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=12, ReportTableName="Comparative Summary Report Per Revenue Item", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                //new ReportTable { ReportTableId=13, ReportTableName="Billing Collection Report Daily(USD)", ReportTableTypeId=1  , BillingPeriodTB=false,CreditARTB=false,TransactionDateTB=true},
                new ReportTable { ReportTableId=31, ReportTableName="Billing Collection Report Monthly Daily(USD)", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                
                ////2
                new ReportTable { ReportTableId=13, ReportTableName="List of Check Received", ReportTableTypeId=2  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=14, ReportTableName="List of Cancelled Official Receipt", ReportTableTypeId=2  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                //new ReportTable { ReportTableId=15, ReportTableName="Payment Details", ReportTableTypeId=2  , BillingPeriodTB=false,CreditARTB=true,TransactionDateTB=true},
                new ReportTable { ReportTableId=16, ReportTableName="Cash Receipts And Deposits Record", ReportTableTypeId=2  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=17, ReportTableName="Issued OR", ReportTableTypeId=2  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=18, ReportTableName="ORs with 121", ReportTableTypeId=2  ,NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
               
                //3
                new ReportTable { ReportTableId=19, ReportTableName="Accountable Forms", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=20, ReportTableName="List of Collection Per Account Code", ReportTableTypeId=3  , NatureCollectionWithARS = true, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                new ReportTable { ReportTableId=21, ReportTableName="Serial Number", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=22, ReportTableName="Report of Payment - Engineering Fees", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                // Newly Added Reports
                //3
                new ReportTable { ReportTableId=23, ReportTableName="VISA Application Report", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=24, ReportTableName="Accountability for Accountable Forms Report", ReportTableTypeId=3  ,NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = true, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                //1
                new ReportTable { ReportTableId=25, ReportTableName="Dollar Payments Collection", ReportTableTypeId=1  ,  NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=26, ReportTableName="List of Collection Per Unit Responsibility Center", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = true},
                //new ReportTable { ReportTableId=27, ReportTableName="Statement Of Account(Payment)", ReportTableTypeId=3  , BillingPeriodTB=false,CreditARTB=true,TransactionDateTB=false},
                //3
                new ReportTable { ReportTableId=28, ReportTableName="Advances AlphaList", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=29, ReportTableName="Order of Payment Summary Report", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = true, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                new ReportTable { ReportTableId=30, ReportTableName="Account Description", ReportTableTypeId=3  , NatureCollectionWithARS = true, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = false , TransactionDateDivision = false},
                new ReportTable { ReportTableId=31, ReportTableName="VAT Alphalist", ReportTableTypeId=3  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},

                //1
                new ReportTable { ReportTableId=32, ReportTableName="List of Collection Due to Head Office (421)", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                new ReportTable { ReportTableId=33, ReportTableName="List of Collection Due from Other Zones (142)", ReportTableTypeId=1  , NatureCollectionWithARS = false, AccountRecievableBillings = false, NatureCollectionAccountableForms = false, NatureCollection = false, BillingPeriods = false, TransactionDate = true , TransactionDateDivision = false},
                
            };

            return listOfReportTable;
        }



    }
}