using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Interest
{
    interface IInterestWithBP
    {
        decimal Interest(string BillingType, int CompanyId, string billingReference, DateTime CoverageFrom, DateTime BillDate, decimal amount, string LastId,decimal currentPrincipalAmount);
    }
}
