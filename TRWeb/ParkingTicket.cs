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

/* change log:
        
    italn 20180905 - added wServicePTicket.GetTicketBalance(PTicketBalReq); this line was missing so when a parking ticket was entered, nothing was being returned. 
     
*/
namespace TRWeb
{
    public class ParkingTicket
    {
        private string sb = string.Empty;
        public ArrayList result;
        private CashieringServiceClient wServicePTicket;
        private ParkingTicketCreateRequest PTicketPayReq = new ParkingTicketCreateRequest();
        private CreateDWRecordResponse PTicketPayResp = new CreateDWRecordResponse();
        private ParkingTicketBalanceRequest PTicketBalReq = new ParkingTicketBalanceRequest();
        private ParkingTicketListRequest PTicketListReq = new ParkingTicketListRequest();
        private ParkingTicketResponse PTicketBalResp = new ParkingTicketResponse();
        private long portalId = 0;
        private string portalKey = "";
        private string xTicketNumber;
        private string xLicensePlate;
        private string xLicensePlateState;
        private string xLicensePlateType;
        private string xViolationDate;
        private string xBalanceDue;

        public string TicketNumber
        {
            get { return xTicketNumber; }
            set { xTicketNumber = value; }
        }

        public string LicensePlate
        {
            get { return xLicensePlate; }
            set { xLicensePlate = value; }
        }

        public string LicensePlateState
        {
            get { return xLicensePlateState; }
            set { xLicensePlateState = value; }
        }

        public string LicensePlateType
        {
            get { return xLicensePlateType; }
            set { xLicensePlateType = value; }
        }

        public string ViolationDate
        {
            get { return xViolationDate; }
            set { xViolationDate = value; }
        }

        public string BalanceDue
        {
            get { return xBalanceDue; }
            set { xBalanceDue = value; }
        }

        public string buildPTicketBalance(string ticketNumber)
        {
            PTicketBalReq = new ParkingTicketBalanceRequest();
            sb = string.Empty;

            portalId = 10;
            portalKey = "SlmimFGj@MBxAei";

            sb = "You searched for:\r\n\r\n";

            PTicketBalReq.TicketNumber = ticketNumber;
            PTicketBalReq.SecurityPortalId = portalId;
            PTicketBalReq.SecurityKey = portalKey;            
            sb = sb + "    Ticket Number: " + PTicketBalReq.TicketNumber.ToString() + "\r\n\r\n";

            processResults("Ticket Balance", portalId, portalKey);

            return sb;
        }

        public string buildPTicketList(string licensePlate, string licensePlateState, string licensePlateType)
        {
            PTicketListReq = new ParkingTicketListRequest();
            sb = "";

            portalId = 10;
            portalKey = "SlmimFGj@MBxAei";

            sb = "You searched for:\r\n\r\n";

            PTicketListReq.LicensePlate = licensePlate;
            PTicketListReq.LicensePlateState = licensePlateState;
            PTicketListReq.LicensePlateType = licensePlateType;
            PTicketListReq.SecurityPortalId = portalId;
            PTicketListReq.SecurityKey = portalKey;
            sb = sb + "    License Plate: " + PTicketListReq.LicensePlate.ToString() + "\r\n";
            sb = sb + "    License Plate State: " + PTicketListReq.LicensePlateState.ToString() + "\r\n";
            sb = sb + "    License Plate Type: " + PTicketListReq.LicensePlateType.ToString() + "\r\n\r\n";

            processResults("Ticket List", portalId, portalKey);

            return sb;
        }

        public string buildParkingTicketPayVoid(string type, string ticketNumber, string licnesePlate, string licensePlateState, 
               string licensePlateType, DateTime violationDate, Decimal paymentAmount)
        {
            PTicketPayReq = new ParkingTicketCreateRequest();
            sb = "";

            portalId = 11;
            portalKey = "68n9Nx946XADZBN";

            sb = sb + "The Request contains:\r\n";

            PTicketPayReq.TicketNumber = ticketNumber;
            PTicketPayReq.LicensePlate = licnesePlate;
            PTicketPayReq.LicensePlateState = licensePlateState;
            PTicketPayReq.LicensePlateType = licensePlateType;
            PTicketPayReq.ViolationDate = violationDate;
            PTicketPayReq.PaymentAmount = paymentAmount;
            PTicketPayReq.PaymentDate = DateTime.Now;
            PTicketPayReq.PaymentID = 1;
            PTicketPayReq.BatchID = 1;
            PTicketPayReq.SecurityPortalId = portalId;
            PTicketPayReq.SecurityKey = portalKey;
            sb = sb + "    Ticket Number: " + PTicketPayReq.TicketNumber.ToString() + "\r\n";
            sb = sb + "    Licnese Plate: " + PTicketPayReq.LicensePlate.ToString() + "\r\n";
            sb = sb + "    Licnese Plate State: " + PTicketPayReq.LicensePlateState.ToString() + "\r\n";
            sb = sb + "    Licnese Plate Type: " + PTicketPayReq.LicensePlateType.ToString() + "\r\n";
            sb = sb + "    Violation Date: " + PTicketPayReq.ViolationDate.ToString() + "\r\n";
            sb = sb + "    PaymentAmount: " + PTicketPayReq.PaymentAmount.ToString() + "\r\n";
            sb = sb + "    PaymentDate: " + PTicketPayReq.PaymentDate.ToString() + "\r\n";

            processResults(type, portalId, portalKey);

            return sb;
        }

        public ArrayList processResults(string type, long portalId, string portalKey)
        {
            wServicePTicket = new CashieringServiceClient();
            wServicePTicket.Open();

            result = new ArrayList();

            if (type == "Ticket Balance")
            {
                ParkingTicket pt = new ParkingTicket();
                PTicketBalResp = new ParkingTicketResponse();
                sb = sb + "Your search results were returned with a status of ";
                // italn 20180905 - see comment in change log above.
                PTicketBalResp = wServicePTicket.GetTicketBalance(PTicketBalReq);
                sb = sb + PTicketBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                pt.TicketNumber = PTicketBalResp.TicketNumber;
                pt.LicensePlate = PTicketBalResp.LicensePlate;
                pt.LicensePlateState = PTicketBalResp.LicensePlateState;
                pt.LicensePlateType = PTicketBalResp.LicensePlateType;
                pt.ViolationDate = PTicketBalResp.ViolationDate;
                pt.BalanceDue = string.Format("{0:C}", PTicketBalResp.BalanceDue);
                result.Add(pt);
            }
            else if (type == "Ticket List")
            {
                PTicketBalResp = new ParkingTicketResponse();
                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServicePTicket.GetParkingTicketList(PTicketListReq);
                PTicketBalResp = ((ParkingTicketResponse)arrGetBal.GetValue(0));
                sb = sb + PTicketBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (PTicketBalResp.Status == "Success")
                {
                    if (PTicketBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + PTicketBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment.";
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            ParkingTicket pt = new ParkingTicket();
                            PTicketBalResp = new ParkingTicketResponse();
                            PTicketBalResp = ((ParkingTicketResponse)arrGetBal.GetValue(r));

                            pt.TicketNumber = PTicketBalResp.TicketNumber;
                            pt.LicensePlate = PTicketBalResp.LicensePlate;
                            pt.LicensePlateState = PTicketBalResp.LicensePlateState;
                            pt.LicensePlateType = PTicketBalResp.LicensePlateType;
                            pt.ViolationDate = PTicketBalResp.ViolationDate;
                            pt.BalanceDue = string.Format("{0:C}", PTicketBalResp.BalanceDue);
                            result.Add(pt);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + PTicketBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else
            {
                PTicketPayResp = new CreateDWRecordResponse();
                sb = sb + "\r\nThe result from the " + type + "\r\n";
                if (type == "Make Payment")
                {
                    PTicketPayResp = wServicePTicket.ApplyTicketPayment(PTicketPayReq);
                }
                else if (type == "Void Payment")
                {
                    PTicketPayResp = wServicePTicket.VoidTicketPayment(PTicketPayReq);
                }
                sb = sb + "Status: " + PTicketPayResp.Status.ToString() + "\r\n";
                if (PTicketPayResp.Status == "Success")
                {
                    if (PTicketPayResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + PTicketPayResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "UniqueID: " + PTicketPayResp.UniqueID.ToString() + "\r\n";
                    }
                }
                else
                {
                    sb = sb + "Error: " + PTicketPayResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            wServicePTicket.Close();
            return result;
        }
    }
}