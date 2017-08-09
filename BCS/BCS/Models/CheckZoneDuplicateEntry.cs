using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class CheckZoneDuplicateEntry
    {
        string name;
        string zonegroup;
        string zonegroupname;
        public CheckZoneDuplicateEntry(string name,string zonegroup,string zonegroupname)
        {
            this.name = name;
            this.zonegroup = zonegroup;
            this.zonegroupname = zonegroupname;
        }

        public CheckZoneDuplicateEntry(string name, string zonegroup)
        {
            this.name = name;
            this.zonegroup = zonegroup;
        }

        public bool hasDuplicateEntry()
        {
            bool hasDup = false;
            BCS_Context db = new BCS_Context();
            if(!string.IsNullOrEmpty(zonegroupname))
            {
                if (zonegroupname.ToUpper() != name.ToUpper())
                    hasDup = db.Zone.Any(m => m.ZoneName.ToUpper() == name.ToUpper() && m.ZoneGroup == zonegroup);
            }
            else
                hasDup = db.Zone.Any(m => m.ZoneName.ToUpper() == name.ToUpper() && m.ZoneGroup == zonegroup);

            return hasDup;
        }
    }
}