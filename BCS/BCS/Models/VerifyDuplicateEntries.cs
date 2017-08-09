using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public class VerifyDuplicateEntries
    {
        private int CompanyId;
        private int BillingPeriod;
        private string Services;
        public VerifyDuplicateEntries(int CompanyId,int BillingPeriod,string Services)
        {
            this.CompanyId = CompanyId;
            this.BillingPeriod = BillingPeriod;
            this.Services = Services;
        }

        public bool hasDuplicateByBillingPeriod()
        {
            bool hasDuplicate = true;
            BCS_Context db = new BCS_Context();
            if(Services.ToUpper() == "GARBAGE")
            {
                hasDuplicate = db.GarbageInformations.Any(m => m.CompanyId == CompanyId && m.BillingPeriod == BillingPeriod);
            }

            return hasDuplicate;
        }
    }
}