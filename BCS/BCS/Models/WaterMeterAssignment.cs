using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public interface IWaterMeterAssignment
    {
         string MeterNumber { get; set; }
         string Size { get; set; }
         string Phase { get; set; }
         DateTime StartDate { get; set; }
         DateTime EndDate { get; set; }
    }
    public class WaterMeterAssignment:IWaterMeterAssignment
    {
        public int WaterMeterAssignmentId { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(20)]
        public string MeterNumber { get; set; }
        [Required]
        [StringLength(20)]
        public string Size { get; set; }
        [Required]
        [StringLength(20)]
        public string Phase { get; set; }
        [Range(0, 1)]
        public int IncludeBilling { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [StringLength(150)]
        public string Createdby { get; set; }
        public DateTime CreateDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(150)]
        public string Status { get; set; }
        public string CompCode { get; set; }
        public string OldWaterMeterAssignmentId { get; set; }
    }
}