using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCS.Reports
{
    public partial class CompanyReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string opnum = Request.QueryString["opnum"].ToString();
            System.Uri opnumuri = new System.Uri("http://dev4-pc/ReportServer_DCIDEVDB");
            ReportViewer1.ServerReport.ReportPath = "/Reports/OP_EDD_HO&rc:Parameter&OPNumber=00000001";
            ReportViewer1.ServerReport.ReportServerUrl = opnumuri;

            //ReportParameter[] rpt = new ReportParameter[2];
            //rpt[0] = new ReportParameter("@OPNumber", "00000001");

            //ReportViewer1.ServerReport.SetParameters(rpt); 
        }
    }
}