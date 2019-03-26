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
        private string errorMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) //check if the webpage is loaded for the first time.
            {
                ViewState["PreviousPage"] = Request.UrlReferrer; //Saves the Previous page url in ViewState
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            biz = new BizData();
            string sqlStr = "exec GetUserGroupList";
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
            lblUserEmailError.Text = "";

            lblUserIdError.Text = "";
            lblUserEmailError.Text = "";

            if (string.IsNullOrWhiteSpace(txtUserEmail.Text))
            {
                lblUserEmailError.Text = "You must enter a valid notification email.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                //lblUserIdError.Text = "You must enter a 5 letter User ID.";  //User IDs can now be 4-8 characters.
                lblUserIdError.Text = "You must enter a valid User ID.";
                valid = false;
            }

            if (!IsValidEmail(txtUserEmail.Text))
            {
                lblUserEmailError.Text = "Please enter a valid email address.";
                valid = false;
            }

            //Added by Jane S., 5/8/2017.
            if (!IsUserIDValidLength(txtUserId.Text))
            {
                lblUserIdError.Text = "The User ID you entered is too long or too short; please enter a User ID from 4 to 8 characters.";
                valid = false;
            }

            /* No longer using IsFiveLettersLong due to User ID length being allowed to be longer or shorter. - Jane S. 5/8/2017
                    if (!IsFiveLettersLong(txtUserId.Text))
                    {
                        lblUserIdError.Text = "Please enter a 5 letter User ID.";
                        valid = false;
                    }
            */

            if (valid)
            {
                a.insertUserInformation(txtUserId.Text, txtFirstName.Text, txtLastName.Text, txtUserEmail.Text
                    , cmbUserGroup.SelectedItem.Text, chkActiveUser.Checked);
                //Response.Redirect("~/Administration.aspx");
                Response.Redirect(ViewState["PreviousPage"].ToString());    // Returns to calling page
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewState["PreviousPage"].ToString());    // Returns to calling page
        }

        //Added by Jane S., 5/8/2017.
        public static bool IsUserIDValidLength(string str)
        {
            /*** A User ID could be 4 - 6 characters for sure, so allowing 4-8 just in case. - Jane S. 5/8/2017 ***/
            Regex rx = new Regex(@"^[a-zA-Z0-9]{4,8}$");
            return rx.IsMatch(str);
        }

        /* A User ID could be other than five characters, so created new function IsUserIDValidLength. - Jane S. 5/8/2017
            public static bool IsFiveLettersLong(string str)
            {
                Regex rx = new Regex(@"^[a-zA-Z]{5}$");
                return rx.IsMatch(str);
            }
        */

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
}
