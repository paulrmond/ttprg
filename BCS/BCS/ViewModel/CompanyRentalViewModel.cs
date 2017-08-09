using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.ViewModel
{
    public class CompanyRentalViewModel
    {
        public Company company { get; set; }
        public List<RentalInformation> rentalinformation { get; set; }
    }
}