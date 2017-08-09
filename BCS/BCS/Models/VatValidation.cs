using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class VatValidation
    {
        int id;
        //string billingType;
        public VatValidation(int id)
        {
            this.id = id;
            //this.billingType = billingType;
        }

        public bool hasVat()
        {
            BCS_Context db = new BCS_Context();
            bool isValid = false;
            string _vatableItems = db.Company.Single(m => m.CompanyID == id).VatableItems;
            if (!string.IsNullOrEmpty(_vatableItems))
            {
                if (_vatableItems.ToUpper() == "YES")
                    isValid = true;
            }
            return isValid;
        }
    }
}