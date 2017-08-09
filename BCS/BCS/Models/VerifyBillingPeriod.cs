using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    //The role is not to generate billing if previous period already generate billing.
    public class VerifyBillingPeriod
    {
        int id;
        string ZoneGroup;
        public VerifyBillingPeriod(int id, string ZoneGroup)
        {
            this.id = id;
            this.ZoneGroup = ZoneGroup;
        }

        public bool canGenerate()
        {
            bool isVerified = false;
            BCS_Context db = new BCS_Context();
            List<BillingPeriod> bill = new List<BillingPeriod>();
            bill = db.BillingPeriod.OrderByDescending(m => m.BillingPeriodId).Where(m => m.groupCode == ZoneGroup).ToList();
            var indexNumber = bill.IndexOf(bill.SingleOrDefault(m => m.BillingPeriodId == id));

            if (indexNumber > 0)
            {
                BillingPeriod newBill = bill[indexNumber - 1];
                if (newBill.Generated.ToUpper() == "NO")
                    isVerified = true;
            }
            else
            {
                isVerified = true;
            }

            return isVerified;
        }
    }
}