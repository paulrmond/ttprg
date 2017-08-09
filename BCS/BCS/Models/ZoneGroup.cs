using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class ZoneGroup
    {
        public int ZoneGroupId { get; set; }
        [StringLength(20)]
        public string ZoneGroupCode { get; set; }
        [StringLength(100)]
        public string ZoneGroupName { get; set; }
        [StringLength(300)]
        public string ZoneGroupAddress { get; set; }
        [StringLength(150)]
        public string ZoneGroupRole { get; set; }
        [StringLength(50)]
        public string ZoneGroupInitials { get; set; }
    }
}