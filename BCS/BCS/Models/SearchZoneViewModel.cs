using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchZoneViewModel
    {
        public SearchZoneViewModel()
        {
            this.ZoneList = new List<Zone>();
            this.ZoneGroupList = new List<ZoneGroup>();
        }
        public string ReturnedId { get; set; }
        public List<Zone> ZoneList = new List<Zone>();
        public List<ZoneGroup> ZoneGroupList = new List<ZoneGroup>();
    }
}