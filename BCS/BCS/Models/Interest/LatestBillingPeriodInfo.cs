using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models.Interest
{
    public class LatestBillingPeriodInfo
    {
        public int BillingPeriod { get; set; }
        public string BillingReference { get; set; }
        public string TransactionReference { get; set; }
    }
}