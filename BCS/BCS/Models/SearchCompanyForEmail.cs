using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchCompanyForEmail
    {
        public SearchCompanyForEmail()
        {
            this.companylist = new List<Company>();
            this.mailmodellist = new List<MailModel>();
          
        }
        public List<Company> companylist = new List<Company>();

        public List<MailModel> mailmodellist = new List<MailModel>();
       

    }
}