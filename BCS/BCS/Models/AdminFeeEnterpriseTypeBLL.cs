using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class AdminFeeEnterpriseTypeBLL
    {
        List<AdminFee> AdminFees;
        string Enterprise;
        public AdminFeeEnterpriseTypeBLL(List<AdminFee> AdminFees, string Enterprise)
        {
            this.AdminFees = AdminFees;
            this.Enterprise = Enterprise;
        }

        public List<AdminFee> AdminFeePerEnterprise()
        {
            List<AdminFee> _AdminFee = new List<AdminFee>();

            foreach (var item in AdminFees)
            {
                var splitZone = item.Zone_Type.Split(' ');
                CheckITEnterpriseAdminFee _CheckITEnterpriseAdminFee = new CheckITEnterpriseAdminFee(splitZone);
                if (Enterprise.ToUpper() == "IT")
                {
                    if (_CheckITEnterpriseAdminFee.HasITWord()) _AdminFee.Add(item);
                }
                else if (Enterprise.ToUpper() == "SEZ")
                {
                    if (_CheckITEnterpriseAdminFee.HasCEZWord()) _AdminFee.Add(item);
                }
                else if (Enterprise.ToUpper() == "OTHERS")
                {
                    if (_CheckITEnterpriseAdminFee.isOther()) _AdminFee.Add(item);
                }
            }
            return _AdminFee;
        }
    }
}