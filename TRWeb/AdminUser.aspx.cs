using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Dts.Runtime;
//using Microsoft.SqlServer;
using System.Collections;
using System.IO;
using System.Data;

namespace TRWeb
{
    public partial class AdminUser : System.Web.UI.Page
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
            cmbUserGroup.DataSource = uGrpList;
            cmbUserGroup.DataValueField = "GroupName";
            cmbUserGroup.DataTextField = "GroupName";
            cmbUserGroup.DataBind();
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

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AuthAdd.aspx");
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            a = new Auth();
            int spaceLoc = txtUserName.Text.IndexOf(" ");
            string userEmail = txtUserEmail.Text;
            string firstName = txtUserName.Text.Substring(0, spaceLoc);
            string lastName = txtUserName.Text.Substring(spaceLoc + 1);
            a.updateUserInformation(userEmail, firstName, lastName, cmbUserGroup.SelectedItem.ToString(), chkActiveUser.Checked, chkAdministrator.Checked);
            ListUsers();
        }

        protected void ListUsers()
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
            //grdResult.DataBind();
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
            string newSortDirection = "";

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
            int cmbUserGroupInt;

            if (callType.Equals("User Email Address"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    string userID = "";                     // User Id Field
                    userID = grdViewRow.Cells[1].Text.Trim();
                    txtUserID.Text = userID;
                    string userFirstName = "";              // User First Name Field
                    userFirstName = grdViewRow.Cells[2].Text.Trim();
                    string userLastName = "";               // User Last Name Field
                    userLastName = grdViewRow.Cells[3].Text.Trim();
                    txtUserName.Text = (userFirstName + " " + userLastName).TrimStart();
                    string userEmail = "";    // User Email Field
                    userEmail = grdViewRow.Cells[4].Text.Trim();
                    txtUserEmail.Text = userEmail;
                    string userGroup = "";    // User Group Field
                    userGroup = grdViewRow.Cells[5].Text.Trim();
                    cmbUserGroupInt = ConvertUserGroupValueToInt(userGroup);
                    if (cmbUserGroupInt > -1)   // If we got a valid return value
                    {
                        cmbUserGroup.SelectedIndex = cmbUserGroupInt;
                    }                    
                    string groupDescription = "";     // Group Description Field
                    groupDescription = grdViewRow.Cells[6].Text.Trim();
                    CheckBox chkActive;             // Active Field
                    chkActive = (CheckBox)grdViewRow.Cells[7].Controls[0];
                    chkActiveUser.Checked = chkActive.Checked;
                    CheckBox chkAdmin;     // Administration Field
                    chkAdmin = (CheckBox)grdViewRow.Cells[8].Controls[0];
                    chkAdministrator.Checked = chkAdmin.Checked;
                    btnUpdateUser.Enabled = true;
                }
            }            
        }

        private int ConvertUserGroupValueToInt(string userGroupVal) // Converts the Group name to the selected index
        {
            int ConvertUserGroupValueToInt = -1;
            int iLoop = 0;
            int iMax = cmbUserGroup.Items.Count;

            for (iLoop = 0; iLoop < iMax; iLoop++) 
            {
                if (cmbUserGroup.Items[iLoop].Value == userGroupVal)
                {
                    ConvertUserGroupValueToInt = iLoop;
                    break; 
                }
            } 
            return ConvertUserGroupValueToInt;
        }

        public void resetPage()
        {
            txtUserID.Text = "";
            txtUserName.Text = "";
            txtUserEmail.Text = "";
            chkActiveUser.Checked = false;
            chkAdministrator.Checked = false;
            ListUsers();
            btnUpdateUser.Enabled = false;
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
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
