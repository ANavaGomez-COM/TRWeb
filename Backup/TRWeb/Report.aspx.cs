using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace TRWeb
{
    public partial class Report : System.Web.UI.Page
    {
        private String reportURL = "http://sqlEntDB1/ReportServer";
        private String reportPath = "/ERP Middleware/ERPReports/ProcessingSummary";

        protected void Page_Init(object sender, EventArgs e)
        {
            ddlReportName.Items.Add("Process Summary");
            ddlReportName.Items.Add("Monthly Report for US Bank");

            ReportViewer1.ProcessingMode = ProcessingMode.Remote;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ddlReportName_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddlReportName.SelectedValue == "Process Summary")
            {
                reportPath = "/ERP Middleware/ERPReports/ProcessingSummary";
            }
            else
            {
                reportPath = "/ERP Middleware/ERPReports/MonthlyReportforUSBank";
            }

            ReportViewer1.ProcessingMode = ProcessingMode.Remote;
            ReportViewer1.Reset();
            ReportViewer1.ServerReport.ReportServerUrl = new Uri(reportURL);
            ReportViewer1.ServerReport.ReportPath = reportPath;
            ReportViewer1.ServerReport.Refresh();
        }
    }
}
