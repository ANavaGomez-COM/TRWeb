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
    public partial class Ticket : System.Web.UI.Page
    {
        private string ticketFuncSelectedValue = String.Empty;

        private GridViewRow grdViewRow;
        private ParkingTicket pTicket;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
            resetPage();

            cmbFunction.Items.Clear();
            cmbFunction.Items.Add("Get Balance By Parking Ticket Number");
            cmbFunction.Items.Add("Get Parking Ticket List");
            cmbFunction.Items.Add("Make a Payment");
            cmbFunction.Items.Add("Void a Payment");

            if (IsPostBack == false)
            {
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            }
            else
            {
                ticketFuncSelectedValue = Session["sessTicketFuncSelVal"].ToString();
            }
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ticketFuncSelectedValue = cmbFunction.SelectedValue;
            Session["sessTicketFuncSelVal"] = ticketFuncSelectedValue;
            resetPage();
            setScreen();
        }

        protected void btnVoidPay_Click(object sender, EventArgs e)
        {
            if (txtTicketNumber.Text == "" || txtLicensePlateNum.Text == "" || txtLicensePlateSt.Text == "" || txtLicensePlateTyp.Text == "" ||
                txtDate.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter an Ticket Number, License Plate Number, License Plate Number, " +
                    " License Plate State, License Plate Type, Violation Date and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00" + Environment.NewLine + 
                "Violation Date must be formatted MM/DD/YYYY";
            }
            else
            {
                pTicket = new ParkingTicket();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");
                String type = "Void Payment";

                txtResult.Text = pTicket.buildParkingTicketPayVoid(type, txtTicketNumber.Text, txtLicensePlateNum.Text,
                        txtLicensePlateSt.Text, txtLicensePlateTyp.Text, Convert.ToDateTime(txtDate.Text), Convert.ToDecimal(newBalance));
                txtTicketNumber.Text = String.Empty;
                txtLicensePlateNum.Text = String.Empty;
                txtLicensePlateSt.Text = String.Empty;
                txtLicensePlateTyp.Text = String.Empty;
                txtDate.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnMakePay_Click(object sender, EventArgs e)
        {
            if (txtTicketNumber.Text == "" || txtLicensePlateNum.Text == "" || txtLicensePlateSt.Text == "" || txtLicensePlateTyp.Text == "" ||
                    txtDate.Text == "" || txtBalanceDue.Text == "")
            {
                lblResult.Visible = true;
                txtResult.Visible = true;
                txtResult.Text = "You must enter a Ticket Number, License Plate Number, License Plate Number, " +
                    " License Plate State, License Plate Type, Violation Date and Balance Due." + Environment.NewLine +
                Environment.NewLine + "Balance Due must be formatted 000.00" + Environment.NewLine + "Violation Date must be formatted MM/DD/YYYY";
            }
            else
            {
                pTicket = new ParkingTicket();
                lblResult.Visible = true;
                txtResult.Visible = true;
                String newBalance = txtBalanceDue.Text.Replace("$", "");
                String type = "Make Payment";

                txtResult.Text = pTicket.buildParkingTicketPayVoid(type, txtTicketNumber.Text, txtLicensePlateNum.Text, 
                        txtLicensePlateSt.Text, txtLicensePlateTyp.Text, Convert.ToDateTime(txtDate.Text), Convert.ToDecimal(newBalance));
                txtTicketNumber.Text = String.Empty;
                txtLicensePlateNum.Text = String.Empty;
                txtLicensePlateSt.Text = String.Empty;
                txtLicensePlateTyp.Text = String.Empty;
                txtDate.Text = String.Empty;
                txtBalanceDue.Text = String.Empty;
                btnSubmit.Text = "Clear";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            txtResult.Text = String.Empty;

            if (btnSubmit.Text == "Submit")
            {
                processRequest(ticketFuncSelectedValue);
            }
            else
            {
                resetPage();
                btnSubmit.Text = "Submit";
                cmbFunction.SelectedValue = Session["sessTicketFuncSelVal"].ToString();
                this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
            } 
        }

        protected void processRequest(String type)
        {
            if (type == "Get Balance By Parking Ticket Number")
            {
                if (txtTicketNumber.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    pTicket = new ParkingTicket();
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = pTicket.buildPTicketBalance(txtTicketNumber.Text).ToString();
                    grdResult.DataSource = pTicket.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    txtTicketNumber.Text = String.Empty;
                    btnSubmit.Text = "Clear";
                }
            }
            else
            {
                if (txtLicensePlateNum.Text == "" && txtLicensePlateSt.Text == "" && txtLicensePlateTyp.Text == "")
                {
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = "You must enter some search criteria.";
                }
                else
                {
                    pTicket = new ParkingTicket();
                    grdResult.Visible = true;
                    lblResult.Visible = true;
                    txtResult.Visible = true;
                    txtResult.Text = pTicket.buildPTicketList(txtLicensePlateNum.Text, txtLicensePlateSt.Text
                        , txtLicensePlateTyp.Text).ToString();
                    grdResult.DataSource = pTicket.result;
                    grdResult.DataBind();
                    grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
                    txtLicensePlateNum.Text = String.Empty;
                    txtLicensePlateSt.Text = String.Empty;
                    txtLicensePlateTyp.Text = String.Empty;
                    btnSubmit.Text = "Clear";
                }
            }
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str = String.Empty;
            grdViewRow = grdResult.SelectedRow;
            String vDate = String.Empty;
            if (grdViewRow.Cells[5].Text.Trim() == "")
            {
                vDate = "";
            }
            else
            {
                vDate = grdViewRow.Cells[5].Text.Trim();
                String year = vDate.Substring(0, 4);
                String month = vDate.Substring(4, 2);
                String day = vDate.Substring(6, 2);
                vDate = month + "/" + day + "/" + year;
            }
            str = str + "Selected Record:\r\n\r\n";
            str = str + "Ticket Number: " + grdViewRow.Cells[1].Text.Trim() + "\r\n";
            str = str + "License Plate Number: " + grdViewRow.Cells[2].Text.Trim() + "\r\n";
            str = str + "License Plate State: " + grdViewRow.Cells[3].Text.Trim() + "\r\n";
            str = str + "License Plate Type: " + grdViewRow.Cells[4].Text.Trim() + "\r\n";
            str = str + "Violation Date: " + vDate + "\r\n";
            str = str + "Balance Due: " + grdViewRow.Cells[6].Text.Trim() + "\r\n\r\n";
            str = str + "Adjust the Payment Amount, if necessary, then click Make Payment or Void Payment to process this record.";
            txtTicketNumber.Text = grdViewRow.Cells[1].Text.Trim();
            txtLicensePlateNum.Text = grdViewRow.Cells[2].Text.Trim();
            txtLicensePlateSt.Text = grdViewRow.Cells[3].Text.Trim();
            txtLicensePlateTyp.Text = grdViewRow.Cells[4].Text.Trim();
            txtDate.Text = vDate;
            txtBalanceDue.Text = grdViewRow.Cells[6].Text.Trim();
            txtResult.Text = str;
            lblTicketNumber.Visible = true;
            txtTicketNumber.Visible = true;
            lblLicensePlateSt.Visible = true;
            txtLicensePlateSt.Visible = true;
            lblLicensePlateTyp.Visible = true;
            txtLicensePlateTyp.Visible = true;
            lblDate.Visible = true;
            txtDate.Visible = true;
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
            tblParkingTicket.Visible = true;

            if (ticketFuncSelectedValue.Equals("Get Balance By Parking Ticket Number"))
            {
                lblDirection.Text = "Enter the ticket number below and click Submit to retrieve balance information.";
                lblTicketNumber.Visible = true;
                txtTicketNumber.Visible = true;
                txtTicketNumber.Text = String.Empty;
                btnSubmit.Visible = true;
            }
            else if (ticketFuncSelectedValue.Equals("Get Parking Ticket List"))
            {
                lblDirection.Text = "Enter any of the information below and click Submit to retrieve balance information.";
                lblLicensePlateNum.Visible = true;
                txtLicensePlateNum.Visible = true;
                txtLicensePlateNum.Text = String.Empty;
                lblLicensePlateSt.Visible = true;
                txtLicensePlateSt.Visible = true;
                txtLicensePlateSt.Text = String.Empty;
                lblLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Text = String.Empty;
                btnSubmit.Visible = true;
            }
            else if (ticketFuncSelectedValue.Equals("Void a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to void a payment.";
                lblTicketNumber.Visible = true;
                txtTicketNumber.Visible = true;
                txtTicketNumber.Text = String.Empty;
                lblLicensePlateNum.Visible = true;
                txtLicensePlateNum.Visible = true;
                txtLicensePlateNum.Text = String.Empty;
                lblLicensePlateSt.Visible = true;
                txtLicensePlateSt.Visible = true;
                txtLicensePlateSt.Text = String.Empty;
                lblLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Text = String.Empty;
                lblDate.Visible = true;
                lblDate.Text = "Violation Date";
                txtDate.Visible = true;
                txtDate.Text = String.Empty;
                lblBalanceDue.Visible = true;
                lblBalanceDue.Text = "Void Amount";
                txtBalanceDue.Visible = true;
                txtBalanceDue.Text = String.Empty;
                btnVoidPay.Visible = true;
            }
            else if (ticketFuncSelectedValue.Equals("Make a Payment"))
            {
                lblDirection.Text = "Enter the information below and click Submit to make a payment.";
                lblTicketNumber.Visible = true;
                txtTicketNumber.Visible = true;
                txtTicketNumber.Text = String.Empty;
                lblLicensePlateNum.Visible = true;
                txtLicensePlateNum.Visible = true;
                txtLicensePlateNum.Text = String.Empty;
                lblLicensePlateSt.Visible = true;
                txtLicensePlateSt.Visible = true;
                txtLicensePlateSt.Text = String.Empty;
                lblLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Visible = true;
                txtLicensePlateTyp.Text = String.Empty;
                lblDate.Visible = true;
                lblDate.Text = "Violation Date";
                txtDate.Visible = true;
                txtDate.Text = String.Empty;
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

            tblParkingTicket.Visible = true;

            lblTicketNumber.Visible = false;
            txtTicketNumber.Visible = false;
            txtTicketNumber.Text = String.Empty;

            lblLicensePlateNum.Visible = false;
            txtLicensePlateNum.Visible = false;
            txtLicensePlateNum.Text = String.Empty;

            lblLicensePlateSt.Visible = false;
            txtLicensePlateSt.Visible = false;
            txtLicensePlateSt.Text = String.Empty;

            lblLicensePlateTyp.Visible = false;
            txtLicensePlateTyp.Visible = false;
            txtLicensePlateTyp.Text = String.Empty;

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