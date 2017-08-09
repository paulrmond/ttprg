using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class Zone
    {
        public int ZoneId { get; set; }
        [StringLength(10)]
        public string ZoneCode { get; set; }
        [StringLength(100)]
        public string ZoneName { get; set; }
        [StringLength(100)]
        public string ZoneGroup { get; set; }    
        public string ZoneType { get; set; }
        public string ZoneNature { get; set; }    
    }
}