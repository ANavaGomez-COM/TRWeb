using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Collections;
using System.Configuration;
using System.Data;

namespace TRWeb
{
    public class Auth
    {
        private String xUserID;
        private String xUserFirstName;
        private String xUserLastName;
        private String xUserEmail;
        private String xUserGroup;
        private String xGroupDescription;
        private Boolean xActive;
        private Boolean xAdministrator;

        private BizData biz = new BizData();

        public String UserID
        {
            get { return xUserID; }
            set { xUserID = value; }
        }

        public String UserFirstName
        {
            get { return xUserFirstName; }
            set { xUserFirstName = value; }
        }

        public String UserLastName
        {
            get { return xUserLastName; }
            set { xUserLastName = value; }
        }

        public String UserEmail
        {
            get { return xUserEmail; }
            set { xUserEmail = value; }
        }

        public String UserGroup
        {
            get { return xUserGroup; }
            set { xUserGroup = value; }
        }

        public String GroupDescription
        {
            get { return xGroupDescription; }
            set { xGroupDescription = value; }
        }

        public Boolean Active
        {
            get { return xActive; }
            set { xActive = value; }
        }

        public Boolean Administrator
        {
            get { return xAdministrator; }
            set { xAdministrator = value; }
        }
        
        public ArrayList getUserAll()
        {
            biz = new BizData();
            DataTable userDT = new DataTable();
            String sqlStr = String.Empty;
            ArrayList result = new ArrayList();

            sqlStr = "exec GetUserAll";
            userDT = biz.getData(sqlStr);

            foreach (DataRow row in userDT.Rows)
            {
                Auth auth = new Auth();
                foreach (DataColumn col in userDT.Columns)
                {
                    if (col.ColumnName.Equals("UserID"))
                    {
                        auth.UserID = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserFirstName"))
                    {
                        auth.UserFirstName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserLastName"))
                    {
                        auth.UserLastName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserEmail"))
                    {
                        auth.UserEmail = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupName"))
                    {
                        auth.UserGroup = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupDescription"))
                    {
                        auth.GroupDescription = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        auth.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Administrator"))
                    {
                        auth.Administrator = Convert.ToBoolean(row[col].ToString());
                    }

                }
                result.Add(auth);
            }
            return result;
        }

        public ArrayList getActiveUserByUserID(String userId)
        {
            biz = new BizData();
            DataTable userDT = new DataTable();
            String sqlStr = String.Empty;
            ArrayList result = new ArrayList();
            
            sqlStr = "exec GetActiveUserByUserID " + userId;
            userDT = biz.getData(sqlStr);

            foreach (DataRow row in userDT.Rows)
            {
                Auth auth = new Auth();
                foreach (DataColumn col in userDT.Columns)
                {
                    if (col.ColumnName.Equals("UserID"))
                    {
                        auth.UserID = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserFirstName"))
                    {
                        auth.UserFirstName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserLastName"))
                    {
                        auth.UserLastName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserEmail"))
                    {
                        auth.UserEmail = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupName"))
                    {
                        auth.UserGroup = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupDescription"))
                    {
                        auth.GroupDescription = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        auth.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Administrator"))
                    {
                        auth.Administrator = Convert.ToBoolean(row[col].ToString());
                    }
                }
                result.Add(auth);
            }
            return result;
        }

        public ArrayList getUserByEmail(String email)
        {
            biz = new BizData();
            DataTable userDT = new DataTable();
            String sqlStr = String.Empty;
            ArrayList result = new ArrayList();

            sqlStr = "exec GetUserByEmail '" + email + "'";
            userDT = biz.getData(sqlStr);

            foreach (DataRow row in userDT.Rows)
            {
                Auth auth = new Auth();
                foreach (DataColumn col in userDT.Columns)
                {
                    if (col.ColumnName.Equals("UserID"))
                    {
                        auth.UserID = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserFirstName"))
                    {
                        auth.UserFirstName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserLastName"))
                    {
                        auth.UserLastName = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("UserEmail"))
                    {
                        auth.UserEmail = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupName"))
                    {
                        auth.UserGroup = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("GroupDescription"))
                    {
                        auth.GroupDescription = row[col].ToString();
                    }
                    if (col.ColumnName.Equals("Active"))
                    {
                        auth.Active = Convert.ToBoolean(row[col].ToString());
                    }
                    if (col.ColumnName.Equals("Administrator"))
                    {
                        auth.Administrator = Convert.ToBoolean(row[col].ToString());
                    }
                }
                result.Add(auth);
            }
            return result;
        }

        public String updateUserInformation(String email, String first, String last, String group, Boolean active, Boolean administrator)
        {
            biz = new BizData();
            String sqlStr = String.Empty;
            String result = String.Empty;

            sqlStr = "exec UpdateUserInformation '" + email + "', '" + first + "', '" + last + "', '" + group + "', '" + active + "', '" + administrator + "'";

            result = biz.updateData(sqlStr);

            return result;
        }

        public String formatUserName()
        {
            String formattedName = "NoNameDefined";
            String adminUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            String uName = HttpContext.Current.User.Identity.Name;
            String uHostName = HttpContext.Current.Request.UserHostName;
            String last = String.Empty;
            String first = String.Empty;

            int location = uName.IndexOf("\\");
            if (location < 0)
            {
                formattedName = uName;
                formattedName = formattedName.ToUpper();
            }
            else
            {
                formattedName = uName.Substring(location + 1);
                formattedName = formattedName.ToUpper();
                if (formattedName.Equals("BIZCASH"))
                {
                    formattedName = "ITBizcash";
                }
                else if (formattedName.Equals("DEVMIN"))
                {
                    formattedName = "ITDevmin";
                }
                else if (formattedName.Equals("ITJSM"))
                {
                    formattedName = "ITJSM";
                }
            }
            return formattedName;
        }

        public String insertUserInformation(String userId,String first, String last, String email
            , String group, Boolean active)
        {
            biz = new BizData();
            String sqlStr = String.Empty;
            String result = String.Empty;

            sqlStr = "exec InsertUserInformation '" + userId.ToUpper() + "', '" + first + "', '" + last + "', '" + 
                email + "', '" + group + "', '" + active + "'";

            result = biz.insertData(sqlStr);

            return result;
        }
    }
}