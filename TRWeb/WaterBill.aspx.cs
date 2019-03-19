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
    public partial class WaterBill : System.Web.UI.Page
    {
        private string watFuncSelectedValue = "";
        private GridViewRow grdViewRow;
        private Water water;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            resetPage();
            cmbFunction.Items.Clear();
            cmbFunction.Items.Add("Get Balance By Customer Account");
            cmbFunction.Items.Add("Get Balance By Customer Name");
            cmbFunction.Items.Add("Get Balance By Address");
            cmbFunction.Items.Add("Make a Payment");
            cmbFunction.Items.Add("Void a Payment");

            if (IsPostBack == false)
            {
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            }
            else
            {
                watFuncSelectedValue = Session["sessWatFuncSelVal"].ToString();
            }
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            watFuncSelectedValue = cmbFunction.SelectedValue;
            Session["sessWatFuncSelVal"] = watFuncSelectedValue;
            resetPage();
            setScreen();
        }

        protected void btnVoidPay_Click(object sender, EventArgs e)
        {
            if (txtFirst.Text == "" || txtSecond.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter an Account Number, Customer Number, and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
            }
            else
            {
                water = new Water();
                lblResult.Visible = true;
                txtResult.Visible = true;
                string newBalance = txtBalanceDue.Text.Replace("$", "");
                string type = "Void Payment";

                txtResult.Text = water.buildWaterPayVoid(type, txtFirst.Text, txtSecond.Text, Convert.ToDecimal(newBalance));
                txtFirst.Text = "";
                txtSecond.Text = "";
                txtUnit.Text = "";
                txtBalanceDue.Text = "";
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnMakePay_Click(object sender, EventArgs e)
        {
            if (txtFirst.Text == "" || txtSecond.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter an Account Number, Customer Number, and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00";
            }
            else
            {
                water = new Water();
                lblResult.Visible = true;
                txtResult.Visible = true;
                string newBalance = txtBalanceDue.Text.Replace("$", "");
                string type = "Make Payment";

                txtResult.Text = water.buildWaterPayVoid(type, txtFirst.Text, txtSecond.Text, Convert.ToDecimal(newBalance));
                txtFirst.Text = "";
                txtSecond.Text = "";
                txtUnit.Text = "";
                txtBalanceDue.Text = "";
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";

            if (btnSubmit.Text == "Submit")
            {
                processRequest(watFuncSelectedValue);
            }
            else
            {
                resetPage();
                btnSubmit.Text = "Submit";
                cmbFunction.SelectedValue = Session["sessWatFuncSelVal"].ToString();
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            } 
        }

        protected void processRequest(string type)
        {
            if (type == "Get Balance By Customer Account" || type == "Get Balance By Customer Name")
            {
                if (txtFirst.Text == "" || txtSecond.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    water = new Water();
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    if (type == "Get Balance By Customer Account")
                    {
                        txtResult.Text = water.buildWaterAccountBalance(txtFirst.Text, txtSecond.Text).ToString();
                    }
                    else
                    {
                        txtResult.Text = water.buildWaterNameBalance(txtFirst.Text, txtSecond.Text).ToString();
                    }
                    grdResult.DataSource = water.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    txtFirst.Text = "";
                    txtSecond.Text = "";
                    btnSubmit.Text = "Clear";
                }
            }
            else
            {
                if (txtFirst.Text == "" && txtSecond.Text == "" && txtUnit.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    water = new Water();
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = water.buildWaterAddressBalance(txtFirst.Text, txtSecond.Text, txtUnit.Text).ToString();
                    grdResult.DataSource = water.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    txtFirst.Text = "";
                    txtSecond.Text = "";
                    txtUnit.Text = "";
                    btnSubmit.Text = "Clear";
                }
            }
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = "";
            grdViewRow = grdResult.SelectedRow;
            string account = "";
            string unit = "";
            string first = "";
            if (grdViewRow.Cells[1].Text == "&nbsp;")
            {
                account = "";
            }
            else
            {
                account = grdViewRow.Cells[1].Text.Trim();
            }
            if (grdViewRow.Cells[3].Text == "&nbsp;")
            {
                first = "";
            }
            else
            {
                first = grdViewRow.Cells[3].Text.Trim();
            }
            if (grdViewRow.Cells[7].Text == "&nbsp;")
            {
                unit = "";
            }
            else
            {
                unit = grdViewRow.Cells[7].Text.Trim();
            }
            str = str + "Selected Record:\r\n\r\n";
            str = str + "Account: " + account + "\r\n";
            str = str + "Customer: " + grdViewRow.Cells[2].Text.Trim() + "\r\n";
            str = str + "First Name: " + first + "\r\n";
            str = str + "Last Name: " + grdViewRow.Cells[4].Text.Trim() + "\r\n";
            str = str + "Street Number: " + grdViewRow.Cells[5].Text.Trim() + "\r\n";
            str = str + "Street Name: " + grdViewRow.Cells[6].Text.Trim() + "\r\n";
            str = str + "Unit: " + unit + "\r\n";
            str = str + "Balance Due: " + grdViewRow.Cells[8].Text.Trim() + "\r\n\r\n";
            str = str + "Adjust the Payment Amount, if necessary, then click Make Payment or Void Payment to process this record.";
            txtFirst.Text = account;
            txtSecond.Text = grdViewRow.Cells[2].Text.Trim();
            txtUnit.Text = first + " " + grdViewRow.Cells[4].Text.Trim();
            txtBalanceDue.Text = grdViewRow.Cells[8].Text.Trim();
            txtResult.Text = str;
            lblFirst.Text = "Account";
            lblSecond.Text = "Customer";
            lblUnit.Text = "Name";
            lblUnit.Visible = true;
            txtUnit.Visible = true;
            txtBalanceDue.Visible = true;
            lblBalanceDue.Visible = true;
            lblBalanceDue.Text = "Payment/Void Amount";
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

            tblWater.Visible = true;

            lblFirst.Visible = true;
            txtFirst.Visible = true;
            txtFirst.Text = "";

            lblSecond.Visible = true;
            txtSecond.Visible = true;
            txtSecond.Text = "";

            if (watFuncSelectedValue.Equals("Get Balance By Customer Account"))
            {
                lblFirst.Text = "Account Number";
                lblSecond.Text = "Customer Number";
                lblDirection.Text = "Enter the account information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (watFuncSelectedValue.Equals("Get Balance By Customer Name"))
            {
                lblFirst.Text = "First Name";
                lblSecond.Text = "Last Name";
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (watFuncSelectedValue.Equals("Get Balance By Address"))
            {
                lblFirst.Text = "Street Number";
                lblSecond.Text = "Street Name";
                lblUnit.Text = "Unit"; 
                lblUnit.Visible = true;
                txtUnit.Visible = true;
                txtUnit.Text = "";
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                btnSubmit.Visible = true;
            }
            else if (watFuncSelectedValue.Equals("Void a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to void a payment.";
                lblFirst.Text = "Account";
                lblSecond.Text = "Customer";
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Void Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = "";
                btnVoidPay.Visible = true;
            }
            else if (watFuncSelectedValue.Equals("Make a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to make a payment.";
                lblFirst.Text = "Account";
                lblSecond.Text = "Customer";
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Payment Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = "";
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
            txtResult.Text = "";

            tblWater.Visible = true;

            lblFirst.Visible = false;
            lblFirst.Text = "";
            txtFirst.Visible = false;
            txtFirst.Text = "";

            lblSecond.Visible = false;
            lblSecond.Text = "";
            txtSecond.Visible = false;
            txtSecond.Text = "";

            lblUnit.Visible = false;
            lblUnit.Text = "";
            txtUnit.Visible = false;
            txtUnit.Text = "";

            lblBalanceDue.Visible = false;
            lblBalanceDue.Text = "";
            txtBalanceDue.Visible = false;
            txtBalanceDue.Text = "";

            btnMakePay.Visible = false;
            btnVoidPay.Visible = false;
            btnSubmit.Visible = false;
            btnSubmit.Text = "Submit";

            grdResult.Visible = false;
        }

    }
}