using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class BillingStatementViewModel
    {
        public List<Zone> zone = new List<Zone>();
        public List<BillingPeriod> billingPeriod = new List<BillingPeriod>();
        public List<string> zoneName = new List<string>();
    }
}