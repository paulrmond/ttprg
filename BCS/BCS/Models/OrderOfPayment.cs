using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class OrderOfPayment
    {
        public int OrderOfPaymentId { get; set; }
        [StringLength(20)]
        public string OPNumber { get; set; }
        public string CompanyCode { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string DivisionCode { get; set; }
        [StringLength(150)]
        public string ZoneGroupcode { get; set; }
        public DateTime? OPDate { get; set; }
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        [StringLength(100)]
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        [StringLength(200)]
        public string PayOrigin { get; set; }
        public string ORNumber { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal TotalAmount { get; set; }
    }
}