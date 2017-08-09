using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public interface IGarbageInformation
    {
         int CompanyId { get; set; }
         int BillingPeriod { get; set; }
         string Type { get; set; }
         decimal Weight { get; set; }
         decimal Rate { get; set; }
         DateTime CollectionDate { get; set; }
    }
    public class GarbageInformation : IGarbageInformation
    {
        public int GarbageInformationId { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriod { get; set; }
        [StringLength(150)]
        public string Type { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Weight { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Rate { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime CollectionDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(150)]
        public string BillingStatus { get; set; }
        public string CompCode { get; set; }
    }
}