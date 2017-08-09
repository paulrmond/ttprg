using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        [StringLength(20)]
        public string CompanyCode { get; set; }
        [StringLength(250)]
        public string CompanyName { get; set; }
        [StringLength(10)]
        public string ZoneCode { get; set; }
        [StringLength(10)]
        public string Phase { get; set; }
        public string Address { get; set; }
        [StringLength(5)]
        public string VatableItems { get; set; }
        public int? Vat { get; set; }
        [StringLength(5)]
        public string WithHolding { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(50)]
        public string EnterpriseType { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        [StringLength(30)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(20)]
        public string OwnershipType { get; set; }
        [StringLength(50)]
        public string SendEmail { get; set; }
        [StringLength(50)]
        public string PrimaryEmailAddress { get; set; }
        [StringLength(50)]
        public string SecondaryEmailAddress { get; set; }
        public DateTime? DateOfRegistration { get; set; }
        [StringLength(20)]
        public string TypeCode { get; set; }
    }
}