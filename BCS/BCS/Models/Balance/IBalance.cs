using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Balance
{
    interface IBalance
    {
        decimal Balance(string BillingType,string BillingReference,int CompanyId,int MaxBillingPeriod);
    }
}
