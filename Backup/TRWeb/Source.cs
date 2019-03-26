//Class: EventDA
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
    public class Source
    {
        private String sb = String.Empty;
        public ArrayList result;
        private String xSource;
        private String xDescription;
        private String xBillerCode;
        private String xAttachmentName;

        public BizData xData;

        public String SourceType
        {
            get { return xSource; }
            set { xSource = value; }
        }

        public String Description 
        {
            get { return xDescription; }
            set { xDescription = value; }
        }

        public String BillerCode
        {
            get { return xBillerCode; }
            set { xBillerCode = value; }
        }

        public String AttachmentName
        {
            get { return xAttachmentName; }
            set { xAttachmentName = value; }
        }

        public ArrayList getSourceInformation(String callType, String search)
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            String sqlStr = String.Empty;
            result = new ArrayList();

            if (callType.Equals("Source Type"))
            {
                sqlStr = "exec GetSourceBySourceType '" + search.ToUpper() + "'";
            }
            else if (callType.Equals("Source Description"))
            {
                sqlStr = "exec GetSourceBySourceDescription '" + search + "'";
            }
            else if (callType.Equals("Notification Email Address"))
            {
                sqlStr = "exec GetSourceByNotifEmail '" + search + "'";
            }
            else if (callType.Equals("User Name"))
            {
                sqlStr = "exec GetSourceByUserName '" + search + "'";
            }
            else if (callType.Equals("User Email Address"))
            {
                sqlStr = "exec GetSourceByUserEmail '" + search + "'";
            }
            else if (callType.Equals("User Group"))
            {
                sqlStr = "exec GetSourceByGroupName '" + search + "'";
            }
            else
            {
                sqlStr = "exec GetSourceAll";
            }

            sourceNotificationDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceNotificationDT.Rows)
            {
                Source s = new Source();
                foreach (DataColumn col in sourceNotificationDT.Columns)
                {
                    if (col.ColumnName.Equals("SourceType"))
                    {
                        s.SourceType = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("SourceTypeDescription"))
                    {
                        s.Description = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("BillerProductCode"))
                    {
                        s.BillerCode = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("AttachmentName"))
                    {
                        s.AttachmentName = row[col].ToString();
                    }
                }
                result.Add(s);
            }

            return result;
        }

        public int getActiveNotificationInformation(String source)
        {
            xData = new BizData();
            DataTable sourceNotificationDT = new DataTable();
            String sqlStr = String.Empty;
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

        public ArrayList getSourceList()
        {
            xData = new BizData();
            DataTable sourceDT = new DataTable();
            String sqlStr = String.Empty;
            result = new ArrayList();

            sqlStr = "SELECT S.SourceType, S.SourceType + ' - ' + S.SourceTypeDescription AS 'Source'  FROM SourceType S ORDER BY S.SourceType";

            sourceDT = xData.getData(sqlStr);

            foreach (DataRow row in sourceDT.Rows)
            {
                Source s = new Source();
                foreach (DataColumn col in sourceDT.Columns)
                {
                    if (col.ColumnName.Equals("SourceType"))
                    {
                        s.SourceType = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("Source"))
                    {
                        s.Description = row[col].ToString();
                    }
                }
                result.Add(s);
            }

            return result;
        }

        public String updateSourceType(String source, String description, String attach, String billerCode)
        {
            xData = new BizData();
            String sqlStr = String.Empty;
            String result = String.Empty;

            sqlStr = "exec UpdateSourceInformation '" + source.ToUpper() + "', '" + description + "', '" +
                attach + "', '" + billerCode + "'";            

            result = xData.updateData(sqlStr);

            return result;
        }

        public String deleteSourceType(String source)
        {
            xData = new BizData();
            String sqlStr = String.Empty;
            String result = String.Empty;

            sqlStr = "exec DeleteSourceInformation '" + source.ToUpper() + "'";

            result = xData.deleteData(sqlStr);

            return result;
        }

        public String insertSourceInformation(String source, String description, String attach, String biller,
            String email, Boolean online, Boolean remit, Boolean active)
        {
            xData = new BizData();
            String sqlStr = String.Empty;
            String result = String.Empty;

            sqlStr = "exec InsertSourceInformation '" + source.ToUpper() + "', '" + description + "', '" +  attach
                + "', '" + biller + "'";

            result = xData.insertData(sqlStr);

            result += Environment.NewLine;

            sqlStr = "exec InsertEmailNotificationInformation '" + source.ToUpper() + "', '" + email + "', '" + online + 
                "', '" + remit + "', '" + active + "'";

            result += xData.insertData(sqlStr);

            return result;
        }
    }
}