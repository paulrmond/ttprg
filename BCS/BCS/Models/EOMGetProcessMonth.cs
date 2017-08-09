using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public static class EOMGetProcessMonth
    {
        public static int GetMonth(string zoneGroup)
        {
            int returnMonth = 0;
            BCS_Context db = new BCS_Context();

            var maxBillingId = db.BillingPeriod.Where(m => m.groupCode == zoneGroup).Max(m => m.BillingPeriodId);
            BillingPeriod billingPeriod =  db.BillingPeriod.Where(m=>m.BillingPeriodId == maxBillingId).FirstOrDefault();

            if(billingPeriod != null)
                returnMonth = billingPeriod.Finalized.ToUpper() == "YES" ? billingPeriod.DateTo.Month : billingPeriod.DateFrom.Month;

            return returnMonth;
        }
    }
}