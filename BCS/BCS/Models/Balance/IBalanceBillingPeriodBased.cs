using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Balance
{
    interface IBalanceBillingPeriodBased
    {
        decimal Balance(string BillingType, int CompanyId, int MaxBillingPeriod);
    }
}
