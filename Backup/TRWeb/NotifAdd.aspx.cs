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
        private string errorMessage = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

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
            notif = new Notification();
            Boolean valid = true;
            lblNotifEmailError.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "You must enter a valid notification email.";
                valid = false;
            }

            if (!IsValidEmail(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "Please enter a valid email address.";
                valid = false;
            }

            if (valid)
            {
                notif.insertNotificationInformation(cmbSourceList.SelectedItem.ToString().Substring(0, 6), txtNotifEmail.Text, chkReceiveOnline.Checked
                    , chkReceiveRemittance.Checked, chkActiveNotif.Checked);
                Response.Redirect("~/Administration.aspx");
            }
        }   

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
}
