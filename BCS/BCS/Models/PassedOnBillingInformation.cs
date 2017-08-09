using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class PassedOnBillingInformation
    {
        public int PassedOnBillingInformationId { get; set; }
        [StringLength(50)]
        public string Type { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriod { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Amount { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime OriginDate { get; set; }
        public DateTime BillingDate { get; set; }
        [StringLength(150)]
        public string Status { get; set; }
        [StringLength(150)]
        public string BillingStatus { get; set; }
        public string Remarks { get; set; }

    }
}