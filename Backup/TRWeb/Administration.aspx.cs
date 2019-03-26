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
    public partial class Administration : System.Web.UI.Page
    {
        private string searchSelectedValue = String.Empty;

        private BizData biz = new BizData();
        private String packagePath = String.Empty;
        private string errorMessage = String.Empty;
        static String gridType = String.Empty;

        private GridViewRow grdViewRow;
        private Notification notif;
        private Source s;
        private Auth a;

        protected void Page_Load(object sender, EventArgs e)
        {
            biz = new BizData();
            DataTable configDT = new DataTable();
            String sqlStr = String.Empty;

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

            cmbSearchType.Items.Clear();
            cmbSearchType.Items.Add("Source Type");
            cmbSearchType.Items.Add("Notification Email Address");
            cmbSearchType.Items.Add("Source Description");
            cmbSearchType.Items.Add("User Name");
            cmbSearchType.Items.Add("User Email Address");
            cmbSearchType.Items.Add("User Group");

            biz = new BizData();
            String sqlStr = "exec GetUserGroupList";
            DataTable uGrpList = new DataTable();
            uGrpList = biz.getData(sqlStr);
            cmbUserGroup.DataSource = uGrpList;
            cmbUserGroup.DataValueField = "GroupName";
            cmbUserGroup.DataTextField = "GroupName";
            cmbUserGroup.DataBind();

            if (IsPostBack == false)
            {
                this.cmbSearchType_SelectedIndexChanged(this.cmbSearchType, new EventArgs());
            }
            else
            {
                searchSelectedValue = Session["sessSearchSelVal"].ToString();
            }
        }

        protected void cmbSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchSelectedValue = cmbSearchType.SelectedValue;
            Session["sessSearchSelVal"] = searchSelectedValue;

            resetPage();
            setAdministrationScreen();

            if (searchSelectedValue.Equals("Source Type"))
            {
                this.cmbSearchList_SelectedIndexChanged(this.cmbSearchList, new EventArgs());
            }
        }

        protected void cmbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);          
        }

        protected void lstSourceList_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            String search = String.Empty;

            ArrayList sourceInfo = new ArrayList();
            ArrayList notifInfo = new ArrayList();
            ArrayList authInfo = new ArrayList();

            lstNotifEmailList.Items.Clear();
            lstUserEmailList.Items.Clear();

            s = new Source();
            search = lstSourceList.SelectedItem.Text;
            sourceInfo = s.getSourceInformation("Source Type", search);

            s = (Source)sourceInfo[0];

            txtSourceDescription.Text = s.Description;
            txtBillerProductCode.Text = s.BillerCode;
            txtSourceAttachment.Text = s.AttachmentName;

            notif = new Notification();
            String type = lstSourceList.SelectedItem.Text;
            notifInfo = notif.getNotificationEmailBySourceType(type);

            if (notifInfo.Count > 0)
            {
                lstNotifEmailList.DataSource = notifInfo;
                lstNotifEmailList.DataValueField = "Email";
                lstNotifEmailList.DataTextField = "Email";
                lstNotifEmailList.DataBind();

                notif = (Notification)notifInfo[0];

                chkActiveNotif.Checked = notif.Active;
                chkReceiveOnline.Checked = notif.OnlineEmail;
                chkReceiveRemittance.Checked = notif.RemittanceEmail;

                foreach (Notification n in notifInfo)
                {
                    a = new Auth();
                    authInfo = new ArrayList();

                    authInfo = a.getUserByEmail(n.Email);

                    if (authInfo.Count > 0)
                    {
                        a = (Auth)authInfo[0];
                        lstUserEmailList.Items.Add(a.UserEmail);
                    }
                }

                if (lstUserEmailList.Items.Count > 0)
                {
                    a = new Auth();
                    authInfo = new ArrayList();
                    String email = lstUserEmailList.Items[0].Text;
                    authInfo = a.getUserByEmail(email);
                    a = (Auth)authInfo[0];
                    txtUserID.Text = a.UserID;
                    txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                    cmbUserGroup.SelectedValue = a.UserGroup;
                    chkActiveUser.Checked = a.Active;
                    chkAdministrator.Checked = a.Administrator;
                }
            }
        }

        protected void lstNotifEmailList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList notifInfo = new ArrayList();

            notif = new Notification();
            String type = lstSourceList.SelectedItem.Text;
            String notifEmail = lstNotifEmailList.SelectedItem.Text;
            notifInfo = notif.getNotificationInformationByEmail(notifEmail, type);

            if (notifInfo.Count > 0)
            {
                notif = (Notification)notifInfo[0];

                chkActiveNotif.Checked = notif.Active;
                chkReceiveOnline.Checked = notif.OnlineEmail;
                chkReceiveRemittance.Checked = notif.RemittanceEmail;
            }
        }

        protected void lstUserEmailList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            a = new Auth();
            ArrayList authInfo = new ArrayList();

            String uEmail = lstUserEmailList.SelectedItem.Text;

            authInfo = a.getUserByEmail(uEmail);

            if (authInfo.Count > 0)
            {
                a = new Auth();
                a = (Auth)authInfo[0];
                txtUserID.Text = a.UserID;
                txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                cmbUserGroup.SelectedValue = a.UserGroup;
                chkActiveUser.Checked = a.Active;
                chkAdministrator.Checked = a.Administrator;
            }
        }

        protected void btnAddSource_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SourceAdd.aspx");
        }

        protected void btnUpdateSource_Click(object sender, EventArgs e)
        {
            s = new Source();
            s.updateSourceType(lstSourceList.SelectedItem.Text, txtSourceDescription.Text,
                txtSourceAttachment.Text, txtBillerProductCode.Text);
        }

        protected void btnDeleteSource_Click(object sender, EventArgs e)
        {
            if (chkHidden.Checked)
            {
                s = new Source();
                s.deleteSourceType(lstSourceList.SelectedItem.Text);
                chkHidden.Checked = false;
                resetPage();
                setAdministrationScreen();

                if (searchSelectedValue.Equals("Source Type"))
                {
                    this.cmbSearchList_SelectedIndexChanged(this.cmbSearchList, new EventArgs());
                }
            }
            else
            {
                confirmBox("Warning: deleting a source type will also delete all associated notifications " +
                    "and orphan all existing transactions associated with it.  If you wish to continue click OK then " +
                    "the Delete Source button.");
            }
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
            String type = lstSourceList.SelectedItem.Text;
            String notifEmail = lstNotifEmailList.SelectedItem.Text;

            notifInfo = notif.getNotificationInformationByEmail(notifEmail, type);
            notif = (Notification)notifInfo[0];

            notif.updateNotificationInformation(notif.EmailID, lstNotifEmailList.SelectedItem.Text
                , chkReceiveOnline.Checked, chkReceiveRemittance.Checked, chkActiveNotif.Checked);
        }

        protected void btnListNotifications_Click(object sender, EventArgs e)
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

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            a = new Auth();
            int spaceLoc = txtUserName.Text.IndexOf(" ");
            String email = lstUserEmailList.SelectedValue;
            String firstName = txtUserName.Text.Substring(0, spaceLoc);
            String lastName = txtUserName.Text.Substring(spaceLoc + 1);
            a.updateUserInformation(email, firstName, lastName, cmbUserGroup.SelectedItem.ToString(), chkActiveUser.Checked, chkAdministrator.Checked);
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
            grdResult.DataBind();
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
            string newSortDirection = String.Empty;

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

        protected void processRequest(String callType, Boolean grdCall)
        {
            String search = String.Empty;
            
            ArrayList sourceInfo = new ArrayList();
            ArrayList notifInfo = new ArrayList();
            ArrayList authInfo = new ArrayList();

            lstSourceList.Items.Clear();
            lstNotifEmailList.Items.Clear();
            lstUserEmailList.Items.Clear();

            s = new Source();
            if (callType.Equals("Source Type"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[1].Text.Trim();
                }
                else
                {
                    search = cmbSearchList.SelectedItem.ToString().Substring(0, 6);
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("Source Description"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("Notification Email Address"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[2].Text.Trim();
                }
                else
                {
                    search = txtSearchSingle.Text;
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Name"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Email Address"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[4].Text.Trim();
                }
                else
                {
                    search = txtSearchSingle.Text;
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Group"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }

            if (sourceInfo.Count > 0)
            {
                lstSourceList.DataSource = sourceInfo;
                lstSourceList.DataValueField = "SourceType";
                lstSourceList.DataTextField = "SourceType";
                lstSourceList.DataBind();

                s = (Source)sourceInfo[0];

                txtSourceDescription.Text = s.Description;
                txtBillerProductCode.Text = s.BillerCode;
                txtSourceAttachment.Text = s.AttachmentName;
            }

            chkReceiveRemittance.Visible = false;
            lblReceiveRemittance.Visible = false;

            if (lstSourceList.Items.Count > 0)
            {
                notif = new Notification();
                String type = lstSourceList.Items[0].Text;
                notifInfo = notif.getNotificationEmailBySourceType(type);

                if ((type == "TRSDP5") || (type == "TRSONL"))
                {
                    chkReceiveRemittance.Visible = true;
                    lblReceiveRemittance.Visible = true;
                }


                if (notifInfo.Count > 0)
                {
                    lstNotifEmailList.DataSource = notifInfo;
                    lstNotifEmailList.DataValueField = "Email";
                    lstNotifEmailList.DataTextField = "Email";
                    lstNotifEmailList.DataBind();

                    notif = (Notification)notifInfo[0];

                    chkActiveNotif.Checked = notif.Active;
                    chkReceiveOnline.Checked = notif.OnlineEmail;
                    chkReceiveRemittance.Checked = notif.RemittanceEmail;

                    foreach (Notification n in notifInfo)
                    {
                        a = new Auth();
                        authInfo = new ArrayList();

                        authInfo = a.getUserByEmail(n.Email);

                        if (authInfo.Count > 0)
                        {
                            a = (Auth)authInfo[0];
                            lstUserEmailList.Items.Add(a.UserEmail);
                        }
                    }

                    if (lstUserEmailList.Items.Count > 0)
                    {
                        a = new Auth();
                        authInfo = new ArrayList();
                        String email = lstUserEmailList.Items[0].Text;
                        authInfo = a.getUserByEmail(email);
                        a = (Auth)authInfo[0];
                        txtUserID.Text = a.UserID;
                        txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                        cmbUserGroup.SelectedValue = a.UserGroup;
                        chkActiveUser.Checked = a.Active;
                        chkAdministrator.Checked = a.Administrator;
                    }
                }
            }
            if (lstSourceList.Items.Count > 0)
            {
                lstSourceList.SelectedIndex = 0;
            }
            if (lstNotifEmailList.Items.Count > 0)
            {
                lstNotifEmailList.SelectedIndex = 0;
            }
            if (lstUserEmailList.Items.Count > 0)
            {
                lstUserEmailList.SelectedIndex = 0;
            }
        }

        public void setAdministrationScreen()
        {
            if (searchSelectedValue.Equals("Source Type"))
            {
                s = new Source();

                lblSearch.Visible = true;
                cmbSearchList.Visible = true;

                cmbSearchList.DataSource = s.getSourceList();
                cmbSearchList.DataValueField = "SourceType";
                cmbSearchList.DataTextField = "Description";
                cmbSearchList.DataBind();
            }
            else if (searchSelectedValue.Equals("Source Description"))
            {
                lblSearch.Text = "Source Description";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("Notification Email Address"))
            {
                lblSearch.Text = "Email Address";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Name"))
            {
                lblSearch.Text = "User Name";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Email Address"))
            {
                lblSearch.Text = "Email Address";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Group"))
            {
                lblSearch.Text = "User Group";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
        }

        public void resetPage()
        {
            lblSearch.Visible = false;
            cmbSearchList.Visible = false;
            txtSearchSingle.Visible = false;
            btnSearch.Visible = false;

            lstNotifEmailList.Items.Clear();
            lstSourceList.Items.Clear();
            lstUserEmailList.Items.Clear();

            txtBillerProductCode.Text = String.Empty;
            txtSearchSingle.Text = String.Empty;
            txtSourceAttachment.Text = String.Empty;
            txtSourceDescription.Text = String.Empty;
            txtUserID.Text = String.Empty;
            txtUserName.Text = String.Empty;

            chkActiveNotif.Checked = false;
            chkActiveUser.Checked = false;
            chkAdministrator.Checked = false;
            chkReceiveOnline.Checked = false;
            chkReceiveRemittance.Checked = false;
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        public Boolean confirmBox(String Message) 
        {
            Boolean resp = false;
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "MessageBox",
                "<script type='text/javascript'>confirm_proceed('" + Message + "');</script>");
            resp = chkHidden.Checked;
            return resp;
        }

    }
}
