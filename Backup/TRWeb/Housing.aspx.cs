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

namespace TRWeb
{
    public partial class Housing : System.Web.UI.Page
    {

        private string cdaFuncSelectedValue = String.Empty;
 
        private GridViewRow grdViewRow;
        private CDA cda;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            resetPage();

            cmbFunction.Items.Clear();
            cmbFunction.Items.Add("Get CDA Balance");
            cmbFunction.Items.Add("Make a Payment");
            cmbFunction.Items.Add("Void a Payment");
    
            if (IsPostBack == false)
            {
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            }
            else
            {
                cdaFuncSelectedValue = Session["sessCDAFuncSelVal"].ToString();
            }
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            cdaFuncSelectedValue = cmbFunction.SelectedValue;
            Session["sessCDAFuncSelVal"] = cdaFuncSelectedValue;
            resetPage();
            setScreen();
        }

        protected void btnVoidPay_Click(object sender, EventArgs e)
        {
            if (txtUnitID.Text == "" || txtClientID.Text == "" || txtClientName.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter a Client ID, Client Name, Unit ID, and Balance Due.  " + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
            }
            else
            {
                cda = new CDA();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");
                String type = "Void Payment";

                txtResult.Text = cda.buildCDAPayVoid(type, txtUnitID.Text, txtClientID.Text, txtClientName.Text, Convert.ToDecimal(newBalance));
                txtUnitID.Text = String.Empty;
                txtClientID.Text = String.Empty;
                txtClientName.Text = String.Empty;
                txtClientAddress.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnMakePay.Visible = false;
                btnVoidPay.Visible = false;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnMakePay_Click(object sender, EventArgs e)
        {
            if (txtUnitID.Text == "" || txtClientID.Text == "" || txtClientName.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter a Client ID, Client Name, Unit ID, and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
            }
            else
            {
                cda = new CDA();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");
                String type = "Make Payment";

                txtResult.Text = cda.buildCDAPayVoid(type, txtUnitID.Text, txtClientID.Text, txtClientName.Text, Convert.ToDecimal(newBalance));
                txtUnitID.Text = String.Empty;
                txtClientID.Text = String.Empty;
                txtClientName.Text = String.Empty;
                txtClientAddress.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            txtResult.Text = String.Empty;

            if (btnSubmit.Text == "Submit")
            {
                processRequest();
            }
            else
            {
                resetPage();
                btnSubmit.Text = "Submit";
                cmbFunction.SelectedValue = Session["sessCDAFuncSelVal"].ToString();
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            } 
        }

        protected void processRequest()
        {
            if (txtUnitID.Text == "" && txtClientID.Text == "" && txtClientName.Text == "" && txtClientAddress.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter some search criteria.";
            }
            else
            {
                cda = new CDA();
                grdResult.Visible = true;
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = cda.buildCDABalance(txtUnitID.Text, txtClientID.Text, txtClientName.Text, txtClientAddress.Text).ToString();
                grdResult.DataSource = cda.result;
                grdResult.DataBind();
                grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                txtUnitID.Text = String.Empty;
                txtClientID.Text = String.Empty;
                txtClientName.Text = String.Empty;
                txtClientAddress.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str = String.Empty;
            grdViewRow = grdResult.SelectedRow;
            str = str + "Selected Record:\r\n\r\n";
            str = str + "Client ID: " + grdViewRow.Cells[1].Text.Trim() + "\r\n";
            str = str + "Client Name: " + grdViewRow.Cells[2].Text.Trim() + "\r\n";
            str = str + "Unit ID: " + grdViewRow.Cells[3].Text.Trim() + "\r\n";
            str = str + "Address: " + grdViewRow.Cells[4].Text.Trim() + "\r\n";
            str = str + "Balance Due: " + grdViewRow.Cells[5].Text.Trim() + "\r\n\r\n";
            str = str + "Adjust the Payment Amount, if necessary, then click Make Payment or Void Payment to process this record.";
            txtClientID.Text = grdViewRow.Cells[1].Text.Trim();
            txtClientName.Text = grdViewRow.Cells[2].Text.Trim();
            txtUnitID.Text = grdViewRow.Cells[3].Text.Trim();
            txtClientAddress.Text = grdViewRow.Cells[4].Text.Trim();
            txtBalanceDue.Text = grdViewRow.Cells[5].Text.Trim();
            txtResult.Text = str;
            lblBalanceDue.Visible = true;
            lblBalanceDue.Text = "Payment//Void Amount";
            txtBalanceDue.Visible = true;
            btnMakePay.Visible = true;
            btnVoidPay.Visible = true;
        }

        protected void txtBalanceDue_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(txtBalanceDue.Text, @"^\s*\$?\s*\d{1,3}((,\d{3})*|\d*)(\.\d{2})?\s*$"))
            {
                // It is not a number, do something!
            }
        }

        public void setScreen()
        {
            lblDirection.Visible = true;
            tblCDAForm.Visible = true;

            txtUnitID.Text = String.Empty;
            txtClientID.Text = String.Empty;
            txtClientName.Text = String.Empty;
            txtClientAddress.Text = String.Empty;

            if (cdaFuncSelectedValue.Equals("Void a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to void a payment.";
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Void Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnVoidPay.Visible = true;
            }
            else if (cdaFuncSelectedValue.Equals("Make a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to make a payment.";
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Payment Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnMakePay.Visible = true;
            }
            else
            {
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                lblBalanceDue.Text = "Balance Due";
                btnSubmit.Visible = true;
            }
        }
        
        public void resetPage()
        {
            lblDirection.Visible = false;
            
            lblResult.Visible = false;
            txtResult.Visible = false;
            txtResult.Text = String.Empty;

            tblCDAForm.Visible = true;

            txtUnitID.Text = String.Empty;
            txtClientID.Text = String.Empty;
            txtClientName.Text = String.Empty;
            txtClientAddress.Text = String.Empty;

            lblBalanceDue.Visible = false;
            lblBalanceDue.Text = String.Empty;
            txtBalanceDue.Visible = false;
            txtBalanceDue.Text = String.Empty;

            btnMakePay.Visible = false;
            btnVoidPay.Visible = false;
            btnSubmit.Visible = false;
            btnSubmit.Text = "Submit";

            grdResult.Visible = false;
        }
    }
}