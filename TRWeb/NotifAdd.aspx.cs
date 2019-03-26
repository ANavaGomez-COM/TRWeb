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
    public partial class NotifAdd : System.Web.UI.Page
    {
        private Source s;
        private Notification notif;
        private string errorMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) //check if the webpage is loaded for the first time.
            {
                ViewState["PreviousPage"] = Request.UrlReferrer; //Saves the Previous page url in ViewState
                Auth a = new Auth();
                lstNotifEmailList.DataSource = a.getUserAll();
                lstNotifEmailList.DataBind();
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            s = new Source();
            cmbSourceList.DataSource = s.getSourceList();
            cmbSourceList.DataValueField = "SourceType";
            cmbSourceList.DataTextField = "Description";
            cmbSourceList.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //itjas, 9/13/2018: old code superseded by code to update emails using free-text (SR-18-0000250)
            //notif = new Notification();
            //Boolean valid = true;
            //lblNotifEmailError.Text = "";

            //if (lstNotifEmailList.SelectedIndex < 0)
            //{
            //    lblNotifEmailError.Text = "You must select a valid notification email.";
            //    valid = false;
            //}

            //if (valid)
            //{
            //    notif.insertNotificationInformation(cmbSourceList.SelectedItem.ToString().Substring(0, 6), lstNotifEmailList.SelectedValue, chkReceiveOnline.Checked
            //        , chkReceiveRemittance.Checked, chkActiveNotif.Checked);
            //    Response.Redirect(ViewState["PreviousPage"].ToString());    // Returns to calling page
            //}
            if (string.IsNullOrWhiteSpace(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "You must enter a valid notification email.";
            }
            else if (!IsValidEmail(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "Please enter a valid email address.";
            }
            else
            {
                notif = new Notification();

                //save return value--itjas, 8/28/2018
                string result = notif.insertNotificationInformation(cmbSourceList.SelectedItem.ToString().Substring(0, 6), txtNotifEmail.Text, chkReceiveOnline.Checked
                    , chkReceiveRemittance.Checked, chkActiveNotif.Checked);

                //check return value and display appropriate message--itjas, 8/28/2018
                if (result.Equals("Update Successful"))
                {
                    lblNotifEmailError.Text = "";
                    Response.Redirect(ViewState["PreviousPage"].ToString());    // Returns to calling page
                }
                else if (result.Contains("duplicate key"))
                {
                    lblNotifEmailError.Text = "Duplicate exists--unable to add " + txtNotifEmail.Text;
                }
                else
                {
                    lblNotifEmailError.Text = "Unable to add " + txtNotifEmail.Text;
                }
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewState["PreviousPage"].ToString());    // Returns to calling page
        }

        //added to implement free-text email values--itjas, 8/23/2018
        protected void lstNotifEmailList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            txtNotifEmail.Text = lstNotifEmailList.SelectedItem.Text;
            lblNotifEmailError.Text = "";
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
}
