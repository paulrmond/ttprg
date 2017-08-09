using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class DeveloperDetailsViewModel
    {
        public DeveloperDetailsViewModel()
        {
            this.DeveloperId1 = new List<int>();
            this.Dev_Comp_Code1 = new List<string>();
            this.Developer1 = new List<string>();
            this.Ecozone1 = new List<string>();
            this.Zone_Code1 = new List<string>();
        }
        public List<int> DeveloperId1 { get; set; }
        public List<string> Dev_Comp_Code1 { get; set; }
        public List<string> Developer1 { get; set; }
        public List<string> Ecozone1 { get; set; }
        public List<string> Zone_Code1 { get; set; }
    }
}