using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class PoleInformation
    {     
        public int PoleInformationId { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(10)]
        [Required(AllowEmptyStrings = false)]
        public string DueOn { get; set; }
        [StringLength(150)]
        [Required(AllowEmptyStrings = false)]
        public string BillingMonths { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(150)]
        public string Status { get; set; }
        [StringLength(150)]
        public string BillingStatus { get; set; }
        [StringLength(150)]
        public string BillMode { get; set; }
        public string CompCode { get; set; }
        public int OldPoleInformationId { get; set; }
    }
}