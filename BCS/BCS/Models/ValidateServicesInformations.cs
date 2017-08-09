using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class ValidateServicesInformations
    {
        string billingMonths;
        DateTime startDate;
        DateTime endDate;
        DateTime coverageFrom;
        DateTime coverageTo;
        public int multiplier = 1; //Month difference
        string ServicesType;
        public ValidateServicesInformations(string billingMonths, DateTime startDate, DateTime endDate, DateTime coverageFrom, DateTime coverageTo)
        {
            this.billingMonths = billingMonths;
            this.startDate = startDate;
            this.endDate = endDate;
            this.coverageFrom = coverageFrom;
            this.coverageTo = coverageTo;
        }

        public ValidateServicesInformations(string billingMonths, DateTime startDate, DateTime endDate, DateTime coverageFrom, DateTime coverageTo, string ServicesType)
        {
            this.billingMonths = billingMonths;
            this.startDate = startDate;
            this.endDate = endDate;
            this.coverageFrom = coverageFrom;
            this.coverageTo = coverageTo;
            this.ServicesType = ServicesType;
        }

        private string SortBillingMonths()
        {
            //BUBBLE SORT
            string[] ab = billingMonths.Split(',');
            int[] num = new int[ab.Length];
            bool isSorted = true;

            for (int i = 0; i < ab.Length; i++) { num[i] = int.Parse(ab[i]); } //parse array of string to int

            while (isSorted) //loop until array is sorted
            {
                isSorted = false;
                for (int i = 0; i < ab.Length - 1; i++)
                {
                    if (num[i] > num[i + 1])
                    {
                        int temp = num[i];
                        num[i] = num[i + 1];
                        num[i + 1] = temp;

                        isSorted = true;
                    }
                }
            }

            return string.Join(",", num);
        }

        public bool ValidateBillingMonths()
        {
            bool isValid = true;
            string[] ab = billingMonths.Split(',');
            //int[] num = new int[ab.Length];
            for (int i = 0; i < ab.Length; i++)
            {
                int num = int.Parse(ab[i]);
                if(num < 1 || num > 12)
                {
                    isValid = false;
                    break;
                }
            }



            return isValid;
        }

        public bool isValidInformation()
        {
            if (ValidateBillingMonths())
            {
                SortBillingMonths(); //sort the billing months
                bool isValid = false;

                List<DateTime> arrayOfBillingMonths = new List<DateTime>();
                int day = startDate.Day;
                int startMonth = startDate.Month;
                int yeardiff = endDate.Year - startDate.Year;
                //string[] billm = billingMonths.Split(',');
                string bm = SortBillingMonths();
                string[] billm = bm.Split(',');
                int curryear = startDate.Year;
                int endMonth = endDate.Month;
                int tempYearDiff = yeardiff;
                for (int i = 0; i <= yeardiff; i++) //Get all the billing date
                {
                    foreach (var item in billm)
                    {
                        var billingMonthValue = int.Parse(item);
                        DateTime dt = new DateTime();
                        if (tempYearDiff == 0)
                        {
                            if (billingMonthValue <= endMonth)
                            {
                                if(day > 28 && item == "2") //If Day > 28 and The current month is february
                                     dt = Convert.ToDateTime(item + "/28/" + curryear);
                                else
                                     dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);

                                arrayOfBillingMonths.Add(dt);
                            }                            
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if(billingMonthValue >= startMonth)
                                {
                                    dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);
                                    arrayOfBillingMonths.Add(dt);
                                }                             
                            }   
                            else
                            {
                                dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);
                                arrayOfBillingMonths.Add(dt);
                            }                             
                        }
                    }
                    curryear++;
                    if(tempYearDiff > 0)
                        tempYearDiff -= 1;
                }

                if (arrayOfBillingMonths[0] < startDate && yeardiff > 0) //Start date must be greater than the first billing date
                    arrayOfBillingMonths.Insert(0, startDate);
                //arrayOfBillingMonths[0] = startDate;

                if (arrayOfBillingMonths[arrayOfBillingMonths.Count - 1] < endDate) //if end date not sync with billing months = "ADD"
                    arrayOfBillingMonths.Add(endDate);

                for (int num = 0; num < arrayOfBillingMonths.Count; num++) //Check if has valid billing months
                {
                    if (arrayOfBillingMonths[num] >= coverageFrom && arrayOfBillingMonths[num] <= coverageTo)
                    {
                        if (num > 0) //If greater than 0 = get previous billing date
                        {
                            isValid = true;
                            if (!string.IsNullOrEmpty(ServicesType) && billingMonths == "1") //if annual franchise
                                if (num < arrayOfBillingMonths.Count - 1) //in annual franchise do not bill last index. (Bill first before consume)
                                    multiplier = ((arrayOfBillingMonths[num + 1].Year - arrayOfBillingMonths[num].Year) * 12) + (arrayOfBillingMonths[num + 1].Month - arrayOfBillingMonths[num].Month);
                                else
                                    isValid = false;
                            else
                                multiplier = ((arrayOfBillingMonths[num].Year - arrayOfBillingMonths[num - 1].Year) * 12) + (arrayOfBillingMonths[num].Month - arrayOfBillingMonths[num - 1].Month);

                            break;
                        }
                        else // if zero. MULTIPLIER = (START DATE) - (FIRST BILLING DATE)
                        {
                            if (!string.IsNullOrEmpty(ServicesType) && billingMonths == "1")//if annual franchise
                                if (num < arrayOfBillingMonths.Count - 1) //in annual franchise do not bill last index. (Bill first before consume
                                    multiplier = ((startDate.Year - arrayOfBillingMonths[num + 1].Year) * 12) + (startDate.Month - arrayOfBillingMonths[num + 1].Month);
                                else
                                {
                                    isValid = false;
                                    break;
                                }
                            else
                                multiplier = ((startDate.Year - arrayOfBillingMonths[num].Year) * 12) + (startDate.Month - arrayOfBillingMonths[num].Month);

                            isValid = multiplier == 0 ? false : true;
                            break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                if (multiplier == 0)
                    multiplier = 1;

                multiplier = Math.Abs(multiplier);
                return isValid;
            }
            else
            {
                return false;
            }
        }


        public bool isValidInformationFranchise()
        {
            if (ValidateBillingMonths())
            {
                SortBillingMonths(); //sort the billing months
                bool isValid = false;

                List<DateTime> arrayOfBillingMonths = new List<DateTime>();
                int day = startDate.Day;
                int startMonth = startDate.Month;
                int yeardiff = endDate.Year - startDate.Year;
                //string[] billm = billingMonths.Split(',');
                string bm = SortBillingMonths();
                string[] billm = bm.Split(',');
                int curryear = startDate.Year;
                int endMonth = endDate.Month;
                int tempYearDiff = yeardiff;
                for (int i = 0; i <= yeardiff; i++) //Get all the billing date
                {
                    foreach (var item in billm)
                    {
                        var billingMonthValue = int.Parse(item);
                        DateTime dt = new DateTime();
                        if (tempYearDiff == 0)
                        {
                            if (billingMonthValue <= endMonth)
                            {
                                if (day > 28 && item == "2") //If Day > 28 and The current month is february
                                    dt = Convert.ToDateTime(item + "/28/" + curryear);
                                else
                                    dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);

                                arrayOfBillingMonths.Add(dt);
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if (billingMonthValue >= startMonth)
                                {
                                    dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);
                                    arrayOfBillingMonths.Add(dt);
                                }
                                else if(billm.Count() == 1)
                                {
                                    dt = Convert.ToDateTime(startMonth + "/" + day + "/" + curryear);
                                    arrayOfBillingMonths.Add(dt);
                                }
                            }
                            else
                            {
                                dt = Convert.ToDateTime(item + "/" + day + "/" + curryear);
                                arrayOfBillingMonths.Add(dt);
                            }
                        }
                    }
                    curryear++;
                    if (tempYearDiff > 0)
                        tempYearDiff -= 1;
                }


                if (arrayOfBillingMonths[arrayOfBillingMonths.Count - 1] < endDate) //if end date not sync with billing months = "ADD"
                    arrayOfBillingMonths.Add(endDate);

                for (int num = 0; num < arrayOfBillingMonths.Count -1; num++) //Check if has valid billing months
                {
                    if (arrayOfBillingMonths[num] >= coverageFrom && arrayOfBillingMonths[num] <= coverageTo)
                    {
                        DateTime currentDt = arrayOfBillingMonths[num];
                        if (num == 0 && currentDt.Month != 1) //If first billing and Not January DO NOT BILL... Next billing generation should be 12 + ProRated
                        {
                            isValid = false;                            
                            break;
                        }
                        else // if zero. MULTIPLIER = (START DATE) - (FIRST BILLING DATE)
                        {
                            if(num == 1)
                            {
                                multiplier = ((arrayOfBillingMonths[num].Year - arrayOfBillingMonths[num -1].Year) * 12) + (arrayOfBillingMonths[num].Month - arrayOfBillingMonths[num -1].Month);
                                multiplier += ((arrayOfBillingMonths[num + 1].Year - arrayOfBillingMonths[num].Year) * 12) + (arrayOfBillingMonths[num + 1].Month - arrayOfBillingMonths[num].Month);
                            }
                            else
                            {
                                multiplier = ((arrayOfBillingMonths[num + 1].Year - arrayOfBillingMonths[num].Year) * 12) + (arrayOfBillingMonths[num + 1].Month - arrayOfBillingMonths[num].Month);
                            }
                           
                            isValid = multiplier == 0 ? false : true;
                            break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                if (multiplier == 0)
                    multiplier = 1;

                multiplier = Math.Abs(multiplier);
                return isValid;
            }
            else
            {
                return false;
            }
        }
    }
}