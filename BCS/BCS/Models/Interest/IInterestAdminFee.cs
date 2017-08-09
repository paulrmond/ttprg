using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Interest
{
    interface IInterestAdminFee
    {
        decimal Interest(string BillingType,int CompanyId, string BillingReference, string ZoneCode, DateTime CoverageFrom, DateTime BillDate, decimal amount);
    }
}
