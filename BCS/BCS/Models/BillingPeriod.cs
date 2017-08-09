using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class BillingPeriod
    {
        public int BillingPeriodId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        [StringLength(200)]
        public string PeriodText { get; set; }
        [StringLength(10)]
        public string Generated { get; set; }
        [StringLength(5)]
        public string Finalized { get; set; }
        public DateTime? BillingDate { get; set; }
        public DateTime? DueDate { get; set; }
        [StringLength(50)]
        public string groupCode { get; set; }
        public bool? IsPaymentOpen { get; set; }
        public string EOMStatus { get; set; }
    }
}