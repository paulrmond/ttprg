using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchZoneGroupViewModel
    {
        public SearchZoneGroupViewModel()
        {
            this.ZoneGroupList = new List<ZoneGroup>();

        }
        public string ReturnedId { get; set; }
        public List<ZoneGroup> ZoneGroupList = new List<ZoneGroup>();
    }
}