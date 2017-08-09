using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class AdminFee
    {
        public int AdminFeeId { get; set; }
        public string Ecozone { get; set; }
        public string Zone_Type { get; set; }
        public string Company_Name { get; set; }
        public string Enterprise_Type { get; set; }
        public string Employment { get; set; }
        public string Zone_Code { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Comp_Code { get; set; }
        public string Developer { get; set; }
        public string Dev_Comp_Code { get; set; }
        public string Total_Locators { get; set; }
        public string Total_Employment { get; set; }
        public int BillingPeriodId { get; set; }
        public string Upload_Type { get; set; }
    }
}