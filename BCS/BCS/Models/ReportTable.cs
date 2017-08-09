using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class ReportTable
    {

        public int ReportTableId { get; set; }

        //OLD TABLES
        public string ReportTableName { get; set; }
        public int ReportTableTypeId { get; set; }
        public bool BillingPeriodTB { get; set; }
        public bool CreditARTB { get; set; }
        public bool TransactionDateTB { get; set; }


        //NEW TABLES
        public bool NatureCollectionWithARS { get; set; }
        public bool AccountRecievableBillings { get; set; }
        public bool NatureCollectionAccountableForms { get; set; }
        public bool NatureCollection { get; set; }
        public bool BillingPeriods { get; set; }
        public bool TransactionDate { get; set; }
        public bool TransactionDateDivision { get; set; }


    }
}