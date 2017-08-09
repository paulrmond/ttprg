//using NPOI.HSSF.Converter;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using BCS.Models;

namespace BCS.Helper
{
    public class ExcelHelper
    {
        public List<PropertyInfo> GetProperties(Type type, IEnumerable<string> propertyNames)
        {
            var allProperties = type.GetProperties();
            var properties = new List<PropertyInfo>();

            foreach (var propertyName in propertyNames)
            {
                var property = allProperties.First(p => p.Name == propertyName);

                properties.Add(property);
            }

            return properties;
        }

        public List<T> ReadData<T>(Stream stream, string fileName, List<PropertyInfo> columns, int billperiod, string uploadType,string uploadtype2)
        {
            var list = new List<T>();

            if (fileName.EndsWith("xls"))
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                if (sheet.PhysicalNumberOfRows <= 1)
                    return list;

                if (uploadType == "Billing")
                {
                    var test1 = sheet.GetRow(1).GetCell(3).ToString();
                    BCS_Context db = new BCS_Context();
                    var lst = db.AssessmentBilling.Where(m => m.TRANS_DATE == test1).ToList();
                    db.Database.ExecuteSqlCommand("Delete from AssessmentBillings where TRANS_DATE = '" + test1 + "'");
                    //db.AssessmentBilling.RemoveRange(lst);
                    db.SaveChanges();
                }
                else if (uploadType == "Payment")
                {
                    var test1 = sheet.GetRow(1).GetCell(8).ToString();
                    BCS_Context db = new BCS_Context();
                    var lst = db.AssessmentPayment.Where(m => m.OR_DATE == test1).ToList();
                    db.Database.ExecuteSqlCommand("Delete from AssessmentPayments where OR_DATE = '" + test1 + "'");
                    //db.AssessmentPayment.RemoveRange(lst);
                    db.SaveChanges();
                }
                //else if (uploadType == "Admin")
                //{
                //    BCS_Context db = new BCS_Context();
                //    var lst = db.AdminFee.Where(m => m.BillingPeriodId == billperiod).ToList();
                //    db.Database.ExecuteSqlCommand("delete from AdminFees where BillingPeriodId = '" + billperiod + "'");
                //    //db.AdminFee.RemoveRange(lst);
                //    db.SaveChanges();
                //}



                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var item = Activator.CreateInstance<T>();

                    var row = sheet.GetRow(i);

                    var j = 0;

                    foreach (var column in columns)
                    {
                        if (j == row.Cells.Count)
                        {
                            Type t2 = Nullable.GetUnderlyingType(column.PropertyType) ?? column.PropertyType;
                            column.SetValue(item, Convert.ChangeType(billperiod, t2), null);

                            Type t3 = Nullable.GetUnderlyingType(columns[14].PropertyType) ?? columns[14].PropertyType;
                            columns[14].SetValue(item, Convert.ChangeType(uploadtype2, t3), null);

                            break;
                        }
                        
                        var val = row.GetCell(j).ToString();
                        if(val == "AYALA LAND INC")
                        {

                        }
                        if (val.Contains("\n"))
                        {
                            val = val.Substring(0, val.Length - 1);
                        }

                        j++;

                        if (string.IsNullOrWhiteSpace(val))
                        {
                            continue;
                        }
                        Type t = Nullable.GetUnderlyingType(column.PropertyType) ?? column.PropertyType;
                        column.SetValue(item, Convert.ChangeType(val, t), null);
                    }

                    list.Add(item);
                }
            }
            else if (fileName.EndsWith("xlsx"))
            {
                var workbook = new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int i = 0; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var item = Activator.CreateInstance<T>();

                    var row = sheet.GetRow(i);

                    var j = 0;

                    foreach (var column in columns)
                    {
                        var val = row.GetCell(j).StringCellValue;

                        if (string.IsNullOrWhiteSpace(val))
                        {
                            continue;
                        }
                        Type t = Nullable.GetUnderlyingType(column.PropertyType) ?? column.PropertyType;
                        column.SetValue(item, Convert.ChangeType(val, t), null);

                        j++;
                    }

                    list.Add(item);
                }
            }

            return list;
        }

        public HSSFWorkbook CreateXls<T>(List<T> items, List<PropertyInfo> properties)
        {
            // Create the workbook, sheet, and row
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var i = 0;

            // Header row
            {
                var j = 0;

                var row = sheet.CreateRow(i);

                foreach (var property in properties)
                {
                    row.CreateCell(j).SetCellValue(property.Name);

                    j++;
                }

                i++;
            }

            foreach (var item in items)
            {
                var j = 0;

                var row = sheet.CreateRow(i);

                foreach (var property in properties)
                {
                    var value = property.GetValue(item);

                    if (value != null)
                    {
                        row.CreateCell(j).SetCellValue(value.ToString());
                    }

                    j++;
                }

                i++;
            }

            return workbook;
        }

        public XSSFWorkbook CreateXlsx<T>(List<T> items, List<PropertyInfo> properties)
        {
            // Create the workbook, sheet, and row
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var i = 0;

            // Header row
            {
                var j = 0;

                var row = sheet.CreateRow(i);

                foreach (var property in properties)
                {
                    row.CreateCell(j).SetCellValue(property.Name);

                    j++;
                }

                i++;
            }

            foreach (var item in items)
            {
                var j = 0;

                var row = sheet.CreateRow(i);

                foreach (var property in properties)
                {
                    var value = property.GetValue(item);

                    if (value != null)
                    {
                        row.CreateCell(j).SetCellValue(value.ToString());
                    }

                    j++;
                }

                i++;
            }

            return workbook;
        }
    }
}