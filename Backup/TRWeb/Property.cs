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
    public class Property
    {
        private String sb = String.Empty;
        public ArrayList result;
        private CashieringServiceClient wServiceProp;

        private PersonalPropertyTaxCreateRequest PPropertyPayReq = new PersonalPropertyTaxCreateRequest();
        private PropertyTaxCreateRequest RPropertyPayReq = new PropertyTaxCreateRequest();
        private CreateDWRecordResponse payVoidResp = new CreateDWRecordResponse();

        private PersonalPropertyTaxResponse PPropertyBalResp = new PersonalPropertyTaxResponse();
        private RealPropertyTaxResponse RPropertyBalResp = new RealPropertyTaxResponse();

        private PersonalPropertyByAccountRequest PPropertyAcctReq = new PersonalPropertyByAccountRequest();
        private RealPropertyByParcelRequest RPropertyParcelReq = new RealPropertyByParcelRequest();
        private PersonalPropertyByOwnerNameRequest PPropertyOwnerReq = new PersonalPropertyByOwnerNameRequest();
        private RealPropertyByLastNameRequest RPropertyLastReq = new RealPropertyByLastNameRequest();
        private PropertyTaxAddressRequest PPropertyAddressReq = new PropertyTaxAddressRequest();
        private PropertyTaxAddressRequest RPropertyAddressReq = new PropertyTaxAddressRequest();
        
        private long rPortalId = 18;
        private String rPortalKey = "$Uw9q2W%IG3Ewpa";
        private long pPortalId = 13;
        private String pPortalKey = "d8oi8ari8RAPsa7";
        private String xNumber;
        private String xName1;
        private String xName2;
        private Decimal xHouseNumber;
        private String xStreetDirection;
        private String xStreetName;
        private String xStreetType;
        private String xUnit;
        private String xCity;
        private String xZip;
        private String xZip1;
        private String xAddress;
        private String xBalanceDue;
        private String xFullAmount;

        public String Number
        {
            get { return xNumber; }
            set { xNumber = value; }
        }

        public String Name1        
        {
            get { return xName1; }
            set { xName1 = value; }
        }

        public String Name2        
        {
            get { return xName2; }
            set { xName2 = value; }
        }

        public Decimal HouseNumber
        {
            get { return xHouseNumber; }
            set {xHouseNumber = value; }
        }

        public String StreetDirection
        {
            get { return xStreetDirection; }
            set { xStreetDirection = value; }
        }

        public String StreetName
        {
            get { return xStreetName; }
            set { xStreetName = value; }
        }

        public String StreetType
        {
            get { return xStreetType; }
            set { xStreetType = value; }
        }

        public String Unit
        {
            get { return xUnit; }
            set { xUnit = value; }
        }

        public String City
        {
            get { return xCity; }
            set { xCity = value; }
        }

        public String Zip
        {
            get { return xZip; }
            set { xZip = value; }
        }

        public String Zip1
        {
            get { return xZip1; }
            set { xZip1 = value; }
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

        public String FullAmount
        {
            get { return xFullAmount; }
            set { xFullAmount = value; }
        }

        public String getPropertyByNumber(String type, String number) 
        {

            sb = String.Empty;
            sb = "You searched for:\r\n\r\n";
            
            if (type == "Personal")
            {
                PPropertyAcctReq = new PersonalPropertyByAccountRequest();
                PPropertyAcctReq.AccountNumber = number;
                PPropertyAcctReq.SecurityPortalId = pPortalId;
                PPropertyAcctReq.SecurityKey = pPortalKey;
                sb = sb + "    Account Number: " + PPropertyAcctReq.AccountNumber.ToString() + Environment.NewLine + 
                    Environment.NewLine;
                processResults("pNumber Balance", pPortalId, pPortalKey);
            }
            else
            {
                RPropertyParcelReq = new RealPropertyByParcelRequest();
                RPropertyParcelReq.ParcelNumber = number;
                RPropertyParcelReq.SecurityPortalId = rPortalId;
                RPropertyParcelReq.SecurityKey = rPortalKey;
                sb = sb + "    Parcel Number: " + RPropertyParcelReq.ParcelNumber.ToString() + Environment.NewLine + 
                    Environment.NewLine;
                processResults("rNumber Balance", rPortalId, rPortalKey);
            }

            return sb;
        }

        public String getPropertyByName(String type, String name)
        {

            sb = String.Empty;
            sb = "You searched for:\r\n\r\n";

            if (type == "Personal")
            {
                PPropertyOwnerReq = new PersonalPropertyByOwnerNameRequest();
                PPropertyOwnerReq.OwnerName = name;
                PPropertyOwnerReq.SecurityPortalId = pPortalId;
                PPropertyOwnerReq.SecurityKey = pPortalKey;
                sb = sb + "    Owner: " + PPropertyOwnerReq.OwnerName.ToString() + Environment.NewLine +
                    Environment.NewLine;
                processResults("pName Balance", pPortalId, pPortalKey);
            }
            else
            {
                RPropertyLastReq = new RealPropertyByLastNameRequest();
                RPropertyLastReq.LastName = name;
                RPropertyLastReq.SecurityPortalId = rPortalId;
                RPropertyLastReq.SecurityKey = rPortalKey;
                sb = sb + "    Last Name: " + RPropertyLastReq.LastName.ToString() + Environment.NewLine +
                    Environment.NewLine;
                processResults("rName Balance", rPortalId, rPortalKey);
            }

            return sb;
        }

        public String getPropertyByAddress(String type, String hseNum, String direction, String stName, String stType, String unit)
        {

            sb = String.Empty;
            sb = "You searched for:\r\n\r\n";

            if (type == "Personal")
            {
                PPropertyAddressReq = new PropertyTaxAddressRequest();
                PPropertyAddressReq.HouseNumber = hseNum;
                PPropertyAddressReq.StreetDirection = direction;
                PPropertyAddressReq.StreetName = stName;
                PPropertyAddressReq.StreetType = stType;
                PPropertyAddressReq.Unit = unit;
                PPropertyAddressReq.SecurityPortalId = pPortalId;
                PPropertyAddressReq.SecurityKey = pPortalKey;
                sb = sb + "    Number: " + PPropertyAddressReq.HouseNumber.ToString() + Environment.NewLine;
                sb = sb + "    Direction: " + PPropertyAddressReq.StreetDirection.ToString() + Environment.NewLine;
                sb = sb + "    Name: " + PPropertyAddressReq.StreetName.ToString() + Environment.NewLine;
                sb = sb + "    Type: " + PPropertyAddressReq.StreetType.ToString() + Environment.NewLine;
                sb = sb + "    Unit: " + PPropertyAddressReq.Unit.ToString() + Environment.NewLine + Environment.NewLine;
                processResults("pAddress Balance", pPortalId, pPortalKey);
            }
            else
            {
                RPropertyAddressReq = new PropertyTaxAddressRequest();
                RPropertyAddressReq.HouseNumber = hseNum;
                RPropertyAddressReq.StreetDirection = direction;
                RPropertyAddressReq.StreetName = stName;
                RPropertyAddressReq.StreetType = stType;
                RPropertyAddressReq.Unit = unit;
                RPropertyAddressReq.SecurityPortalId = rPortalId;
                RPropertyAddressReq.SecurityKey = rPortalKey;
                sb = sb + "    Number: " + RPropertyAddressReq.HouseNumber.ToString() + Environment.NewLine;
                sb = sb + "    Direction: " + RPropertyAddressReq.StreetDirection.ToString() + Environment.NewLine;
                sb = sb + "    Name: " + RPropertyAddressReq.StreetName.ToString() + Environment.NewLine;
                sb = sb + "    Type: " + RPropertyAddressReq.StreetType.ToString() + Environment.NewLine;
                sb = sb + "    Unit: " + RPropertyAddressReq.Unit.ToString() + Environment.NewLine + Environment.NewLine;
                processResults("rAddress Balance", rPortalId, rPortalKey);
            }

            return sb;
        }

        public String buildPersonalPropertyPayVoid(string type, String number, Decimal paymentAmount)
        {
            PPropertyPayReq = new PersonalPropertyTaxCreateRequest();
            sb = "";

            long portalId = 15;
            String portalKey = "bYaSIpWfF8lnDhX";

            sb = sb + "The Request contains:\r\n";

            PPropertyPayReq.AccountNumber = number;
            PPropertyPayReq.PaymentAmount = paymentAmount;
            PPropertyPayReq.PaymentDate = DateTime.Now;
            PPropertyPayReq.PaymentID = 1;
            PPropertyPayReq.BatchID = 1;
            PPropertyPayReq.SecurityPortalId = portalId;
            PPropertyPayReq.SecurityKey = portalKey;
            sb = sb + "    Account: " + PPropertyPayReq.AccountNumber.ToString() + "\r\n";
            sb = sb + "    PaymentAmount: " + PPropertyPayReq.PaymentAmount.ToString() + "\r\n";
            sb = sb + "    PaymentDate: " + PPropertyPayReq.PaymentDate.ToString() + "\r\n";

            processResults(type, portalId, portalKey);

            return sb;
        }

        public String buildRealPropertyPayVoid(string type, String number, Decimal paymentAmount)
        {
            RPropertyPayReq = new PropertyTaxCreateRequest();
            sb = "";

            long portalId = 17;
            String portalKey = "@oISahtHZcVBCRc";

            sb = sb + "The Request contains:\r\n";

            RPropertyPayReq.ParcelNumber = number;
            RPropertyPayReq.PaymentAmount = paymentAmount;
            RPropertyPayReq.PaymentDate = DateTime.Now;
            RPropertyPayReq.PaymentID = 1;
            RPropertyPayReq.BatchID = 1;
            RPropertyPayReq.SecurityPortalId = portalId;
            RPropertyPayReq.SecurityKey = portalKey;
            sb = sb + "    Parcel: " + RPropertyPayReq.ParcelNumber.ToString() + "\r\n";
            sb = sb + "    PaymentAmount: " + RPropertyPayReq.PaymentAmount.ToString() + "\r\n";
            sb = sb + "    PaymentDate: " + RPropertyPayReq.PaymentDate.ToString() + "\r\n";

            processResults(type, portalId, portalKey);

            return sb;
        }

        public ArrayList processResults(string type, long portalId, String portalKey)
        {
            wServiceProp = new CashieringServiceClient();
            wServiceProp.Open();

            result = new ArrayList();

            if (type == "pNumber Balance")
            {
                PPropertyBalResp = new PersonalPropertyTaxResponse();
                sb = sb + "Your search results were returned with a status of ";

                PPropertyBalResp = wServiceProp.GetPersonalBalanceByAccountNo(PPropertyAcctReq);

                sb = sb + PPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                
                if (PPropertyBalResp.Status == "Success")
                {
                    if (PPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "The record returned is listed below. Click the appropriate button to make or void a payment." 
                            + Environment.NewLine;
                            
                        Property prop = new Property();

                        prop.Number = PPropertyBalResp.AccountNumber;
                        prop.Name1 = PPropertyBalResp.OwnerName1;
                        prop.Name2 = PPropertyBalResp.OwnerName2;
                        prop.HouseNumber = PPropertyBalResp.StreetNumber;
                        prop.StreetDirection = PPropertyBalResp.StreetDirection;
                        prop.StreetName = PPropertyBalResp.StreetName;
                        prop.StreetType = PPropertyBalResp.StreetType;
                        prop.Unit = PPropertyBalResp.Unit;
                        prop.City = PPropertyBalResp.City;
                        prop.Zip = PPropertyBalResp.Zip;
                        prop.Zip1 = PPropertyBalResp.Zip1;
                        prop.Address = PPropertyBalResp.Address;
                        prop.BalanceDue = String.Format("{0:C}", PPropertyBalResp.BalanceDue);
                        sb = sb + "    Account: " + prop.Number.ToString() + "\r\n";
                        sb = sb + "    Owner Name: " + prop.Name1.ToString() + "\r\n";
                        sb = sb + "    Owner Name 2: " + prop.Name2.ToString() + "\r\n";
                        sb = sb + "    Number: " + prop.HouseNumber.ToString() + "\r\n";
                        sb = sb + "    Direction: " + prop.StreetDirection.ToString() + "\r\n";
                        sb = sb + "    Street: " + prop.StreetName.ToString() + "\r\n";
                        sb = sb + "    Type: " + prop.StreetType.ToString() + "\r\n";
                        sb = sb + "    Unit: " + prop.Unit.ToString() + "\r\n";
                        sb = sb + "    City: " + prop.City.ToString() + "\r\n";
                        sb = sb + "    Zip: " + prop.Zip.ToString() + "\r\n";
                        sb = sb + "    Zip1: " + prop.Zip1.ToString() + "\r\n";
                        sb = sb + "    Address: " + prop.Address.ToString() + "\r\n";
                        sb = sb + "    Balance Due: " + prop.BalanceDue.ToString() + "\r\n";
                        result.Add(prop);
                    }
                }
                else
                {
                    sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "pName Balance")
            {
                PPropertyBalResp = new PersonalPropertyTaxResponse();
                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServiceProp.GetPersonalBalanceByOwnerName(PPropertyOwnerReq);
                PPropertyBalResp = ((PersonalPropertyTaxResponse)arrGetBal.GetValue(0));

                sb = sb + PPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (PPropertyBalResp.Status == "Success")
                {
                    if (PPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment." + Environment.NewLine;
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Property prop = new Property();
                            PPropertyBalResp = new PersonalPropertyTaxResponse();
                            PPropertyBalResp = ((PersonalPropertyTaxResponse)arrGetBal.GetValue(r));

                            prop.Number = PPropertyBalResp.AccountNumber;
                            prop.Name1 = PPropertyBalResp.OwnerName1;
                            prop.Name2 = PPropertyBalResp.OwnerName2;
                            prop.HouseNumber = PPropertyBalResp.StreetNumber;
                            prop.StreetDirection = PPropertyBalResp.StreetDirection;
                            prop.StreetName = PPropertyBalResp.StreetName;
                            prop.StreetType = PPropertyBalResp.StreetType;
                            prop.Unit = PPropertyBalResp.Unit;
                            prop.City = PPropertyBalResp.City;
                            prop.Zip = PPropertyBalResp.Zip;
                            prop.Zip1 = PPropertyBalResp.Zip1;
                            prop.Address = PPropertyBalResp.Address;
                            prop.BalanceDue = String.Format("{0:C}", PPropertyBalResp.BalanceDue);
                            result.Add(prop);
                        }                        
                    }
                }
                else
                {
                    sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "rNumber Balance")
            {
                RPropertyBalResp = new RealPropertyTaxResponse();
                sb = sb + "Your search results were returned with a status of ";

                RPropertyBalResp = wServiceProp.GetRealBalanceByParcelNo(RPropertyParcelReq);
                sb = sb + RPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (RPropertyBalResp.Status == "Success")
                {
                    if (RPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + RPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "The record returned is listed below. Click the appropriate button to make or void a payment."
                            + Environment.NewLine;

                        Property prop = new Property();

                        prop.Number = RPropertyBalResp.ParcelNumber;
                        prop.Name1 = RPropertyBalResp.OwnerName1;
                        prop.Name2 = RPropertyBalResp.OwnerName2;
                        prop.Address = RPropertyBalResp.Address;
                        prop.HouseNumber = RPropertyBalResp.FirstInstallment;
                        prop.StreetDirection = String.Format("{0:C}", RPropertyBalResp.SecondInstallment);
                        prop.StreetName = String.Format("{0:C}", RPropertyBalResp.ThirdInstallment);
                        prop.StreetType = String.Format("{0:C}", RPropertyBalResp.FourthInstallment);
                        sb = sb + "    Parcel: " + prop.Number.ToString() + "\r\n";
                        sb = sb + "    Owner Name: " + prop.Name1.ToString() + "\r\n";
                        sb = sb + "    Owner Name 2: " + prop.Name2.ToString() + "\r\n";
                        sb = sb + "    Address: " + prop.Address.ToString() + "\r\n";
                        sb = sb + "    First Installment: " + prop.HouseNumber.ToString("C") + "\r\n";
                        sb = sb + "    Second Installment: " + prop.StreetDirection.ToString() + "\r\n";
                        sb = sb + "    Third Installment: " + prop.StreetName.ToString() + "\r\n";
                        sb = sb + "    Fourth Installment: " + prop.StreetType.ToString() + "\r\n";  
                        result.Add(prop);
                    }
                }
                else
                {
                    sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "rName Balance")
            {
                RPropertyBalResp = new RealPropertyTaxResponse();
                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServiceProp.GetRealParcelByLastName(RPropertyLastReq);
                RPropertyBalResp = ((RealPropertyTaxResponse)arrGetBal.GetValue(0));

                sb = sb + RPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (RPropertyBalResp.Status == "Success")
                {
                    if (RPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + RPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment." + Environment.NewLine;
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Property prop = new Property();
                            RPropertyBalResp = new RealPropertyTaxResponse();
                            RPropertyBalResp = ((RealPropertyTaxResponse)arrGetBal.GetValue(r));

                            prop.Number = RPropertyBalResp.ParcelNumber;
                            prop.Name1 = RPropertyBalResp.OwnerName1;
                            prop.Name2 = RPropertyBalResp.OwnerName2;
                            prop.Address = RPropertyBalResp.Address;
                            prop.HouseNumber = RPropertyBalResp.FirstInstallment;
                            prop.StreetDirection = String.Format("{0:C}", RPropertyBalResp.SecondInstallment);
                            prop.StreetName = String.Format("{0:C}", RPropertyBalResp.ThirdInstallment);
                            prop.StreetType = String.Format("{0:C}", RPropertyBalResp.FourthInstallment);
                            prop.FullAmount = String.Format("{0:C}", RPropertyBalResp.FullAmount);
                            result.Add(prop);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + RPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "pAddress Balance")
            {
                PPropertyBalResp = new PersonalPropertyTaxResponse();

                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServiceProp.GetPersonalBalanceByAddress(PPropertyAddressReq);
                PPropertyBalResp = ((PersonalPropertyTaxResponse)arrGetBal.GetValue(0));

                sb = sb + PPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (PPropertyBalResp.Status == "Success")
                {
                    if (PPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment." + Environment.NewLine;
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Property prop = new Property();
                            PPropertyBalResp = new PersonalPropertyTaxResponse();
                            PPropertyBalResp = ((PersonalPropertyTaxResponse)arrGetBal.GetValue(r));

                            prop.Number = PPropertyBalResp.AccountNumber;
                            prop.Name1 = PPropertyBalResp.OwnerName1;
                            prop.Name2 = PPropertyBalResp.OwnerName2;
                            prop.HouseNumber = PPropertyBalResp.StreetNumber;
                            prop.StreetDirection = PPropertyBalResp.StreetDirection;
                            prop.StreetName = PPropertyBalResp.StreetName;
                            prop.StreetType = PPropertyBalResp.StreetType;
                            prop.Unit = PPropertyBalResp.Unit;
                            prop.City = PPropertyBalResp.City;
                            prop.Zip = PPropertyBalResp.Zip;
                            prop.Zip1 = PPropertyBalResp.Zip1;
                            prop.Address = PPropertyBalResp.Address;
                            prop.BalanceDue = String.Format("{0:C}", PPropertyBalResp.BalanceDue);
                            result.Add(prop);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + PPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "rAddress Balance")
            {
                RPropertyBalResp = new RealPropertyTaxResponse();
                sb = sb + "Your search results were returned with a status of ";

                Array arrGetBal = wServiceProp.GetRealBalanceByAddress(RPropertyAddressReq);
                RPropertyBalResp = ((RealPropertyTaxResponse)arrGetBal.GetValue(0));

                sb = sb + RPropertyBalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (RPropertyBalResp.Status == "Success")
                {
                    if (RPropertyBalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + RPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "All records returned are listed below.  Select a record to make or void a payment." + Environment.NewLine;
                        int r = 0;
                        for (r = 0; r < arrGetBal.Length; r++)
                        {
                            Property prop = new Property();
                            RPropertyBalResp = new RealPropertyTaxResponse();
                            RPropertyBalResp = ((RealPropertyTaxResponse)arrGetBal.GetValue(r));

                            prop.Number = RPropertyBalResp.ParcelNumber;
                            prop.Name1 = RPropertyBalResp.OwnerName1;
                            prop.Name2 = RPropertyBalResp.OwnerName2;
                            prop.Address = RPropertyBalResp.Address;
                            prop.HouseNumber = RPropertyBalResp.FirstInstallment;
                            prop.StreetDirection = String.Format("{0:C}", RPropertyBalResp.SecondInstallment);
                            prop.FullAmount = String.Format("{0:C}", RPropertyBalResp.FullAmount);
                            result.Add(prop);
                        }
                    }
                }
                else
                {
                    sb = sb + "Error: " + RPropertyBalResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else if (type == "Void a Personal Payment" || type == "Make a Personal Payment")
            {
                payVoidResp = new CreateDWRecordResponse();
                sb = sb + "\r\nThe result from the " + type + "\r\n";
                if (type == "Make a Personal Payment") 
                {
                    payVoidResp = wServiceProp.ApplyPersonalPropertyTaxPayment(PPropertyPayReq);
                    sb = sb + "Status: " + payVoidResp.Status.ToString() + "\r\n";
                } 
                else if (type == "Void a Personal Payment") 
                { 
                    payVoidResp = wServiceProp.VoidPersonalPropertyTaxPayment(PPropertyPayReq);
                    sb = sb + "Status: " + payVoidResp.Status.ToString() + "\r\n";
                }
                
                if (payVoidResp.Status == "Success")
                {
                    if (payVoidResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + payVoidResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "UniqueID: " + payVoidResp.UniqueID.ToString() + "\r\n";
                    }
                }
                else
                {
                    sb = sb + "Error: " + payVoidResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            else
            {
                payVoidResp = new CreateDWRecordResponse();
                sb = sb + "\r\nThe result from the " + type + "\r\n";
                if (type == "Make a Real Estate Payment")
                {
                    payVoidResp = wServiceProp.ApplyRealPropertyTaxPayment(RPropertyPayReq);
                    sb = sb + "Status: " + payVoidResp.Status.ToString() + "\r\n";
                }
                else if (type == "Void a Real Estate Payment")
                {
                    payVoidResp = wServiceProp.VoidRealPropertyTaxPayment(RPropertyPayReq);
                    sb = sb + "Status: " + payVoidResp.Status.ToString() + "\r\n";
                }

                if (payVoidResp.Status == "Success")
                {
                    if (payVoidResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + payVoidResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "UniqueID: " + payVoidResp.UniqueID.ToString() + "\r\n";
                    }
                }
                else
                {
                    sb = sb + "Error: " + payVoidResp.ErrorMessage.ToString() + "\r\n";
                }
            }
            wServiceProp.Close();
            return result;
        }
    }
}