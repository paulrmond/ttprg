using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public interface IRentalInformation
    {
        int CompanyId { get; set; }
        string Type { get; set; }
        string BillMode { get; set; }
        int DueOn { get; set; }
        decimal Area { get; set; }
        decimal Rate { get; set; }
        decimal Amount { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
    public class RentalInformation : IRentalInformation
    {        
        public int RentalInformationId { get; set; }
        [Range(0, 999999999999999999)]
        public int CompanyId { get; set; }
        [StringLength(150)]
        public string Type { get; set; }
        [StringLength(20)]
        public string BillMode { get; set; }
        [Range(0, 999999999999999999)]
        public int DueOn { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Area { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Rate { get; set; }
        [Range(0, 999999999999999999)]
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(150)]
        public string BillingMonths { get; set; }
        [StringLength(50)]
        //[Required(AllowEmptyStrings = false)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(100)]
        public string Currency { get; set; }
        [StringLength(100)]
        public string Status { get; set; }
        [StringLength(100)]
        public string BillingStatus { get; set; }
        public string CompCode { get; set; }
        public int OldRentalInformationId { get; set; }
    }
}