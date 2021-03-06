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
    public class Notification
    {
        private string sb = String.Empty;
        public ArrayList result;
        private int xEmailID;
        private string xEmail;
        private Boolean xOnlineEmail;
        private Boolean xRemittanceEmail;
        private Boolean xActive;
        private string xSourceType;

        public BizData xData;

        public int EmailID
        {
            get { return xEmailID; }
            set { xEmailID = value; }
        }

        public string Email
        {
            get { return xEmail; }
            set { xEmail = value; }
        }

        public Boolean OnlineEmail
        {
            get { return xOnlineEmail; }
            set { xOnlineEmail = value; }
        }

        public Boolean RemittanceEmail
        {
            get { return xRemittanceEmail; }
            set { xRemittanceEmail = value; }
        }

        public Boolean Active
        {
            get { return xActive; }
            set { xActive = value; }
        }

        public string SourceType
        {
            get { return xSourceType; }
            set { xSourceType = value; }
        }

        public ArrayList getNotificationEmailBySourceType(string source)
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            sqlStr = "exec GetEmailBySourceType '" + source.ToUpper() + "'";

            sourceNotificationDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceNotificationDT.Rows)
            {
                Notification notif = new Notification();
                foreach (DataColumn col in sourceNotificationDT.Columns)
                {
                    if (col.ColumnName.Equals("ID"))
                    {
                        notif.EmailID = Convert.ToInt32(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("EmailAddress"))
                    {
                        notif.Email = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("ReceiveOnlineEmail"))
                    {
                        notif.OnlineEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("ReceiveRemittanceEmail"))
                    {
                        notif.RemittanceEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        notif.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("SourceType"))
                    {
                        notif.SourceType = row[col].ToString();
                    }
                }
                result.Add(notif);
            }

            return result;
        }

        public ArrayList getNotificationInformationByEmail(string email, string source)
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            sqlStr = "exec GetNotificationInformationByEmail '" + email + "', '" + source + "'";

            sourceNotificationDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceNotificationDT.Rows)
            {
                Notification notif = new Notification();
                foreach (DataColumn col in sourceNotificationDT.Columns)
                {
                    if (col.ColumnName.Equals("ID"))
                    {
                        notif.EmailID = Convert.ToInt32(row[col].ToString());
                    } 
                    if (col.ColumnName.Equals("EmailAddress"))
                    {
                        notif.Email = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("ReceiveOnlineEmail"))
                    {
                        notif.OnlineEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("ReceiveRemittanceEmail"))
                    {
                        notif.RemittanceEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        notif.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("SourceType"))
                    {
                        notif.SourceType = row[col].ToString();
                    }
                }
                result.Add(notif);
            }

            return result;
        }

        public ArrayList getNotificationInformationAll()
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            string sqlStr = "";
            result = new ArrayList();

            sqlStr = "exec GetNotificationInformationAll";

            sourceNotificationDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceNotificationDT.Rows)
            {
                Notification notif = new Notification();
                foreach (DataColumn col in sourceNotificationDT.Columns)
                {
                    if (col.ColumnName.Equals("ID"))
                    {
                        notif.EmailID = Convert.ToInt32(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("EmailAddress"))
                    {
                        notif.Email = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("ReceiveOnlineEmail"))
                    {
                        notif.OnlineEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("ReceiveRemittanceEmail"))
                    {
                        notif.RemittanceEmail = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        notif.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("SourceType"))
                    {
                        notif.SourceType = row[col].ToString();
                    }
                }
                result.Add(notif);
            }

            return result;
        }

        public int getActiveNotificationInformation(string source)
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            string sqlStr = "";
            int countResult = 1;

            sqlStr = "SELECT Count(*) AS 'ActiveCount' FROM EmailNotification E WHERE E.SourceType = '" + source.ToUpper() + "' AND E.Active = 1";

            sourceNotificationDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceNotificationDT.Rows)
            {
                foreach (DataColumn col in sourceNotificationDT.Columns)
                {
                    if (col.ColumnName.Equals("ActiveCount"))
                    {
                        countResult = Convert.ToInt32(row[col].ToString());
                    }
                }
            }

            return countResult;
        }

        public string updateNotificationInformation(int emailID, string email, Boolean online, Boolean remit
            , Boolean active)
        {
            xData = new BizData();
            string sqlStr = "";
            string result = "";

            sqlStr = "exec UpdateEmailNotificationInformation '" + emailID + "', '" + email + "', '" + online +
              "', '" + remit + "', '" + active + "'";

            result = xData.updateData(sqlStr);

            return result;
        }

        public string insertNotificationInformation(string sourceType, string email, Boolean online
            , Boolean remit, Boolean active)
        {
            xData = new BizData();
            string sqlStr = "";
            string result = "";

            sqlStr = "exec InsertEmailNotificationInformation '" + sourceType.ToUpper() + "', '" + email + "', '" +
                online + "', '" + remit + "', '" + active + "'";

            result = xData.insertData(sqlStr);

            return result;
        }
    }
}