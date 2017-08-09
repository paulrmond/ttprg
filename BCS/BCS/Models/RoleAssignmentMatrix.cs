using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class RoleAssignmentMatrix
    {
        public int RoleAssignmentMatrixId { get; set; }
        [StringLength(20)]
        public string UserName { get; set; }
        [StringLength(20)]
        public string ZoneGroup { get; set; }
        public bool? Water { get; set; }
        public bool? Sewerage { get; set; }
        public bool? Rentals { get; set; }
        public bool? Garbage { get; set; }
        public bool? Pole { get; set; }
        public bool? Administrative { get; set; }
        public bool? PassedOnBilling { get; set; }
        public bool? Franchise { get; set; }
        public bool? Billing { get; set; }
        public bool? Payment { get; set; }
        public bool? HO { get; set; }
        public bool? Company { get; set; }
        public bool? Rate { get; set; }
        public bool? Period { get; set; }
        //public bool? Advances { get; set; }
        public bool? JBR { get; set; }
        public bool? SubsidiaryLedger { get; set; }
        //public bool? VAT { get; set; }
        //public bool? Journal { get; set; }
        //public bool? Statement { get; set; }
        public bool? Collection { get; set; }
        //public bool? Ledger { get; set; }
        public bool? Aging { get; set; }
        public dynamic Security { get; internal set; }
    }
}