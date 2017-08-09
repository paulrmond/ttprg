using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class Division
    {
        public int DivisionId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(300)]
        public string Name { get; set; }
    }
}