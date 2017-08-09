using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class OrderOfPaymentDetail
    {
        public int OrderOfPaymentDetailId { get; set; }
        public int OrderOfPaymentId { get; set; }
        public int CompanyId { get; set; }
        public int OPAccountId { get; set; }
        [Range(0, 999999999999999999)]
        public int Quantity { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal Amount { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal TotalAmount { get; set; }
        public decimal WithHoldingTaxAmount { get; set; }
        [StringLength(50)]
        public string Representative { get; set; }
        public string Remarks { get; set; }
        [StringLength(50)]
        public string AccountTag { get; set; }
        public string Withholding { get; set; }
        public string ATC { get; set; }
        public string ATCRates { get; set; }
        public string OPAccountCode { get; set; }
    }
}