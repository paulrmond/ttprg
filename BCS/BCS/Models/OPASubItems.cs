using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class OPASubItems
    {
        public int OPASubItemsId { get; set; }
        public int OPAccountId { get; set; }
        public string Code { get; set; }
        public string NGASCode { get; set; }
        public string Description { get; set; }
        public decimal Fee { get; set; }
    }
}