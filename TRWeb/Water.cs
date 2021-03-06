﻿//Class: EventDA
//Date: 8/15/2014
//Programmer: Jeff Moyer
//Description:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using TRWeb.CashieringService;
using System.Collections;

namespace TRWeb
{
    public class Water
    {
        private string sb = "";
        public ArrayList result;
        private CashieringServiceClient wServiceWater;
        private WaterUtilityCreateRequest WaterPayReq = new WaterUtilityCreateRequest();
        private CreateDWRecordResponse WaterPayResp = new CreateDWRecordResponse();
        private WaterUtilityCustomerAccountRequest WaterAccountReq = new WaterUtilityCustomerAccountRequest();
        private WaterUtilityCustomerNameRequest WaterCustomerReq = new WaterUtilityCustomerNameRequest();
        private WaterUtilityAddressRequest WaterAddressReq = new WaterUtilityAddressRequest();
        private WaterUtilityResponse WaterBalResp = new WaterUtilityResponse();
        private long portalId = 0;
        private string portalKey = "";
        private string xAccount;
        private string xCustomer;
        private string xFirstName;
        private string xLastName;
        private string xStreetNumber;
        private string xStreetName;
        private string xUnit;
        private string xBalanceDue;

        public string Account 
        {
            get { return xAccount; }
            set { xAccount = value; }
        }

        public string Customer        
        {
            get { return xCustomer; }
            set { xCustomer = value; }
        }

        public string FirstName
        {
            get { return xFirstName; }
            set { xFirstName = value; }
        }

        public string LastName
        {
            get { return xLastName; }
            set { xLastName = value; }
        }

        public string StreetNumber
        {
            get { return xStreetNumber; }
            set { xStreetNumber = value; }
        }

        public string StreetName
        {
            get { return xStreetName; }
            set { xStreetName = value; }
        }

        public string Unit
        {
            get { return xUnit; }
            set { xUnit = value; }
        }

        public string BalanceDue
        {
            get { return xBalanceDue; }
            set { xBalanceDue = value; }
        }

        public string buildWaterAccountBalance(string accountNum, string customerNum) 
        {
            WaterAccountReq = new WaterUtilityCustomerAccountRequest();
            sb = "";

            portalId = 7;
            portalKey = "4BSCW4Nn%iOpN95";
                        
            sb = "You searched for:\r\n\r\n";
            
            WaterAccountReq.Account = accountNum;
            WaterAccountReq.Customer = customerNum;
            WaterAccountReq.SecurityPortalId = portalId;
            WaterAccountReq.SecurityKey = portalKey;
            sb = sb + "    AccountNumber: " + WaterAccountReq.Account.ToString() + Environment.NewLine;
            sb = sb + "    CustomerNumber: " + WaterAccountReq.Customer.ToString() + "\r\n\r\n";

            processResults("Water Account Balance", portalId, portalKey);

            return sb;
        }

        public string buildWaterNameBalance(string first, string last)
        {
            WaterCustomerReq = new WaterUtilityCustomerNameRequest();
            sb = "";

            portalId = 7;
            portalKey = "4BSCW4Nn%iOpN95";

            sb = "You searched for:\r\n\r\n";

            WaterCustomerReq.FirstName = first;
            WaterCustomerReq.LastName = last;
            WaterCustomerReq.SecurityPortalId = portalId;
            WaterCustomerReq.SecurityKey = portalKey;
            sb = sb + "    FirstName: " + WaterCustomerReq.FirstName.ToString() + Environment.NewLine;
            sb = sb + "    LastName: " + WaterCustomerReq.LastName.ToString() + "\r\n\r\n";

            processResults("Water Customer Balance", portalId, portalKey);

            return sb;
        }

        public string buildWaterAddressBalance(string number, string street, string unit)
        {
            WaterAddressReq = new WaterUtilityAddressRequest();
            sb = "";

            portalId = 7;
            portalKey = "4BSCW4Nn%iOpN95";

            sb = "You searched for:\r\n\r\n";

            WaterAddressReq.StreetNumber = number;
            WaterAddressReq.Street = street;
            WaterAddressReq.Unit = unit;
            WaterAddressReq.SecurityPortalId = portalId;
            WaterAddressReq.SecurityKey = portalKey;
            sb = sb + "    Number: " + WaterAddressReq.StreetNumber.ToString() + Environment.NewLine;
            sb = sb + "    Name: " + WaterAddressReq.Street.ToString() + Environment.NewLine; ;
            sb = sb + "    Unit: " + WaterAddressReq.Unit.ToString() + "\r\n\r\n";

            processResults("Water Address Balance", portalId, portalKey);

            return sb;
        }

        public string buildWaterPayVoid(string type, string accountNum, string customerNum, decimal paymentAmount)
        {
            WaterPayReq = new WaterUtilityCreateRequest();
            sb = "";

            portalId = 8;
            portalKey = "jB54DN9Z9lcVXIg";

            sb = sb + "The Request contains:\r\n";

            WaterPayReq.Account = accountNum;
            WaterPayReq.Customer = customerNum;
            WaterPayReq.PaymentAmount = paymentAmount;
            WaterPayReq.PaymentDate = DateTime.Now;
            WaterPayReq.PaymentId = "1";
            WaterPayReq.BatchId = 1;
            WaterPayReq.SecurityPortalId = portalId;
            WaterPayReq.SecurityKey = portalKey;
            sb = sb + "    Account: " + WaterPayReq.Account.ToString() + "\r\n";
            sb = sb + "    Customer: " + WaterPayReq.Customer.ToString() + "\r\n";
            sb = sb + "    PaymentAmount: " + WaterPayReq.PaymentAmount.ToString() + "\r\n";
            sb = sb + "    PaymentDate: " + WaterPayReq.PaymentDate.ToString() + "\r\n";

            processResults(type, portalId, portalKey);

            return sb;
        }

        public ArrayList processResults(string type, long portalId, string portalKey)
        {
            wServiceWater = new CashieringServiceClient();
            wServiceWater.Open();

            result = new ArrayList();

            if (type == "Water Account Balance")
            {

                sb = sb + "Your search results were returned with a status of ";
                
                WaterBalResp = new WaterUtilityResponse();
                WaterBalResp = wServiceWater.GetWaterByCustomerAccount(WaterAccountReq);
                
                sb = sb + WaterBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                
                if (WaterBalResp.Status == "Success")
                {    
                    if (WaterBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "The record returned is listed below.  Select the record to make or void a payment.";
                        
                        Water water = new Water();

                        water.Account = WaterBalResp.AccountNumber;
                        water.Customer = WaterBalResp.Customer;
                        water.FirstName = WaterBalResp.FirstName;
                        water.LastName = WaterBalResp.LastName;
                        water.StreetNumber = WaterBalResp.StreetNumber;
                        water.StreetName = WaterBalResp.StreetName;
                        water.BalanceDue = string.Format("{0:C}", WaterBalResp.BalanceDue);

                        result.Add(water);

                    }
                }
                else
                {
                    sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                }
            } 
            else if (type == "Water Customer Balance") 
            {
                sb = sb + "Your search results were returned with a status of ";
                WaterBalResp = new WaterUtilityResponse();
                Array arrGetBal = wServiceWater.GetWaterByCustomerName(WaterCustomerReq);
                WaterBalResp = ((WaterUtilityResponse)arrGetBal.GetValue(0));

                sb = sb + WaterBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                
                if (WaterBalResp.Status == "Success")
                {    
                    if (WaterBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment.";
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Water water = new Water();
                            WaterBalResp = new WaterUtilityResponse();
                            WaterBalResp = ((WaterUtilityResponse)arrGetBal.GetValue(r));

                            water.Account = WaterBalResp.AccountNumber;
                            water.Customer = WaterBalResp.Customer;
                            water.FirstName = WaterBalResp.FirstName;
                            water.LastName = WaterBalResp.LastName;
                            water.StreetNumber = WaterBalResp.StreetNumber;
                            water.StreetName = WaterBalResp.StreetName;
                            water.BalanceDue = string.Format("{0:C}", WaterBalResp.BalanceDue);

                            result.Add(water);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "Water Address Balance")
            {
                sb = sb + "Your search results were returned with a status of ";
                WaterBalResp = new WaterUtilityResponse();
                Array arrGetBal = wServiceWater.GetWaterByAddress(WaterAddressReq);
                WaterBalResp = ((WaterUtilityResponse)arrGetBal.GetValue(0));

                sb = sb + WaterBalResp.Status.ToString().ToLower() + ".\r\n\r\n";

                if (WaterBalResp.Status == "Success")
                {
                    if (WaterBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment.";
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Water water = new Water();
                            WaterBalResp = new WaterUtilityResponse();
                            WaterBalResp = ((WaterUtilityResponse)arrGetBal.GetValue(r));

                            water.Account = WaterBalResp.AccountNumber;
                            water.Customer = WaterBalResp.Customer;
                            water.FirstName = WaterBalResp.FirstName;
                            water.LastName = WaterBalResp.LastName;
                            water.StreetNumber = WaterBalResp.StreetNumber;
                            water.StreetName = WaterBalResp.StreetName;
                            water.BalanceDue = string.Format("{0:C}", WaterBalResp.BalanceDue);

                            result.Add(water);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + WaterBalResp.ErrorMessage.ToString() + "\r\n";
                }
            } 
            else
            {
                WaterPayResp = new CreateDWRecordResponse();
                sb = sb + "\r\nThe result from the " + type + "\r\n";
                if (type == "Make Payment") 
                {
                    WaterPayResp = wServiceWater.ApplyWaterPayment(WaterPayReq);
                } 
                else if (type == "Void Payment") 
                {
                    WaterPayResp = wServiceWater.VoidWaterPayment(WaterPayReq);
                }
                sb = sb + "Status: " + WaterPayResp.Status.ToString() + "\r\n";
                if (WaterPayResp.Status == "Success")
                {
                    if (WaterPayResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + WaterPayResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "UniqueID: " + WaterPayResp.UniqueID.ToString() + "\r\n";
                    }
                }
                else
                {
                    sb = sb + "Error: " + WaterPayResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            wServiceWater.Close();
            return result;
        }
    }
}