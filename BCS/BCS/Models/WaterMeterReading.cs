using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public interface IWaterMeterReading
    {
        decimal BillingPeriod { get; set; }
        decimal PreviousReading { get; set; }
        int PresentReading { get; set; }
    }
    public class WaterMeterReading
    {
        public int WaterMeterReadingId { get; set; }
        [Range(0, 999999999999999999)]
        public int BillingPeriod { get; set; }
        [StringLength(20)]
        public string MeterNumber { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Required]
        [Range(0, 999999999999999999.9999)]
        public decimal PreviousReading { get; set; }
        [Required]
        [Range(0, 999999999999999999.9999)]
        public decimal PresentReading { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public string remarks { get; set; }
        [StringLength(50)]
        public string BillingStatus { get; set; }
        public string CompCode { get; set; }
        public bool? UseThreeMonthsAverage { get; set; }
    }
}