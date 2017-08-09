using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class BillingRate
    {
        public int BillingRateId { get; set; }
        [StringLength(150)]
        public string Category { get; set; }
        [StringLength(150)]
        public string SubCategory { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? Rate { get; set; }
        [StringLength(150)]
        public string ZoneGroup { get; set; }
        public string NGASCode { get; set; }
    }
}