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
using System.Data;

namespace TRWeb
{
    public class WSPortal
    {
        private string sb = "";
        public ArrayList result;
        private CashieringServiceClient wServiceAdmin;
        private NewPortalRequest newPortalReq = new NewPortalRequest();
        private UpdatePortalRequest updatePortalReq = new UpdatePortalRequest();
        private NewKeyRequest newKeyReq = new NewKeyRequest();
        private PortalResponse portalResp = new PortalResponse();
        private UpdateResponse updateResp = new UpdateResponse();
        private int AdminID = 0;
        private string AdminKey = "92jRL6iKpcC4tun";
        private string xOwnerEmail;
        private string xOwnerName;
        private string xPortalID;
        private string xActive;
        private string[] xMethods;

        public BizData xData;

        public string OwnerName
        {
            get { return xOwnerName; }
            set { xOwnerName = value; }
        }

        public string OwnerEmail 
        {
            get { return xOwnerEmail; }
            set { xOwnerEmail = value; }
        }

        public string PortalID
        {
            get { return xPortalID; }
            set { xPortalID = value; }
        }

        public string Active
        {
            get { return xActive; }
            set { xActive = value; }
        }

        public string[] Methods
        {
            get { return xMethods; }
            set { xMethods = value; }
        }


        public ArrayList getPortalInformation(int portalId)
        {
            xData = new BizData();
            DataTable portalDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            if (portalId == 0)
            {
                sqlStr = "exec GetAllPortalInformation";
            }
            else
            {
                sqlStr = "exec GetPortalInformation " + portalId.ToString();
            }

            portalDT = xData.getData(sqlStr);

            foreach (DataRow row in portalDT.Rows)
            {
                WSPortal admin = new WSPortal();
                foreach (DataColumn col in portalDT.Columns)
                {
                    if (col.ColumnName.Equals("portalId"))
                    {
                        admin.PortalID = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("ownerName"))
                    {
                        admin.OwnerName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("ownerEmail"))
                    {
                        admin.OwnerEmail = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("active"))
                    {
                        admin.Active = row[col].ToString();
                    }
                }
                result.Add(admin);
            }

            return result;
        }

        public ArrayList getPortalMethods(int portalId)
        {
            xData = new BizData();
            DataTable portalDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            sqlStr = "exec GetPortalMethodAccess " + portalId.ToString();

            portalDT = xData.getData(sqlStr);

            foreach (DataRow row in portalDT.Rows)
            {
                WSPortal admin = new WSPortal();
                foreach (DataColumn col in portalDT.Columns)
                {
                    result.Add(row[col].ToString());
                }
            }

            return result;
        }

        public ArrayList getAvailablePortalMethods(int portalId)
        {
            xData = new BizData();
            DataTable portalDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            sqlStr = "exec GetAvailablePortalMethodAccess " + portalId.ToString();

            portalDT = xData.getData(sqlStr);

            foreach (DataRow row in portalDT.Rows)
            {
                WSPortal admin = new WSPortal();
                foreach (DataColumn col in portalDT.Columns)
                {
                    result.Add(row[col].ToString());
                }
            }

            return result;
        }

        public string createNewPortal(string ownerName, string ownerEmail, string[] methods) 
        {
            newPortalReq = new NewPortalRequest();
            sb = "";

            sb = "You are requesting a new portal:\r\n\r\n";

            newPortalReq.AdminKey = AdminKey;
            newPortalReq.OwnerName = ownerName;
            newPortalReq.OwnerEmail = ownerEmail;
            newPortalReq.MethodNames = methods;
            //newPortalReq.
            sb = sb + "    Owner Name: " + newPortalReq.OwnerName.ToString() + Environment.NewLine;
            sb = sb + "    Owner Email: " + newPortalReq.OwnerEmail.ToString() + Environment.NewLine + Environment.NewLine;
            sb = sb + "With access to the following methods:" + Environment.NewLine + Environment.NewLine;

            foreach (string method in newPortalReq.MethodNames) 
            {
                sb = sb + "    " + method + Environment.NewLine;
            }

            sb = sb + Environment.NewLine;

            processResults("New Portal", AdminID, AdminKey);

            return sb;
        }

        public string updatePortal(string ownerName, string ownerEmail, bool active, int portalID, string[] methods)
        {
            updatePortalReq = new UpdatePortalRequest();
            sb = "";

            sb = "You are requesting an update to a portal:\r\n\r\n";
            
            updatePortalReq.AdminKey = AdminKey;
            updatePortalReq.PortalID = portalID;
            updatePortalReq.Active = active;
            updatePortalReq.OwnerName = ownerName;
            updatePortalReq.OwnerEmail = ownerEmail;
            updatePortalReq.MethodNames = methods;
            sb = sb + "    Portal ID: " + updatePortalReq.PortalID.ToString() + Environment.NewLine;
            sb = sb + "    Active: " + updatePortalReq.Active.ToString() + Environment.NewLine;
            sb = sb + "    Owner Name: " + updatePortalReq.OwnerName.ToString() + Environment.NewLine;
            sb = sb + "    Owner Email: " + updatePortalReq.OwnerEmail.ToString() + Environment.NewLine + Environment.NewLine;
            sb = sb + "With access to the following methods:" + Environment.NewLine + Environment.NewLine;

            foreach (string method in updatePortalReq.MethodNames)
            {
                sb = sb + "    " + method + Environment.NewLine;
            }

            processResults("Update Portal", AdminID, AdminKey);

            return sb;
        }

        public string createNewKey(int portalID)
        {
            newKeyReq = new NewKeyRequest();
            sb = "";

            sb = sb + "The New Key Request contains:\r\n";

            newKeyReq.AdminKey = AdminKey;
            newKeyReq.PortalId = portalID;
            sb = sb + "    Portal ID: " + newKeyReq.PortalId.ToString() + "\r\n";

            processResults("New Key", AdminID, AdminKey);

            return sb;
        }

        public ArrayList processResults(string type, long portalId, string portalKey)
        {
            wServiceAdmin = new CashieringServiceClient();
            wServiceAdmin.Open();

            result = new ArrayList();

            sb = sb + "Your results were returned with a status of ";

            if (type == "New Portal")
            {
                portalResp = new PortalResponse();
                portalResp = wServiceAdmin.CreateNewPortal(newPortalReq);
            }
            else if (type == "New Key")
            {
                portalResp = new PortalResponse();
                portalResp = wServiceAdmin.RequestNewKey(newKeyReq);
            }

            if (type == "New Portal" || type == "New Key")
            {
                sb = sb + portalResp.Status.ToString().ToLower() + ".\r\n\r\n";
                if (portalResp.Status == "Success")
                {
                    if (portalResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + portalResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "Your new portal key for portal id(" + portalResp.PortalId.ToString() + ") is: " + 
                            Environment.NewLine + portalResp.SecurityKey + Environment.NewLine + "An email will also be sent " + 
                            "with the new key.";
                    }
                }
                else
                {
                    sb = sb + "Error: " + portalResp.ErrorMessage.ToString() + "\r\n";
                }
            }

            if (type == "Update Portal")
            {
                updateResp = new UpdateResponse();
                updateResp = wServiceAdmin.UpdatePortal(updatePortalReq);
                
                sb = sb + updateResp.Status.ToString().ToLower() + ".\r\n\r\n";
                
                if (updateResp.Status == "Success")
                {
                    if (updateResp.ErrorMessage == "No Records Found")
                    {
                        sb = sb + "Error: " + updateResp.ErrorMessage.ToString() + "\r\n";
                    }
                    else
                    {
                        sb = sb + "Update to portal id(" + updateResp.PortalId.ToString() + "):" + Environment.NewLine;
                        sb = sb + "Owner Name: " + updateResp.OwnerName.ToString() + Environment.NewLine;
                        sb = sb + "Owner Email: " + updateResp.OwnerEmail.ToString() + Environment.NewLine;
                        sb = sb + "Active: " + updateResp.Active.ToString() + Environment.NewLine + Environment.NewLine;
                        sb = sb + "Methods: " + Environment.NewLine + Environment.NewLine;
                        sb = sb + "An email will also be sent with the updates.";
                        
                        foreach (string method in updateResp.MethodNames)
                        {
                            sb = sb + method.ToString() + Environment.NewLine;
                        }

                    }
                }
            }
            wServiceAdmin.Close();
            return result;
        }
    }
}