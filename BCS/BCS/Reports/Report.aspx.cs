using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace BCS.Reports
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string ReportServerPath = ConfigurationManager.AppSettings["ReportServerPath"].ToString();

                Microsoft.Reporting.WebForms.ReportParameter[] ReportParameters;
                var reportTypeValue = Request.QueryString["reportType"].ToString();
                var zoneGroupCode = Request.QueryString["zoneGroupCode"].ToString();
                var developer = "";
                var groupedStartDate = "";
                var groupedEndDate = "";
                var groupedARType = "";
                var tdStartDate = "";
                var tdEndDate = "";
                var statusType = "";


                switch (reportTypeValue)
                {
                    // Data Entries

                    case "RentalAlphaList":
                        System.Uri rentaluri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/RentalAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = rentaluri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "PoleRentalAlphaList":
                        System.Uri poleRentaluri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/PoleRentalAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = poleRentaluri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "GarbageCollectionAlphaList":
                        System.Uri garbageCollectionuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/GarbageCollectionAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = garbageCollectionuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "WaterMeterAlphalist":
                        System.Uri waterMeteruri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/WaterMeterAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = waterMeteruri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "WaterReadingAlphaList":
                        System.Uri waterReadinguri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/WaterReadingAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = waterReadinguri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "FranchiseAlphaList":
                        System.Uri franchiseuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/FranchiseAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = franchiseuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "AdminFeeAlphaList":
                        System.Uri adminFeeuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/AdminFeeAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = adminFeeuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("developer", developer);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "AdminFeeMonthlyReport":
                        System.Uri adminFeeMonthlyguri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/AdminFeeMonthlyReport";
                        ReportViewer1.ServerReport.ReportServerUrl = adminFeeMonthlyguri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "PassedOnBilling":
                        System.Uri securityGuarduri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/DataEntries/PassedOnBilling";
                        ReportViewer1.ServerReport.ReportServerUrl = securityGuarduri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // General Billing, Order of Payments, HO Batch Update

                    case "OrderofPaymentSlip":
                        var opNumber = Request.QueryString["opnum"].ToString();
                        System.Uri opSlipuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Billing/OrderofPaymentSlip";
                        ReportViewer1.ServerReport.ReportServerUrl = opSlipuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("opNumber", opNumber);
                        ReportParameters[1] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "OrderofPaymentSummaryReport":
                        string preparedby = ConfigurationManager.AppSettings["PreparedBy"].ToString();
                        System.Uri opSummary = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Billing/OrderofPaymentSummaryReport";
                        ReportViewer1.ServerReport.ReportServerUrl = opSummary;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("Preparedby", preparedby);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "HOBatchUpdate":
                        break;
                        
                    // Reports

                    case "BillingStatement":
                        var generatType = Request.QueryString["zoneGroupCode"].ToString();
                        string filteredBy = ConfigurationManager.AppSettings["PreparedBy"].ToString();
                        string receivedBy = ConfigurationManager.AppSettings["PreparedBy"].ToString();
                        string approvedBy = ConfigurationManager.AppSettings["PreparedBy"].ToString();
                        System.Uri generalBillinguri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/BillingStatement";
                        ReportViewer1.ServerReport.ReportServerUrl = generalBillinguri;

                        ReportParameters = new ReportParameter[6];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("generateType", generatType);
                        ReportParameters[2] = new ReportParameter("entrpType", zoneGroupCode);
                        ReportParameters[3] = new ReportParameter("filteredBy", zoneGroupCode);
                        ReportParameters[4] = new ReportParameter("ReceivedBy", zoneGroupCode);
                        ReportParameters[5] = new ReportParameter("ApprovedBy", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Monthly Aging

                    case "AgingofAccountsReceivable":
                        System.Uri JBRAccountsuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Aging/AgingofAccountsReceivable";
                        ReportViewer1.ServerReport.ReportServerUrl = JBRAccountsuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("statusType", statusType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlyAgingSummary":
                        System.Uri MonthlyAgingSummarysuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Aging/MonthlyAgingSummary";
                        ReportViewer1.ServerReport.ReportServerUrl = MonthlyAgingSummarysuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("statusType", statusType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlyAgingByMonth":
                        System.Uri MonthlyAgingByMonthsuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Aging/MonthlyAgingByMonth";
                        ReportViewer1.ServerReport.ReportServerUrl = MonthlyAgingByMonthsuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("statusType", statusType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlyAgingPriorwithYears":
                        System.Uri MonthlyAgingPriorwithYearsuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Aging/MonthlyAgingPriorwithYears";
                        ReportViewer1.ServerReport.ReportServerUrl = MonthlyAgingPriorwithYearsuri;

                        ReportParameters = new ReportParameter[2];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("statusType", statusType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Collections
                    // Collections : Collection Reports

                    case "MonthlyReportCollection":
                        System.Uri monthlySummaryCollectionuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/MonthlyCollectionReports";
                        ReportViewer1.ServerReport.ReportServerUrl = monthlySummaryCollectionuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlySummaryCollectionPerAccount":
                        System.Uri PerAccounturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/MonthlySummaryCollectionPerAccount";
                        ReportViewer1.ServerReport.ReportServerUrl = PerAccounturi;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "FinalValueAddedTaxWithheldByPayor(Form2306)":
                        System.Uri finalVATuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/FinalValueAddedTaxWithheldByPayor(Form2306)";
                        ReportViewer1.ServerReport.ReportServerUrl = finalVATuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "CollectionReportGrossNet":
                        System.Uri grossNeturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CollectionReportGrossNet";
                        ReportViewer1.ServerReport.ReportServerUrl = grossNeturi;

                        ReportParameters = new ReportParameter[4];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("groupedStartDate", groupedStartDate);
                        ReportParameters[2] = new ReportParameter("groupedEndDate", groupedEndDate);
                        ReportParameters[3] = new ReportParameter("groupedARType", groupedARType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "CollectionReportInterest":
                        System.Uri interesturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CollectionReportInterest";
                        ReportViewer1.ServerReport.ReportServerUrl = interesturi;

                        ReportParameters = new ReportParameter[4];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("groupedStartDate", groupedStartDate);
                        ReportParameters[2] = new ReportParameter("groupedEndDate", groupedEndDate);
                        ReportParameters[3] = new ReportParameter("groupedARType", groupedARType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ExpandedWithHoldingTaxWithHeldByPayor(Form2307)":
                        System.Uri EWTuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ExpandedWithHoldingTaxWithHeldByPayor(Form2307)";
                        ReportViewer1.ServerReport.ReportServerUrl = EWTuri;

                        ReportParameters = new ReportParameter[4];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("groupedStartDate", groupedStartDate);
                        ReportParameters[2] = new ReportParameter("groupedEndDate", groupedEndDate);
                        ReportParameters[3] = new ReportParameter("groupedARType", groupedARType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "CollectionReportVAT":
                        System.Uri VATuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CollectionReportVAT";
                        ReportViewer1.ServerReport.ReportServerUrl = VATuri;

                        ReportParameters = new ReportParameter[4];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("groupedStartDate", groupedStartDate);
                        ReportParameters[2] = new ReportParameter("groupedEndDate", groupedEndDate);
                        ReportParameters[3] = new ReportParameter("groupedARType", groupedARType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "CreditableWithHoldingTax":
                        System.Uri CWTuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CreditableWithHoldingTax";
                        ReportViewer1.ServerReport.ReportServerUrl = CWTuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "SummaryCollection(DailySummaryinaMonth)":
                        System.Uri summaryCollectionTuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/SummaryCollection(DailySummaryinaMonth)";
                        ReportViewer1.ServerReport.ReportServerUrl = summaryCollectionTuri;

                        ReportParameters = new ReportParameter[4];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("groupedStartDate", groupedStartDate);
                        ReportParameters[2] = new ReportParameter("groupedEndDate", groupedEndDate);
                        ReportParameters[3] = new ReportParameter("groupedARType", groupedARType);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlySummaryofDeposits":
                        System.Uri summaryDepositsuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/MonthlySummaryofDeposits";
                        ReportViewer1.ServerReport.ReportServerUrl = summaryDepositsuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ComparativeSummaryReportPerRevenueItem":
                        System.Uri perRevenueItemuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ComparativeSummaryReportPerRevenueItem";
                        ReportViewer1.ServerReport.ReportServerUrl = perRevenueItemuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "MonthlySummaryReportCollectionPerRevenueItem":
                        System.Uri collectionPerRevenueItemuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/MonthlySummaryReportCollectionPerRevenueItem";
                        ReportViewer1.ServerReport.ReportServerUrl = collectionPerRevenueItemuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "BillingCollectionReportDaily(USD)":
                        System.Uri dailyUSDuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/BillingCollectionReportDaily(USD)";
                        ReportViewer1.ServerReport.ReportServerUrl = dailyUSDuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "BillingCollectionReportMonthlyDaily(USD)":
                        System.Uri dailyMonthlyUSDuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/BillingCollectionReportMonthlyDaily(USD)";
                        ReportViewer1.ServerReport.ReportServerUrl = dailyMonthlyUSDuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "DollarPaymentsCollection":
                        System.Uri dollarPaymentsuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/DollarPaymentsCollection";
                        ReportViewer1.ServerReport.ReportServerUrl = dollarPaymentsuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ListofCollectionPerUnitResponsibilityCenter":
                        System.Uri responsibilityCenteruri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ListofCollectionPerUnitResponsibilityCenter";
                        ReportViewer1.ServerReport.ReportServerUrl = responsibilityCenteruri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Collections : Official Receipt Reports

                    case "ListofCheckReceived":
                        System.Uri checkReceiveuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ListofCheckReceived";
                        ReportViewer1.ServerReport.ReportServerUrl = checkReceiveuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ListofCancelledOfficialReceipt":
                        System.Uri cancelledORuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ListofCancelledOfficialReceipt";
                        ReportViewer1.ServerReport.ReportServerUrl = cancelledORuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "PaymentDetails":
                        System.Uri paymentDetailuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/PaymentDetails";
                        ReportViewer1.ServerReport.ReportServerUrl = paymentDetailuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "CashReceiptandDepositRecord":
                        System.Uri depositRecorduri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CashReceiptandDepositRecord";
                        ReportViewer1.ServerReport.ReportServerUrl = depositRecorduri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "IssuedOR":
                        System.Uri oruri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/IssuedOR";
                        ReportViewer1.ServerReport.ReportServerUrl = oruri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "OR'swith121":
                        System.Uri ors121uri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/OR'swith121";
                        ReportViewer1.ServerReport.ReportServerUrl = ors121uri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Collections : Other Reports

                    case "AccountableForms":
                        System.Uri afuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/AccountableForms";
                        ReportViewer1.ServerReport.ReportServerUrl = afuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "AccountabilityforAccountableFormsReport":
                        System.Uri aafuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/AccountabilityforAccountableFormsReport";
                        ReportViewer1.ServerReport.ReportServerUrl = aafuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ListofCollectionPerAccountCode":
                        System.Uri perAccountCodeuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ListofCollectionPerAccountCode";
                        ReportViewer1.ServerReport.ReportServerUrl = perAccountCodeuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "SerialNumber":
                        System.Uri serialuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/CollectionReports/SerialNumber";
                        ReportViewer1.ServerReport.ReportServerUrl = serialuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ReportofPayment-EngineeringFees":
                        System.Uri engFeesuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/ReportofPayment-EngineeringFees";
                        ReportViewer1.ServerReport.ReportServerUrl = engFeesuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "VISAApplicationReport":
                        System.Uri VISAuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/VISAApplicationReport";
                        ReportViewer1.ServerReport.ReportServerUrl = VISAuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "StatementOfAccount(Payment)":
                        System.Uri statementofAccounturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/StatementOfAccount(Payment)";
                        ReportViewer1.ServerReport.ReportServerUrl = statementofAccounturi;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "WaterReadingReport":
                        string accountOfficer = ConfigurationManager.AppSettings["accountOfficer"].ToString();
                        string enterpriseRep = ConfigurationManager.AppSettings["enterpriseRep"].ToString();
                        System.Uri WaterReadingReporturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/WaterReadingReport";
                        ReportViewer1.ServerReport.ReportServerUrl = WaterReadingReporturi;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("AccountOfficer", accountOfficer);
                        ReportParameters[2] = new ReportParameter("EnterpriseRep", enterpriseRep);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "WaterReadingPeriodicConsumption":
                        System.Uri periodicConsumptionuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/WaterReadingPeriodicConsumption";
                        ReportViewer1.ServerReport.ReportServerUrl = periodicConsumptionuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "AdvancesAplhaList":
                        System.Uri advanceuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Reports/AdvancesAplhaList";
                        ReportViewer1.ServerReport.ReportServerUrl = advanceuri;

                        ReportParameters = new ReportParameter[3];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);
                        ReportParameters[1] = new ReportParameter("tdStartDate", tdStartDate);
                        ReportParameters[2] = new ReportParameter("tdEndDate", tdEndDate);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Maintenance

                    case "CompanyAlphaList":
                        System.Uri companyuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Maintenance/CompanyAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = companyuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "VATAlphaList":
                        System.Uri vaturi = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Maintenance/VATAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = vaturi;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;
                    
                    case "BillingRatesAlphaList":
                        System.Uri billingRateuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Maintenance/BillingRatesAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = billingRateuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    // Admin Page

                    case "UserAlphaList":
                        System.Uri useruri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Admin/UserAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = useruri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    case "ZoneAlphaList":
                        System.Uri zoneuri = new System.Uri(ReportServerPath);
                        ReportViewer1.ServerReport.ReportPath = "/Admin/ZoneAlphaList";
                        ReportViewer1.ServerReport.ReportServerUrl = zoneuri;

                        ReportParameters = new ReportParameter[1];
                        ReportParameters[0] = new ReportParameter("zoneGroupCode", zoneGroupCode);

                        ReportViewer1.ServerReport.SetParameters(ReportParameters);
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                var aaa = ex.Message;
            }
        }
    }
}
