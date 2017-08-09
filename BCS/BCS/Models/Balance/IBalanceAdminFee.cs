using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Balance
{
    interface IBalanceAdminFee
    {
        decimal Balance(string BillingType, string BillingReference, string ZoneCode,int MaxBillingPeriod);
    }
}
