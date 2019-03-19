using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections;
using System.IO;
using System.Data;

namespace TRWeb
{
    public partial class AdminNotify : System.Web.UI.Page
    {
        private string searchSelectedValue = "";

        private BizData biz = new BizData();
        private string packagePath = "";
        private string errorMessage = "";
        static string gridType = "";

        private GridViewRow grdViewRow;
        private Notification notif;
        private Source s;
        private Auth a;

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
            resetPage();
            biz = new BizData();
            string sqlStr = "exec GetUserGroupList";
            DataTable uGrpList = new DataTable();
            uGrpList = biz.getData(sqlStr);
        }

        protected void cmbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);          
        }

        protected void btnAddSource_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SourceAdd.aspx");
        }

        protected void btnListSources_Click(object sender, EventArgs e)
        {
            s = new Source();
            gridType = "Source Type";
            grdResult.DataSource = s.getSourceInformation("All", "");
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }
        
        protected void btnAddEmailNotif_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NotifAdd.aspx");
        }

        protected void btnUpdateEmailNotif_Click(object sender, EventArgs e)
        {
            notif = new Notification();
            ArrayList notifInfo = new ArrayList();
            string Source = txtSource.Text;

            string notifEmail = txtNotifEmail.Text;
            notifInfo = notif.getNotificationInformationByEmail(notifEmail, Source);
            notif = (Notification)notifInfo[0];

            notif.updateNotificationInformation(notif.EmailID, notifEmail
                , chkReceiveOnline.Checked, chkReceiveRemittance.Checked, chkActiveNotif.Checked);
            ListNotifications();
        }

        protected void ListNotifications()
        {
            notif = new Notification();
            gridType = "Notification Email Address";
            grdResult.DataSource = notif.getNotificationInformationAll();
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AuthAdd.aspx");
        }

        protected void btnListUsers_Click(object sender, EventArgs e)
        {
            a = new Auth();
            gridType = "User Email Address";
            grdResult.DataSource = a.getUserAll();
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            processRequest(gridType, true);            
        }

        protected void grdResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdResult.PageIndex = e.NewPageIndex;
            grdResult.SelectedIndex = -1;   // Unselect row
            resetPage();
        }

        protected void grdResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dataTable = grdResult.DataSource as DataTable;

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

                grdResult.DataSource = dataView;
                grdResult.DataBind();
            }
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string newSortDirection = string.Empty;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = "ASC";
                    break;

                case SortDirection.Descending:
                    newSortDirection = "DESC";
                    break;
            }

            return newSortDirection;
        }

        protected void processRequest(string callType, Boolean grdCall)
        {
            string EmailID = "";
            string Email = "";
            CheckBox OnlineEmail ;
            CheckBox RemittanceEmail ;
            CheckBox Active ;
            string SourceType = "";
            ArrayList notifInfo = new ArrayList();

            if (grdCall)
            {
                grdViewRow = grdResult.SelectedRow;
                EmailID = grdViewRow.Cells[1].Text.Trim();
                SourceType = grdViewRow.Cells[6].Text.Trim();
                Email = grdViewRow.Cells[2].Text.Trim();
                RemittanceEmail = (CheckBox)grdViewRow.Cells[4].Controls[0];
                OnlineEmail = (CheckBox)grdViewRow.Cells[3].Controls[0];
                Active = (CheckBox)grdViewRow.Cells[5].Controls[0];

                txtSource.Text = SourceType;
                txtNotifEmail.Text = Email;
                chkReceiveRemittance.Checked = RemittanceEmail.Checked;
                chkReceiveOnline.Checked = OnlineEmail.Checked;                
                chkActiveNotif.Checked = Active.Checked;

                if ((SourceType == "TRSDP5") || (SourceType == "TRSONL"))
                {
                    chkReceiveRemittance.Visible = true;
                    lblReceiveRemittance.Visible = true;
                }
                else
                {
                    chkReceiveRemittance.Visible = false;
                    lblReceiveRemittance.Visible = false;
                }
                btnUpdateEmailNotif.Enabled = true;
            }
        }

        public void resetPage()
        {
            txtNotifEmail.Text = "";
            chkReceiveRemittance.Checked = false;
            chkReceiveOnline.Checked = false;
            chkActiveNotif.Checked = false;
            ListNotifications();
            btnUpdateEmailNotif.Enabled = false;
        }

        public Boolean confirmBox(string Message) 
        {
            Boolean resp = false;
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "MessageBox",
                "<script type='text/javascript'>confirm_proceed('" + Message + "');</script>");
            resp = chkHidden.Checked;
            return resp;
        }
    }
}
