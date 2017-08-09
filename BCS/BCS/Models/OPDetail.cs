using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class OPDetail
    {
        public int OPDetailId { get; set; }
        public int OrderOfPaymentId { get; set; }
        public int OrderOfPaymentDetailId { get; set; }
        [StringLength(50)]
        public string GivenName { get; set; }
        [StringLength(50)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        public string SurName { get; set; }
        [StringLength(50)]
        public string VisaName { get; set; }
        public string Nationality { get; set; }
        public string Description { get; set; }
        public int? OPStart { get; set; }
        public int? OPEnd { get; set; }
        [StringLength(200)]
        public string Pending { get; set; }

        public string SN1 { get; set; }
        public string SN2 { get; set; }
        public string SN3 { get; set; }
        public string SN4 { get; set; }
        public string SN5 { get; set; }
        public string SN6 { get; set; }
        public string SN7 { get; set; }
        public string SN8 { get; set; }
        public string SN9 { get; set; }
        public string SN10 { get; set; }
        public string SN11 { get; set; }
        public string SN12 { get; set; }
        public string SN13 { get; set; }
        public string SN14 { get; set; }
        public string SN15 { get; set; }

    }
}