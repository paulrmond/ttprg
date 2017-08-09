using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class UserDetail
    {
        public int UserDetailId { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string GivenName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}