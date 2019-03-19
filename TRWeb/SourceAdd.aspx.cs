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
    public partial class SourceAdd : System.Web.UI.Page
    {
        private Source s;
        private string errorMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            s = new Source();
            Boolean valid = true;
            lblSourceError.Text = "";
            lblNotifEmailError.Text = "";

            if (string.IsNullOrWhiteSpace(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "You must enter a valid notification email.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtSource.Text))
            {
                lblSourceError.Text = "You must enter a 6 letter source.";
                valid = false;
            }

            if (!IsValidEmail(txtNotifEmail.Text))
            {
                lblNotifEmailError.Text = "Please enter a valid email address.";
                valid = false;
            }

            if (!IsSixLettersLong(txtSource.Text))
            {
                lblSourceError.Text = "Please enter a 6 letter source.";
                valid = false;
            }

            if (valid)
            {
                s.insertSourceInformation(txtSource.Text, txtSourceDescription.Text, txtSourceAttachment.Text, txtBillerProductCode.Text
                    , txtNotifEmail.Text, chkReceiveOnline.Checked, chkReceiveRemittance.Checked, chkActiveNotif.Checked);
                Response.Redirect("~/Administration.aspx");
            }
        }   
        
        public static bool IsSixLettersLong(string str)
        {
            Regex rx = new Regex(@"^[a-zA-Z]{6}$");
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
