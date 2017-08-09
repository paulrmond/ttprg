using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;


namespace BCS.Models
{
    public class SearchReportsBillingStatementModels
    {


        public SearchReportsBillingStatementModels()
        {
            this.ZoneGroupList = new List<ZoneGroup>();
            this.ZoneList = new List<Zone>();
            this.BillingPeriodList = new List<BillingPeriod>();
            this.Companylist = new List<Company>();
            this.EnterpriseTypeList = new List<EnterpriseTypes>();
        }

        public List<ZoneGroup> ZoneGroupList = new List<ZoneGroup>();
        public List<Zone> ZoneList = new List<Zone>();
        public List<BillingPeriod> BillingPeriodList = new List<BillingPeriod>();
        public List<Company> Companylist = new List<Company>();
        public List<EnterpriseTypes> EnterpriseTypeList = new List<EnterpriseTypes>();

    }
}