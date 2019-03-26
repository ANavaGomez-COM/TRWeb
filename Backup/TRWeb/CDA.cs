//Class: EventDA
//Date: 8/15/2014
//Programmer: Jeff Moyer
//Description:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TRWeb.CashieringService;
using System.Collections;

namespace TRWeb
{
    public class CDA
    {
        private String sb = String.Empty;
        public ArrayList result;
        private CashieringServiceClient wServiceCDA;
        private CDACreateRequest CDAPayReq = new CDACreateRequest();
        private CreateDWRecordResponse CDAPayResp = new CreateDWRecordResponse();
        private CDABalanceRequest CDABalReq = new CDABalanceRequest();
        private CDABalanceResponse CDABalResp = new CDABalanceResponse();
        private long portalId = 0;
        private String portalKey = String.Empty;
        private String xClientID;
        private String xClientName;
        private String xUnitID;
        private String xAddress;
        private String xBalanceDue;

        public String ClientID 
        {
            get { return xClientID; }
            set { xClientID = value; }
        }

        public String ClientName        
        {
            get { return xClientName; }
            set { xClientName = value; }
        }

        public String UnitID
        {
            get { return xUnitID; }
            set { xUnitID = value; }
        }

        public String Address
        {
            get { return xAddress; }
            set { xAddress = value; }
        }

        public String BalanceDue
        {
            get { return xBalanceDue; }
            set { xBalanceDue = value; }
        }

        public String buildCDABalance(String unitID, String clientID, String clientName, String clientAddress) 
        {
            CDABalReq = new CDABalanceRequest();
            sb = String.Empty;

            portalId = 6;
            portalKey = "ipoKpP8aSbaJEcE";
                        
            sb = "You searched for:\r\n\r\n";

            CDABalReq.UnitID = unitID;
            CDABalReq.ClientID = clientID;
            CDABalReq.ClientName = clientName;
            CDABalReq.Address = clientAddress;
            CDABalReq.SecurityPortalId = portalId;
            CDABalReq.SecurityKey = portalKey;
            sb = sb + "    UnitID: " + CDABalReq.UnitID.ToString() + Environment.NewLine;
            sb = sb + "    ClientID: " + CDABalReq.ClientID.ToString() + "\r\n";
            sb = sb + "    ClientName: " + CDABalReq.ClientName.ToString() + "\r\n";
            sb = sb + "    Address: " + CDABalReq.Address.ToString() + "\r\n\r\n";

            processResults("CDA Balance", portalId, portalKey);

            return sb;
        }

        public String buildCDAPayVoid(string type, String unitID, String clientID, String clientName, Decimal paymentAmount)
        {
            CDAPayReq = new CDACreateRequest();
            sb = "";

            portalId = 19;
            portalKey = "qGs3%X@QJHPXE5n";

            sb = sb + "The Request contains:\r\n";

            CDAPayReq.UnitID = unitID;
            CDAPayReq.ClientID = clientID;
            CDAPayReq.ClientName = clientName;
            CDAPayReq.PaymentAmount = paymentAmount;
            CDAPayReq.PaymentDate = DateTime.Now;
            //CDAPayReq.PaymentMethod
            //CDAPayReq.Deposit
            //CDAPayReq.ReceiptNumber
            CDAPayReq.PaymentID = 1;
            CDAPayReq.BatchID = 1;
            CDAPayReq.SecurityPortalId = portalId;
            CDAPayReq.SecurityKey = portalKey;
            sb = sb + "    UnitID: " + CDAPayReq.UnitID.ToString() + "\r\n";
            sb = sb + "    ClientID: " + CDAPayReq.ClientID.ToString() + "\r\n";
            sb = sb + "    ClientName: " + CDAPayReq.ClientName.ToString() + "\r\n";
            sb = sb + "    PaymentAmount: " + CDAPayReq.PaymentAmount.ToString() + "\r\n";
            sb = sb + "    PaymentDate: " + CDAPayReq.PaymentDate.ToString() + "\r\n";

            processResults(type, portalId, portalKey);

            return sb;
        }

        public ArrayList processResults(string type, long portalId, String portalKey)
        {
            wServiceCDA = new CashieringServiceClient();
            wServiceCDA.Open();

            result = new ArrayList();

            if (type == "CDA Balance")
            {
                CDABalResp = new CDABalanceResponse();
                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServiceCDA.GetCDABalance(CDABalReq);
                CDABalResp = ((CDABalanceResponse)arrGetBal.GetValue(0));
                sb = sb + CDABalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (CDABalResp.Status == "Success")
                {
                    if (CDABalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + CDABalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment.";
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            CDA cda = new CDA();
                            CDABalResp = new CDABalanceResponse();
                            CDABalResp = ((CDABalanceResponse)arrGetBal.GetValue(r));

                            cda.ClientID = CDABalResp.ClientID;
                            cda.ClientName = CDABalResp.ClientName;
                            cda.UnitID = CDABalResp.UnitID;
                            cda.Address = CDABalResp.Address;
                            cda.BalanceDue = String.Format("{0:C}", CDABalResp.BalanceDue);
                            result.Add(cda);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + CDABalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else
            {
                CDAPayResp = new CreateDWRecordResponse();
                sb = sb + "\r\nThe result from the " + type + "\r\n";
                if (type == "Make Payment") 
                {
                    CDAPayResp = wServiceCDA.ApplyCDAPayment(CDAPayReq);
                } 
                else if (type == "Void Payment") 
                { 
                    CDAPayResp = wServiceCDA.VoidCDAPayment(CDAPayReq);
                }
                sb = sb + "Status: " + CDAPayResp.Status.ToString() + "\r\n";
                if (CDAPayResp.Status == "Success")
                {
                    if (CDAPayResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + CDAPayResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "UniqueID: " + CDAPayResp.UniqueID.ToString() + "\r\n";
                    }
                }
                else
                {
                    sb = sb + "Error: " + CDAPayResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            wServiceCDA.Close();
            return result;
        }
    }
}