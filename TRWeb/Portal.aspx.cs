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
    public partial class Portal : System.Web.UI.Page
    {
        private string groupSelectedValue = "";
        private string functionSelectedValue = "";
        private GridViewRow grdViewRow;
        private WSPortal port;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            resetPage();

            cmbGroup.Items.Clear();
            cmbGroup.Items.Add("Administration");
            cmbGroup.Enabled = false;
            
            if (IsPostBack == false)
            {
                this.cmbGroup_SelectedIndexChanged(this.cmbGroup, new EventArgs());
            }
            else
            {
                groupSelectedValue = Session["sessGrpSelVal"].ToString();
                functionSelectedValue = Session["sessFuncSelVal"].ToString();
            }
        }

        protected void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFunction.Items.Clear();
            groupSelectedValue = cmbGroup.SelectedValue;
            Session["sessGrpSelVal"] = groupSelectedValue;

            if (groupSelectedValue.Equals("Administration"))
            {
                cmbFunction.Items.Add("Get Portal Information");
                cmbFunction.Items.Add("Add New Portal");
            }
            this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            functionSelectedValue = cmbFunction.SelectedValue;
            Session["sessFuncSelVal"] = functionSelectedValue;

            if (functionSelectedValue.Equals("Get Portal Information"))
            {
                resetPage();
                setAdministrationScreen();
            }
            else if (functionSelectedValue.Equals("Add New Portal"))
            {
                resetPage();
                setAdministrationScreen();
            }
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            WSPortal admin = new WSPortal();
            string str = "";
            lstAvailableMethods.Items.Clear();
            lstMethods.Items.Clear();
            grdViewRow = grdResult.SelectedRow;

            if (functionSelectedValue.Equals("Get Portal Information"))
            {
                lblOwnerName.Visible = true;
                lblOwnerEmail.Visible = true;
                lblPortalID.Visible = true;
                lblActive.Visible = true;
                txtOwnerName.Visible = true;
                txtOwnerEmail.Visible = true;
                txtPortalID.Visible = true;
                txtActive.Visible = true;
               
                tblMethods.Visible = true;
                lblAvailableMethods.Visible = true;
                lblMethods.Visible = true;
                lstAvailableMethods.Visible = true;
                lstMethods.Visible = true;

                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnUpdate.Visible = true;
                btnNewKey.Visible = true;

                str = str + "Selected Record:\r\n\r\n";
                str = str + "Owner Name: " + grdViewRow.Cells[1].Text.Trim() + "\r\n";
                str = str + "Owner Email: " + grdViewRow.Cells[2].Text.Trim() + "\r\n";
                str = str + "Portal ID: " + grdViewRow.Cells[3].Text.Trim() + "\r\n";
                str = str + "Active: " + grdViewRow.Cells[4].Text.Trim() + "\r\n\r\n";
                str = str + "Make Adjustments on the left the click update to update portal information." + Environment.NewLine +
                    "If you wish to change a portal key please use the Request New Key function.";
                txtOwnerName.Text = grdViewRow.Cells[1].Text.Trim();
                txtOwnerEmail.Text = grdViewRow.Cells[2].Text.Trim();
                txtPortalID.Text = grdViewRow.Cells[3].Text.Trim();
                txtActive.Text = grdViewRow.Cells[4].Text.Trim();

                lstAvailableMethods.DataSource = admin.getAvailablePortalMethods(Convert.ToInt32(txtPortalID.Text));
                lstAvailableMethods.DataBind();
                lstMethods.DataSource = admin.getPortalMethods(Convert.ToInt32(txtPortalID.Text));
                lstMethods.DataBind();

                txtResult.Text = str;
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {

            List<Object> removeList = new List<Object>();

            foreach (ListItem listItem in lstMethods.Items)
            {
                if (listItem.Selected)
                {
                    lstAvailableMethods.Items.Add(listItem.Text);
                    removeList.Add(listItem);
                }
            }
            foreach (ListItem removeItem in removeList)
            {
                lstMethods.Items.Remove(removeItem);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            List<Object> removeList = new List<Object>();

            foreach (ListItem listItem in lstAvailableMethods.Items)
            {
                if (listItem.Selected)
                {
                    lstMethods.Items.Add(listItem.Text);
                    removeList.Add(listItem);
                }
            }
            foreach (ListItem removeItem in removeList)
            {
                lstAvailableMethods.Items.Remove(removeItem);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";

            if (groupSelectedValue.Equals("Administration"))
            {
                processPortalRequest();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (functionSelectedValue.Equals("Get Portal Information"))
            {
                port = new WSPortal();
                List<String> methods = new List<String>();
                foreach (ListItem item in lstMethods.Items)
                {
                    methods.Add(item.ToString());
                }
                string[] passedMethods = methods.ToArray();
                txtResult.Text = port.updatePortal(txtOwnerName.Text, txtOwnerEmail.Text,
                    Convert.ToBoolean(txtActive.Text), Convert.ToInt32(txtPortalID.Text), passedMethods);
            }
        }

        protected void btnNewKey_Click(object sender, EventArgs e)
        {
            port = new WSPortal();

            txtResult.Text = port.createNewKey(Convert.ToInt32(txtPortalID.Text));
        }

        protected void btnViewMethods_Click(object sender, EventArgs e)
        {
            port = new WSPortal();

            lstAvailableMethods.Visible = true;
            lblAvailableMethods.Visible = true;

            lstAvailableMethods.DataSource = port.getAvailablePortalMethods(Convert.ToInt32(txtPortalID.Text));
            lstAvailableMethods.DataBind();
        }

        protected void processPortalRequest()
        {
            if (functionSelectedValue.Equals("Get Portal Information"))
            {
                if (txtPortalID.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    port = new WSPortal();
                    txtPortalID.Enabled = false;
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "Below are the results.  Select the desired portal the click " +
                        "Update or Request New Key.";
                    grdResult.DataSource = port.getPortalInformation(Convert.ToInt32(txtPortalID.Text));
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                }
            }
            else if (functionSelectedValue.Equals("Add New Portal"))
            {
                if (txtOwnerName.Text == "" || txtOwnerEmail.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter an owner name, email";
                }
                else
                {
                    port = new WSPortal();
                    List<String> methods = new List<String>();
                    foreach (ListItem item in lstMethods.Items)
                    {
                        methods.Add(item.ToString());
                    }
                    string[] passedMethods = methods.ToArray();

                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = port.createNewPortal(txtOwnerName.Text, txtOwnerEmail.Text, passedMethods);
                }
            }
            
        }

        public void setAdministrationScreen()
        {
            if (functionSelectedValue.Equals("Get Portal Information"))
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Enter a portal ID to retrieve.  If you want to return every portal enter 0.  " + 
                    "Results will display in a table.  Select the desired portal to update portal information or " + 
                    "request a new key.";
                tblPortal.Visible = true;
                lblPortalID.Visible = true;
                txtPortalID.Visible = true;
                txtPortalID.Text = "0";
                txtPortalID.Enabled = true;
                lblOwnerName.Text = "Owner Name";
                lblOwnerEmail.Text = "Owner Email";
                lblPortalID.Text = "Portal ID";
                lblActive.Text = "Active";
                btnSubmit.Visible = true;
            }
            else if (functionSelectedValue.Equals("Add New Portal"))
            {
                port = new WSPortal();

                lblDirection.Visible = true;
                lblDirection.Text = "Enter the owner name and email and select the needed methods then click Submit.";

                btnSubmit.Visible = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;

                tblPortal.Visible = true;
                tblMethods.Visible = true;

                lblOwnerName.Visible = true;
                lblOwnerEmail.Visible = true;
                lblOwnerName.Text = "Owner Name";
                lblOwnerEmail.Text = "Owner Email";
                lblPortalID.Text = "Portal ID";
                lblActive.Text = "Active";
                txtOwnerName.Visible = true;
                txtOwnerEmail.Visible = true;

                lblPortalID.Visible = true;
                txtPortalID.Visible = true;
                txtPortalID.Text = "0";
                txtPortalID.Enabled = false;

                lblMethods.Visible = true;
                lstMethods.Visible = true;
                lblAvailableMethods.Visible = true;
                lstAvailableMethods.Visible = true;

                lstAvailableMethods.DataSource = port.getAvailablePortalMethods(Convert.ToInt32(txtPortalID.Text));
                lstAvailableMethods.DataBind();
            }
            else
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Click Process to test the email functionality.";
            }
        }

        public void resetPage()
        {

            lblDirection.Visible = false;

            lblResult.Visible = false;
            txtResult.Visible = false;
            txtResult.Text = "";

            btnSubmit.Visible = false;
            btnUpdate.Visible = false;
            btnNewKey.Visible = false;
            btnAdd.Visible = false;
            btnRemove.Visible = false;

            tblPortal.Visible = false;
            tblMethods.Visible = false;

            lblOwnerName.Visible = false;
            lblOwnerEmail.Visible = false;
            lblEmail.Visible = false;
            lblPortalID.Visible = false;
            lblActive.Visible = false;
            txtOwnerName.Visible = false;
            txtOwnerEmail.Visible = false;
            txtEmail.Visible = false;
            txtPortalID.Visible = false;
            txtPortalID.Enabled = false;
            txtActive.Visible = false;
            txtOwnerName.Text = "";
            txtOwnerEmail.Text = "";
            txtEmail.Text = "";
            txtPortalID.Text = "";
            txtActive.Text = "";

            lblMethods.Visible = false;
            lstMethods.Visible = false;
            lblAvailableMethods.Visible = false;
            lstAvailableMethods.Visible = false;

            grdResult.Visible = false;
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
}
