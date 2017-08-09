using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class EOMProcessing
    {
        public int EOMProcessingId { get; set; }
        public int EOMNumber { get; set; } //Identifier. (Act like Billing number)
        public int CompanyId { get; set; }
        public string BillingType { get; set; }
        public decimal? December { get; set; }
        public decimal? November { get; set; }
        public decimal? October { get; set; }
        public decimal? September { get; set; }
        public decimal? August { get; set; }
        public decimal? July { get; set; }
        public decimal? June { get; set; }
        public decimal? May { get; set; }
        public decimal? April { get; set; }
        public decimal? March { get; set; }
        public decimal? February { get; set; }
        public decimal? January { get; set; }
        public decimal? PrevYr1 { get; set; }
        public decimal? PrevYr2 { get; set; }
        public decimal? PrevYr3 { get; set; }
        public decimal? PrevYr4 { get; set; }
        public decimal? OtherPrevYr { get; set; }
        public int? EOMYear { get; set; }
    }
}