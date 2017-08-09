using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class BCSAgingOutput
    {
        public int BCSAgingOutputId { get; set; }
        public int CompanyId { get; set; }
        public double PreviousBalance { get; set; }
        public double PresentBalance { get; set; }
        public double CreditAdjustment { get; set; }
        public double DebitAdjustment { get; set; }
        public int BillOriginMonth { get; set; }
        public int BillOriginYear { get; set; }
        public int CollectionMonth { get; set; }
        public int CollectionYear { get; set; }
        public string CollectionPeriod { get; set; }
        public string BillingType { get; set; }
        public string CompCode { get; set; }
        public double CollectionAmount { get; set; }
    }
}