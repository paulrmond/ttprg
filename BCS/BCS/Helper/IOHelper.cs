using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;

namespace BCS.Helper
{
    public static class IOHelper
    {
        public static void LogTxt(string compcode,string billingperiod,string zonetype)
        {
            var a = System.Web.HttpContext.Current.Server.MapPath("~\\Logs\\AdminFeeFailedUploads.txt");
            using (StreamWriter sw = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~\\Logs\\AdminFeeFailedUploads.txt"), true))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + ": Failed to upload ");
                sb.Append(compcode + " at Billingperiod " + billingperiod);
                sb.Append("| Zone: " + zonetype);
                sb.Append(" | Please check if Comp Code exist in current Zone group.");
                sw.WriteLine(sb.ToString());
                
            }
            //    TextWriter tw = File.CreateText("c:\\AdminFeeFailedUploads.txt");
            //StringBuilder sb = new StringBuilder();
            //sb.Append(DateTime.Now + ": Failed to upload ");
            //sb.Append(compcode + " at Billingperiod " + billingperiod);
            //sb.Append("| Zone: " + zonetype);
            
            //tw.WriteLine(sb.ToString());
            //tw.Close();
        }
    }
}