using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BCS.Models
{
    public class SearchReportsCollectionModels
    {


        public SearchReportsCollectionModels()
        {

            this.ZoneGroupList = new List<ZoneGroup>();
            this.ZoneList = new List<Zone>();
            //this.ReportTableList = new List<ReportTable>();
            //this.ReportTableTypeList = new List<ReportTableType>();
            this.RTlist = new List<ReportTable>();
            this.OPAccountList = new List<OPAccount>();
            //this.RepTables = new List<ReportTableBLL>();
            this.systemlogslist = new List<systemlogs>();
            this.Accounts = new List<AccountList>();
        }
        public List<AccountList> Accounts = new List<AccountList>();
        public List<ZoneGroup> ZoneGroupList = new List<ZoneGroup>();
        public List<Zone> ZoneList = new List<Zone>();
        //public List<ReportTable> ReportTableList = new List<ReportTable>();
        //public List<ReportTableType> ReportTableTypeList = new List<ReportTableType>();
        public List<ReportTable> RTlist = new List<ReportTable>();
        public List<OPAccount> OPAccountList = new List<OPAccount>();
        public List<Division> DivisionList = new List<Division>();
        public List<systemlogs> systemlogslist = new List<systemlogs>();

        //public List<ReportTableBLL> RepTables = new List<ReportTableBLL>();



    }
}