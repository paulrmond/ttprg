using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{

    public class SearchHolidayTableViewModel
    {

        public SearchHolidayTableViewModel()
        {
            this.HolidayList = new List<HolidayTable>();

        }
        public string ReturnedId { get; set; }
        public List<HolidayTable> HolidayList = new List<HolidayTable>();
    }

}