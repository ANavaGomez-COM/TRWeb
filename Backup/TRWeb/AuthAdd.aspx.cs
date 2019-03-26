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
    public partial class AuthAdd : System.Web.UI.Page
    {
        private BizData biz;
        private Auth a;
        private string errorMessage = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            biz = new BizData();
            String sqlStr = "exec GetUserGroupList";
            DataTable uGrpList = new DataTable();
            uGrpList = biz.getData(sqlStr);
            cmbUserGroup.DataSource = uGrpList;
            cmbUserGroup.DataValueField = "GroupName";
            cmbUserGroup.DataTextField = "GroupName";
            cmbUserGroup.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            a = new Auth();
            Boolean valid = true;
            lblUserEmailError.Text = String.Empty;

            lblUserIdError.Text = String.Empty;
            lblUserEmailError.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(txtUserEmail.Text))
            {
                lblUserEmailError.Text = "You must enter a valid notification email.";
                valid = false;
            }

            if (String.IsNullOrWhiteSpace(txtUserId.Text))
            {
                lblUserIdError.Text = "You must enter a 5 letter User ID.";
                valid = false;
            }

            if (!IsValidEmail(txtUserEmail.Text))
            {
                lblUserEmailError.Text = "Please enter a valid email address.";
                valid = false;
            }

            if (!IsFiveLettersLong(txtUserId.Text))
            {
                lblUserIdError.Text = "Please enter a 5 letter User ID.";
                valid = false;
            }

            if (valid)
            {
                a.insertUserInformation(txtUserId.Text, txtFirstName.Text, txtLastName.Text, txtUserEmail.Text
                    , cmbUserGroup.SelectedItem.Text, chkActiveUser.Checked);
                Response.Redirect("~/Administration.aspx");
            }
        }

        public static bool IsFiveLettersLong(string str)
        {
            Regex rx = new Regex(@"^[a-zA-Z]{5}$");
            return rx.IsMatch(str);
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
}
