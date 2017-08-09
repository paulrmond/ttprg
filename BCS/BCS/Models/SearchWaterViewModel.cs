using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class SearchWaterViewModel
    {
        public string ReturnedId { get; set; }
        public List<Company> Companies = new List<Company>();
        public List<WaterMeterAssignment> WaterMeterAssignments = new List<WaterMeterAssignment>();
        public List<WaterMeterReading> WaterMeterReadings = new List<WaterMeterReading>();
        public List<BillingPeriod> BillingPeriods = new List<BillingPeriod>();
    }
}