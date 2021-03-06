﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Configuration;
using Microsoft.SqlServer.Dts.Runtime;

namespace TRWeb
{
    public partial class Report : System.Web.UI.Page
    {
        //KG - These are now gotten from Web.config - private string reportURL = "http://sqlEntDB1/ReportServer";
        //                                            private string reportPath = "/ERP Middleware/ERPReports/ProcessingSummary";
        private string reportURL;
        private string reportPath;
        private string userName = "";
        private string packagePath = "";
        private Auth a = new Auth();
        private BizData biz = new BizData();
        private string errorMessage = "";
//        static string UserGroup = "";

        protected string ReportUrl()
        {
            var reader = new AppSettingsReader();
            reportURL = (reader.GetValue("reportURL", typeof(string))).ToString();
            return reportURL; 
        }
        protected string ReportPath()
        {
            var reader = new AppSettingsReader();
            reportPath = (reader.GetValue("reportPath", typeof(string))).ToString();
            return reportPath;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            biz = new BizData();
            DataTable configDT = new DataTable();
            string sqlStr = "";

            sqlStr = "SELECT Name, Value FROM AppConfig";
            configDT = biz.getData(sqlStr);

            if (configDT.Rows.Count > 0)
            {
                foreach (DataRow row in configDT.Rows)
                {
                    foreach (DataColumn col in configDT.Columns)
                    {
                        if (row[0].ToString().Equals("packagePath"))
                        {
                            packagePath = row[1].ToString();
                        }
                    }
                }
            }
            else
            {
                errorMessage = "The environment variables were not found.";
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ArrayList userInfo = new ArrayList();
            userName = a.formatUserName();
            userInfo = a.getActiveUserByUserID(userName);
            Session["User"] = userInfo;

            ddlReportName.Items.Add("Process Summary");
            ddlReportName.Items.Add("Monthly Report for US Bank");
            ddlReportName.Items.Add("Web Summary by Type, Date");
			
            // KG Added new reporting for IT 1/4/2017
            if (userInfo.Count > 0)
            {
                a = ((Auth)userInfo[0]);
                //UserGroup = a.UserGroup;
                if (a.UserGroup == "IT")
                {
                    ddlReportName.Items.Add("Import Summary");
                    ddlReportName.Items.Add("Export Summary");
                    ddlReportName.Items.Add("Deposit Worksheet");                       //Added by ITJBS, 7/24/2017.
                    ddlReportName.Items.Add("Transactions by Date");                    //Added by ITKG, 01/24/2018
                    ddlReportName.Items.Add("Online Payment Detail");                   //Added by ITALN, 01/08/2019
                    ddlReportName.Items.Add("Missing Online Parking Ticket Numbers");   //Added by ITJBS, 1/9/2019 (SR-19-0004)
                }
                else if (a.UserGroup == "Treasurer")                            //Separated Treasurer from Water, CDA, Police - ITJBS 7/24/2017
                {
                    ddlReportName.Items.Add("Deposit Worksheet");               //Added by ITJBS, 7/24/2017.
                    ddlReportName.Items.Add("Import Summary");                  //Added by ITJBS, 9/25/2017.
                    ddlReportName.Items.Add("Export Summary");
                    ddlReportName.Items.Add("Transactions by Date");            //Added by ITKG, 01/24/2018
                    ddlReportName.Items.Add("Online Payment Detail");           //Added by ITALN, 01/08/2019
                }
                else if (a.UserGroup == "Police")                              //Added by ITJBS 1/9/2019 (SR-19-0004)
                {
                    ddlReportName.Items.Add("Export Summary");                              //Moved from last elseif section to below, ITJBS 1/9/2019.
                    ddlReportName.Items.Add("Missing Online Parking Ticket Numbers");       //Added by ITJBS, 1/9/2019 (SR-19-0004)

                }
                else if ((a.UserGroup == "Water") || (a.UserGroup == "CDA") || (a.UserGroup == "Court"))	//Jane S, 2019-01-24: Added Court for TiPSS project, SR-17-0375.
                {
                    ddlReportName.Items.Add("Export Summary");
                }
            }
            ReportViewer1.ProcessingMode = ProcessingMode.Remote;

            // 4/28/2017 - KG  Added web.config def for these values that were on the .aspx page
            if (!Page.IsPostBack)
            {
                ServerReport serverReport = ReportViewer1.ServerReport;
                serverReport.ReportServerUrl = new Uri(ReportUrl());
                serverReport.ReportPath = ReportPath() + "ProcessingSummary";
            }
        }

         protected void ddlReportName_OnSelectedIndexChange(object sender, EventArgs e)
        {
            // kg added 12/15/2016 - read from Web.config
            string reportName;
            var reader = new AppSettingsReader();
            reportURL = (reader.GetValue("reportURL", typeof(string))).ToString();
            reportPath = (reader.GetValue("reportPath", typeof(string))).ToString();
            DateTime dt = DateTime.Now; 

            if (ddlReportName.SelectedValue == "Import Summary")
            {       // If this is a list of Import Summary Reports
                if (! DateTime.TryParse(tbStartDateImport.Text, out dt))          // If this is not already a date,
                {
                    tbStartDateImport.Text = DateTime.Now.ToShortDateString();    // Default to today's date
                }
                btnRefreshImport_Click(sender, e);    // Load the list of Import Reports
                ReportViewer1.Visible = false;
                pnlExport.Visible = false;
                pnlImport.Visible = true;
            }
            else if (ddlReportName.SelectedValue == "Export Summary")
            {       // If this is a list of Export Summary Reports
                if (! DateTime.TryParse(tbStartDateExport.Text, out dt))        // If this is not already a date,
                {
                    tbStartDateExport.Text = DateTime.Now.ToShortDateString();  // Default to today's date
                }
                btnRefreshExport_Click(sender, e);    // Load the list of Export Reports
                ReportViewer1.Visible = false;
                pnlImport.Visible = false;
                pnlExport.Visible = true;
            }
            else    // If this is an online report
            {
                pnlImport.Visible = false;
                ReportViewer1.Visible = true;
                if (ddlReportName.SelectedValue == "Process Summary")
                {
                    // reportPath = "/ERP Middleware/ERPReports/ProcessingSummary";
                    reportName = reportPath + "ProcessingSummary";
                }
                else if (ddlReportName.SelectedValue == "Web Summary by Type, Date")
                {
                    // reportPath = "/ERP Middleware/ERPReports/TRWebSummary";
                    reportName = reportPath + "TRWebSummary";
                }
                else if (ddlReportName.SelectedValue == "Deposit Worksheet")        //Added by ITJBS, 7/24/2017.
                {
                    // reportPath = "/ERP Middleware/ERPReports/TRWebSummary";
                    reportName = reportPath + "DepositWorksheet";                   //Report previously called OnlineProcessSummary - ITJBS 8/2/2017
                }
                else if (ddlReportName.SelectedValue == "Transactions by Date")        //Added by ITKG, 1/24/2018.
                {
                    // reportPath = "/ERP Middleware/ERPReports/TRWebSummary";
                    reportName = reportPath + "TransactionsByDate";
                }
                else if (ddlReportName.SelectedValue == "Online Payment Detail")    //Added by ITALN, 01/08/2019.
                {
                    // reportPath = "/ERP Middleware/ERPReports/TRWebSummary";
                    reportName = reportPath + "OnlinePaymentDetail";
                }
                else if (ddlReportName.SelectedValue == "Missing Online Parking Ticket Numbers")     //Added by ITJBS, 1/9/2019 (SR-19-0004).
                {
                    reportName = reportPath + "MissingOnlineParkingTicketNumbers";
                }
                else
                {
                    // reportPath = "/ERP Middleware/ERPReports/MonthlyReportforUSBank";
                    reportName = reportPath + "MonthlyReportforUSBank";
                }
                ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                ReportViewer1.Reset();
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(reportURL);
                //ReportViewer1.ServerReport.ReportPath = reportPath;
                ReportViewer1.ServerReport.ReportPath = reportName;
                ReportViewer1.ServerReport.Refresh();
            }
        }

        protected void btnRefreshImport_Click(object sender, EventArgs e)
        {
            // Time to refresh the list of Import/Export rows
            biz = new BizData();
            DataTable reportDT = new DataTable();
            string sDate = tbStartDateImport.Text;
            // Imported Rows
            string sqlStr = "";

            try
            {
            //Jane S, 2017-09-19: Changed MiscHeader line to get specific SourceType instead of just MSCCOM.
			//Jane S, 2019-01-24: Added line for CourtHeader - TiPSS project, SR-17-0375.
                sqlStr = "SELECT * FROM (" +
                                  "SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CDAHeader LEFT OUTER JOIN SourceType ST on CDAHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        //"UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription + '<br>' + Type as SourceTypeDescription FROM MiscHeader LEFT OUTER JOIN SourceType ST on MiscHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        "UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ISNULL((select ST.SourceType from SourceType ST where ST.BillerProductCode = MiscHeader.Type), ST.SourceType) + ' - ' + ST.SourceTypeDescription + '<br>' + Type as SourceTypeDescription FROM MiscHeader LEFT OUTER JOIN SourceType ST on MiscHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        "UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM ParkingHeader LEFT OUTER JOIN SourceType ST on ParkingHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        "UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CourtHeader LEFT OUTER JOIN SourceType ST on CourtHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +						
                        "UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription + '<br>' + Type as SourceTypeDescription FROM PropertyTaxHeader LEFT OUTER JOIN SourceType ST on PropertyTaxHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        "UNION ALL SELECT DISTINCT BatchID, cast(ImportReportSentDate as date) ReportDate, CreatedDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM WaterHeader LEFT OUTER JOIN SourceType ST on WaterHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' " +
                        ") as rs ORDER BY CreatedDate Desc";
                reportDT = biz.getData(sqlStr);
                RptrImport.DataSource = reportDT;
                RptrImport.DataBind();
                btnSendImport.Visible = reportDT.Rows.Count > 0;

                // Now check each row which did not finish printing the report
                foreach (RepeaterItem ri in RptrImport.Items)
                {
                    CheckBox cbSelected = ri.FindControl("cbSelected") as CheckBox;
                    Label lblReportDate = ri.FindControl("lblReportDate") as Label;
                    if (lblReportDate.Text.Length < 1 )
                    {
                        cbSelected.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void btnRefreshExport_Click(object sender, EventArgs e)
        {
            // Time to refresh the list of Import/Export rows
            biz = new BizData();
            DataTable reportDT = new DataTable();
            string sDate = tbStartDateExport.Text;
            // Imported Rows
            string sqlStr = "";
            try
            {
                // Exported Rows
                switch (a.UserGroup)
                {
                    case "IT":
						//Jane S, 2019-01-24: Added line to query for CourtHeader - TiPSS project, SR-17-0375.
                        {
                            sqlStr = "SELECT * FROM (" +
                                               "SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CDAHeader LEFT OUTER JOIN SourceType ST on CDAHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
                                     "UNION ALL SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription + '<br>' + Type as SourceTypeDescription FROM MiscHeader LEFT OUTER JOIN SourceType ST on MiscHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
                                     "UNION ALL SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM ParkingHeader LEFT OUTER JOIN SourceType ST on ParkingHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
									 "UNION ALL SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CourtHeader LEFT OUTER JOIN SourceType ST on CourtHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
                                     "UNION ALL SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM PropertyTaxHeader LEFT OUTER JOIN SourceType ST on PropertyTaxHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
                                     "UNION ALL SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM WaterHeader LEFT OUTER JOIN SourceType ST on WaterHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL " +
                                     ") as rs ";
                            break;
                        }
                    case "Treasurer":
                        {
                            sqlStr = "SELECT * FROM (SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM PropertyTaxHeader LEFT OUTER JOIN SourceType ST on PropertyTaxHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL) as rs";
                            break;
                        }
                    case "Water":
                        {
                            sqlStr = "SELECT * FROM (SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM WaterHeader LEFT OUTER JOIN SourceType ST on WaterHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL) as rs ";
                            break;
                        }
                    case "CDA":
                        {
                            sqlStr = "SELECT * FROM (SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CDAHeader LEFT OUTER JOIN SourceType ST on CDAHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL) as rs ";
                            break;
                        }
                    case "Police":
                        {
                            sqlStr = "SELECT * FROM (SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM ParkingHeader LEFT OUTER JOIN SourceType ST on ParkingHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL) as rs ";
                            break;
                        }
                    //Jane S, 2019-01-24: Added new case for Court - TiPSS project, SR-17-0375.
                    case "Court":
                        {
                            sqlStr = "SELECT * FROM (SELECT DISTINCT ExportedBatchID BatchID, cast(ExportReportSentDate as date) ReportDate, ST.SourceType + ' - ' + ST.SourceTypeDescription as SourceTypeDescription FROM CourtHeader LEFT OUTER JOIN SourceType ST on CourtHeader.SourceType = ST.SourceType WHERE Cast(CreatedDate as date) = '" + sDate + "' AND ExportedBatchID IS NOT NULL) as rs ";
                            break;
                        }						
                }
                reportDT = new DataTable();
                reportDT = biz.getData(sqlStr);
                RptrExport.DataSource = reportDT;
                RptrExport.DataBind();
                btnSendExport.Visible = reportDT.Rows.Count > 0;

                // Now check each row which did not finish printing the report
                foreach (RepeaterItem ri in RptrExport.Items)
                {
                    CheckBox cbSelected = ri.FindControl("cbSelected") as CheckBox;
                    Label lblReportDate = ri.FindControl("lblReportDate") as Label;
                    if (lblReportDate.Text.Length < 1)
                    {
                        cbSelected.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void btnSendImport_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendImport.Enabled = false;
                Hashtable ReportSent = new Hashtable();         // Used to remember which reports were sent by batchid
                //lblImportStatus.Text = "";                
                foreach (RepeaterItem ri in RptrImport.Items)
                {
                    CheckBox cbSelected = ri.FindControl("cbSelected") as CheckBox;
                    Label lblBatchId = ri.FindControl("lblBatchId") as Label;
                    Label lblSourceType = ri.FindControl("lblSourceType") as Label;
                    string sourceType = lblSourceType.Text.Substring(0, 6);
                    if (cbSelected.Checked)
                    {
                        if(! ReportSent.ContainsKey(lblBatchId.Text))       // If we didn't already run this batch id
                        {
                            CallRunReport(lblBatchId.Text, sourceType);     // Run Report
                            ReportSent.Add(lblBatchId.Text, sourceType);    // Add batch to hashtable list
                        }
                    }
                }
                btnRefreshImport_Click(sender, e);    // Refresh the page
                btnSendImport.Enabled = true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        protected void btnSendExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendExport.Enabled = false;
                Hashtable ReportSent = new Hashtable();         // Used to remember which reports were sent by batchid
                //lblExportStatus.Text = "";
                foreach (RepeaterItem ri in RptrExport.Items)
                {
                    CheckBox cbSelected = ri.FindControl("cbSelected") as CheckBox;
                    Label lblBatchId = ri.FindControl("lblBatchId") as Label;
                    Label lblSourceType = ri.FindControl("lblSourceType") as Label;
                    string sourceType = lblSourceType.Text.Substring(0, 6);
                    if (cbSelected.Checked)
                    {
                        if (!ReportSent.ContainsKey(lblBatchId.Text))       // If we didn't already run this batch id
                        {
                            CallRunReport(lblBatchId.Text, sourceType);     // Run Report
                            ReportSent.Add(lblBatchId.Text, sourceType);    // Add batch to hashtable list
                        }
                    }
                }
                btnRefreshExport_Click(sender, e);    // Refresh the page
                btnSendExport.Enabled = true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void CallRunReport(string batchID, string sourceType)
        {
            Application app = new Application();
            Package package = null;
            var reader = new AppSettingsReader();
            //string password = "OFu=m?ujriK[";
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();
            Parameters parms;
            try
            {
                app.PackagePassword = password;
                package = app.LoadPackage(packagePath + "RunReport.dtsx", null);
                parms = package.Parameters;
                parms["ParmBatchID"].Value = batchID;
                parms["SourceType"].Value = sourceType;
                DTSExecResult results = package.Execute();
                //if (results == Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure)   // Package failed
                //{
                //    foreach (Microsoft.SqlServer.Dts.Runtime.DtsError local_DtsError in package.Errors)
                //    {
                //        //retCode = false;        // Error occurred, set returncode to false
                //        lblStatus.Text = "Package Execution results: " + local_DtsError.Description.ToString() + Environment.NewLine;
                //    }
                //}
                //else    // Package ran ok
                //{
                //    lblStatus.Text = "Email(s) sent.";
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
