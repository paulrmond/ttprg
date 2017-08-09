using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class OPAccount
    {
        public int OPAccountId { get; set; }
        [StringLength(50)]
        public string OPAccountCode { get; set; }
        public string OPAccountDescription { get; set; }
        [Range(0, 999999999999999999.99)]
        public string Alias { get; set; }
        [Range(0, 999999999999999999.99)]
        public decimal? OPAccountFee { get; set; }
        [StringLength(50)]
        public string DivisionCode { get; set; }
        [StringLength(150)]
        public string OPAccountValidity { get; set; }
        [StringLength(50)]
        public string NGASCode { get; set; }
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        [StringLength(150)]
        public string EditedBy { get; set; }
        public DateTime? DateEdited { get; set; }
        [StringLength(50)]
        public string ZoneGroupCode { get; set; }
        [StringLength(100)]
        public string AccountTag { get; set; }
    }
}