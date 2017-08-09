using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchMainOrderOfPaymentViewModel
    {
        public SearchMainOrderOfPaymentViewModel()
        {

            this.OrderOfPaymentList = new List<OrderOfPayment>();
            this.OrderOfPaymentDetailList = new List<OrderOfPaymentDetail>();
            this.OrderOfPaymentDetailListTwo = new List<OrderOfPaymentDetail>();
            this.DetailList = new List<OPDetail>();
            this.CompanyList = new List<Company>();
            this.OPAccountList = new List<OPAccount>();
            this.OPAccountDesc = new List<string>();
            this.CompanyName = new List<string>();
            this.CompanyID = new List<string>();
          
            this.ZoneName = new List<string>();
            this.ZoneCode = new List<string>();
            this.Address = new List<string>();

            this.CompanyCode = new List<string>();

        }

        //added 06092017
        public string UserName { get; set; }
        public List<string> CompanyCode { get; set; }

        public string SearchInput { get; set; }

        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public int AutoOPNumber { get; set; }
        public string AutoReferenceNumber { get; set; }
        public List<string> OPAccountDesc { get; set; }
        public List<string> CompanyName { get; set; }
        public List<string> CompanyID { get; set; }
        public List<string> Address { get; set; }
        public List<string> ZoneName { get; set; }
        public List<string> ZoneCode { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<Company> CompanyListTwo { get; set; }

        public List<OrderOfPayment> OrderOfPaymentList = new List<OrderOfPayment>();
        public List<OrderOfPaymentDetail> OrderOfPaymentDetailList = new List<OrderOfPaymentDetail>();
        public List<OrderOfPaymentDetail> OrderOfPaymentDetailListTwo = new List<OrderOfPaymentDetail>();
        public List<OPAccount> OPAccountList = new List<OPAccount>();
        public List<OPDetail> DetailList = new List<OPDetail>();
        public List<Zone> ZoneList = new List<Zone>();
    }
}