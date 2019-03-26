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
    public partial class PropertyTax : System.Web.UI.Page
    {
        private string taxFuncSelectValue = String.Empty;

        private String resultText = String.Empty;

        private GridViewRow grdViewRow;
        private Property property;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Page_Init(object sender, EventArgs e)
        {

            resetPage();

            cmbFunction.Items.Clear();
            cmbFunction.Items.Add("Get Personal Balance by Account Number");
            cmbFunction.Items.Add("Get Personal Balance by Owner Name");
            cmbFunction.Items.Add("Get Personal Balance by Address");
            cmbFunction.Items.Add("Make a Personal Payment");
            cmbFunction.Items.Add("Void a Personal Payment");
            cmbFunction.Items.Add("Get Real Estate Balance by Parcel Number");
            cmbFunction.Items.Add("Get Real Estate Balance by Last Name");
            cmbFunction.Items.Add("Get Real Estate Balance by Address");
            cmbFunction.Items.Add("Make a Real Estate Payment");
            cmbFunction.Items.Add("Void a Real Estate Payment");

            if (IsPostBack == false)
            {
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            }
            else
            {
                taxFuncSelectValue = Session["sessTaxFuncSelVal"].ToString();
            }
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            taxFuncSelectValue = cmbFunction.SelectedValue;
            Session["sessTaxFuncSelVal"] = taxFuncSelectValue;
            resetPage();
            setScreen();
        }

        protected void btnVoidPay_Click(object sender, EventArgs e)
        {

            if (txtUnitID.Text == "" && txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                if (taxFuncSelectValue.Contains("Personal"))
                {
                    txtResult.Text = "You must enter an Account Number and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
                }
                else
                {
                    txtResult.Text = "You must enter an Parcel Number and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
                }
            }
            else
            {
                property = new Property();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");
                if (taxFuncSelectValue.Contains("Personal"))
                {
                    String type = "Void a Personal Payment";
                    txtResult.Text = property.buildPersonalPropertyPayVoid(type, txtUnitID.Text, Convert.ToDecimal(newBalance));
                }
                else
                {
                    String type = "Void a Real Estate Payment";
                    txtResult.Text = property.buildRealPropertyPayVoid(type, txtUnitID.Text, Convert.ToDecimal(newBalance));
                }
                txtUnitID.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnMakePay_Click(object sender, EventArgs e)
        {
            if (txtUnitID.Text == "" && txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                if (taxFuncSelectValue.Contains("Personal"))
                {
                    txtResult.Text = "You must enter an Account Number and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
                }
                else
                {
                    txtResult.Text = "You must enter an Parcel Number and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
                }
            }
            else
            {
                property = new Property();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");

                if (taxFuncSelectValue.Contains("Personal"))
                {
                    String type = "Make a Personal Payment";
                    txtResult.Text = property.buildPersonalPropertyPayVoid(type, txtUnitID.Text, Convert.ToDecimal(newBalance));
                }
                else
                {
                    String type = "Make a Real Estate Payment";
                    txtResult.Text = property.buildRealPropertyPayVoid(type, txtUnitID.Text, Convert.ToDecimal(newBalance));
                }
                txtUnitID.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            txtResult.Text = String.Empty;

            if (btnSubmit.Text == "Submit")
            {
                processRequest(taxFuncSelectValue);
            }
            else
            {
                resetPage();
                btnSubmit.Text = "Submit";
                cmbFunction.SelectedValue = Session["sessTaxFuncSelVal"].ToString();
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            } 
        }

        protected void processRequest(String type)
        {
            if (type == "Get Personal Balance by Account Number" || type == "Get Real Estate Balance by Parcel Number")
            {
                if (txtUnitID.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    property = new Property();
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    if (type == "Get Personal Balance by Account Number")
                    {
                        txtResult.Text = property.getPropertyByNumber("Personal", txtUnitID.Text).ToString();
                    }
                    else
                    {
                        txtResult.Text = property.getPropertyByNumber("Real", txtUnitID.Text).ToString();
                    }
                    if (property.result.Count > 0)
                    {
                        property = ((Property)property.result[0]);
                        txtUnitID.Text = property.Number;
                        lblBalanceDue.Visible = true;
                        lblBalanceDue.Text = "Payment Amount";
                        txtBalanceDue.Visible = true;
                        txtBalanceDue.Text = property.BalanceDue;
                        btnMakePay.Visible = true;
                        btnVoidPay.Visible = true;
                    }
                    btnSubmit.Text = "Clear";
                }
            }
            else if (type == "Get Personal Balance by Owner Name" || type == "Get Real Estate Balance by Last Name")
            {
                if (txtUnitID.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    property = new Property();
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    grdResult.Visible = true;
                    if (type == "Get Personal Balance by Owner Name")
                    {
                        txtResult.Text = property.getPropertyByName("Personal", txtUnitID.Text).ToString();
                    }
                    else
                    {
                        txtResult.Text = property.getPropertyByName("Real", txtUnitID.Text).ToString();
                    }
                    grdResult.DataSource = property.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    btnSubmit.Text = "Clear";
                }
            }
            else if (type == "Get Personal Balance by Address" || type == "Get Real Estate Balance by Address")
            {
                if (txtUnitID.Text == "" && txtClientID.Text == "" && txtClientName.Text == "" && txtClientAddress.Text == "" 
                        && txtDate.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    property = new Property();
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    grdResult.Visible = true;
                                        
                    if (type == "Get Personal Balance by Address")
                    {
                        txtResult.Text = property.getPropertyByAddress("Personal", txtUnitID.Text, txtClientID.Text,
                            txtClientName.Text, txtClientAddress.Text, txtDate.Text).ToString();
                    }
                    else
                    {
                        txtResult.Text = property.getPropertyByAddress("Real", txtUnitID.Text, txtClientID.Text,
                            txtClientName.Text, txtClientAddress.Text, txtDate.Text).ToString();
                    }
                    grdResult.DataSource = property.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    btnSubmit.Text = "Clear";
                }
            }
            else
            {
                if (txtUnitID.Text == "" && txtClientID.Text == "" && txtClientName.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtUnitID.Text = String.Empty;
                    txtClientID.Text = String.Empty;
                    txtClientName.Text = String.Empty;
                    btnSubmit.Text = "Clear";
                }
            }
        }

        protected void grdResult_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (taxFuncSelectValue.Contains("Get Real Estate Balance by Last Name") ||
                taxFuncSelectValue.Contains("Get Real Estate Balance by Address"))
            {
                e.Row.Cells[4].Text = "First Install";
                e.Row.Cells[5].Text = "Second Install";
                e.Row.Cells[6].Text = "Third Install";
                e.Row.Cells[7].Visible = false;
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
                e.Row.Cells[10].Visible = false;
                e.Row.Cells[11].Visible = false;
                e.Row.Cells[13].Visible = false;
            }
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str = String.Empty;
            grdViewRow = grdResult.SelectedRow;
            str = str + "Selected Record:\r\n\r\n";
            if (taxFuncSelectValue.Contains("Personal"))
            {
                str = str + "Number: " + grdViewRow.Cells[1].Text.Trim() +"\r\n";
                str = str + "Balance Due: " + grdViewRow.Cells[13].Text.Trim() + "\r\n\r\n";
            }
            else
            {
                str = str + "Number: " + grdViewRow.Cells[1].Text.Trim() +"\r\n";
                str = str + "Balance Due: " + grdViewRow.Cells[4].Text.Trim() + "\r\n\r\n";
            }
            str = str + "Adjust the Payment Amount, if necessary, then click Make Payment or Void Payment to process this record.";
            if (taxFuncSelectValue.Contains("Personal"))
            {
                txtUnitID.Text = grdViewRow.Cells[1].Text.Trim();
                txtBalanceDue.Text = grdViewRow.Cells[13].Text.Trim();
                lblUnitID.Text = "Account Number";
                lblBalanceDue.Text = "Payment/Void Amount";
            }
            else
            {
                txtUnitID.Text = grdViewRow.Cells[1].Text.Trim();
                txtBalanceDue.Text = grdViewRow.Cells[4].Text.Trim();
                lblUnitID.Text = "Parcel Number";
                lblBalanceDue.Text = "First Install Amount";
            }
            txtResult.Text = str;                 
            lblUnitID.Visible = true;
            lblBalanceDue.Visible = true;
            txtUnitID.Visible = true;
            txtBalanceDue.Visible = true;
            btnMakePay.Visible = true;
            btnVoidPay.Visible = true;
            lblClientID.Visible = false;
            lblClientName.Visible = false;
            lblClientAddress.Visible = false;
            lblDate.Visible = false;
            txtClientID.Visible = false;
            txtClientName.Visible = false;
            txtClientAddress.Visible = false;
            txtDate.Visible = false;
            txtClientID.Text = "";
            txtClientName.Text = "";
            txtClientAddress.Text = "";
            txtDate.Text = "";

        }

        protected void txtBalanceDue_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(txtBalanceDue.Text, @"^\s*\$?\s*\d{1,3}((,\d{3})*|\d*)(\.\d{2})?\s*$"))
            {
                // It is not a number, do something!
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(txtDate.Text, @"^(((0[1-9]|[12]\d|3[01])/(0[13578]|1[02])/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)/(0[13456789]|1[012])/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])/02/((19|[2-9]\d)\d{2}))|(29/02/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$"))
            {
                // It is not a number, do something!
            }
        }

        public void setScreen()
        {
            lblDirection.Visible = true;

            if (taxFuncSelectValue.Equals("Get Personal Balance by Account Number"))
            {
                txtUnitID.Visible = true;
                lblUnitID.Visible = true;
                lblUnitID.Text = "Account Number";
                lblDirection.Text = "Enter the account number and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Get Personal Balance by Owner Name"))
            {
                txtUnitID.Visible = true;
                lblUnitID.Visible = true;
                lblUnitID.Text = "Owner Name";
                lblDirection.Text = "Enter the owner name and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Get Personal Balance by Address"))
            {
                lblUnitID.Text = "House Number";
                lblClientID.Text = "Street Direction";
                lblClientName.Text = "Street Name";
                lblClientAddress.Text = "Street Type";
                lblDate.Text = "Unit";
                lblUnitID.Visible = true;
                lblClientID.Visible = true;
                lblClientName.Visible = true;
                lblClientAddress.Visible = true;
                lblDate.Visible = true;
                txtUnitID.Text = String.Empty;
                txtClientID.Text = String.Empty;
                txtClientName.Text = String.Empty;
                txtClientAddress.Text = String.Empty;
                txtDate.Text = String.Empty;
                txtUnitID.Visible = true;
                txtClientID.Visible = true;
                txtClientName.Visible = true;
                txtClientAddress.Visible = true;
                txtDate.Visible = true;
                
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Get Real Estate Balance by Parcel Number"))
            {
                txtUnitID.Visible = true;
                lblUnitID.Visible = true;
                lblUnitID.Text = "Parcel Number";
                lblDirection.Text = "Enter the parcel number and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Get Real Estate Balance by Last Name"))
            {
                txtUnitID.Visible = true;
                lblUnitID.Visible = true;
                lblUnitID.Text = "Last Name";
                lblDirection.Text = "Enter the last name and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Get Real Estate Balance by Address"))
            {
                lblUnitID.Text = "House Number";
                lblClientID.Text = "Street Direction";
                lblClientName.Text = "Street Name";
                lblClientAddress.Text = "Street Type";
                lblDate.Text = "Unit";
                lblUnitID.Visible = true;
                lblClientID.Visible = true;
                lblClientName.Visible = true;
                lblClientAddress.Visible = true;
                lblDate.Visible = true;
                txtUnitID.Text = String.Empty;
                txtClientID.Text = String.Empty;
                txtClientName.Text = String.Empty;
                txtClientAddress.Text = String.Empty;
                txtDate.Text = String.Empty;
                txtUnitID.Visible = true;
                txtClientID.Visible = true;
                txtClientName.Visible = true;
                txtClientAddress.Visible = true;
                txtDate.Visible = true;

                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Void a Personal Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to void a payment.";
                lblUnitID.Text = "Account Number";
                lblUnitID.Visible = true;
                txtUnitID.Text = String.Empty;
                txtUnitID.Visible = true;
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Void Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnVoidPay.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Make a Personal Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to make a payment.";
                lblUnitID.Text = "Account Number";
                lblUnitID.Visible = true;
                txtUnitID.Text = String.Empty;
                txtUnitID.Visible = true;
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Payment Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnMakePay.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Void a Real Estate Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to void a payment.";
                lblUnitID.Text = "Parcel Number";
                lblUnitID.Visible = true;
                txtUnitID.Text = String.Empty;
                txtUnitID.Visible = true;
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Void Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnVoidPay.Visible = true;
            }
            else if (taxFuncSelectValue.Equals("Make a Real Estate Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to make a payment.";
                lblUnitID.Text = "Parcel Number";
                lblUnitID.Visible = true;
                txtUnitID.Text = String.Empty;
                txtUnitID.Visible = true;
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Payment Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnMakePay.Visible = true;
            }
            else
            {
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
        }
                
        public void resetPage()
        {
            lblDirection.Visible = false;
            
            lblResult.Visible = false;
            txtResult.Visible = false;
            txtResult.Text = String.Empty;

            tblPropertyTax.Visible = true;

            lblUnitID.Visible = false;
            lblUnitID.Text = String.Empty;
            txtUnitID.Visible = false;
            txtUnitID.Text = String.Empty;

            lblClientID.Visible = false;
            lblClientID.Text = String.Empty;
            txtClientID.Visible = false;
            txtClientID.Text = String.Empty;

            lblClientName.Visible = false;
            lblClientName.Text = String.Empty;
            txtClientName.Visible = false;
            txtClientName.Text = String.Empty;

            lblClientAddress.Visible = false;
            lblClientAddress.Text = String.Empty;
            txtClientAddress.Visible = false;
            txtClientAddress.Text = String.Empty;

            lblDate.Visible = false;
            lblDate.Text = String.Empty;
            txtDate.Visible = false;
            txtDate.Text = String.Empty;

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