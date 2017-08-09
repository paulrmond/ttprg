using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class systemlogs
    {
        public int systemlogsId { get; set; }
        public string ZoneGroupCode { get; set; }
        public string UserName { get; set; }
        public string remarks { get; set; }
        public DateTime timestamp { get; set; }
        public string AreaAccessed { get; set; }
        public string loglevel { get; set; }

    }
}