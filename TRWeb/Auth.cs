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
        private string xUserID;
        private string xUserFirstName;
        private string xUserLastName;
        private string xUserEmail;
        private string xUserGroup;
        private string xGroupDescription;
        private Boolean xActive;
        private Boolean xAdministrator;

        private BizData biz = new BizData();

        public string UserID
        {
            get { return xUserID; }
            set { xUserID = value; }
        }

        public string UserFirstName
        {
            get { return xUserFirstName; }
            set { xUserFirstName = value; }
        }

        public string UserLastName
        {
            get { return xUserLastName; }
            set { xUserLastName = value; }
        }

        public string UserEmail
        {
            get { return xUserEmail; }
            set { xUserEmail = value; }
        }

        public string UserGroup
        {
            get { return xUserGroup; }
            set { xUserGroup = value; }
        }

        public string GroupDescription
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
            string sqlStr = "";
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

        public ArrayList getActiveUserByUserID(string userId)
        {
            biz = new BizData();
            DataTable userDT = new DataTable();
            string sqlStr = "";
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

        public ArrayList getUserByEmail(string email)
        {
            biz = new BizData();
            DataTable userDT = new DataTable();
            string sqlStr = "";
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

        public string updateUserInformation(string email, string first, string last, string group, Boolean active, Boolean administrator)
        {
            biz = new BizData();
            string sqlStr = "";
            string result = "";

            sqlStr = "exec UpdateUserInformation '" + email + "', '" + first + "', '" + last + "', '" + group + "', '" + active + "', '" + administrator + "'";

            result = biz.updateData(sqlStr);

            return result;
        }

        public string formatUserName()
        {
            string formattedName = "NoNameDefined";
            string adminUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string uName = HttpContext.Current.User.Identity.Name;
            string uHostName = HttpContext.Current.Request.UserHostName;
            //string last = "";
            //string first = "";

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

        public string insertUserInformation(string userId, string first, string last, string email
            , string group, Boolean active)
        {
            biz = new BizData();
            string sqlStr = "";
            string result = "";

            sqlStr = "exec InsertUserInformation '" + userId.ToUpper() + "', '" + first + "', '" + last + "', '" + 
                email + "', '" + group + "', '" + active + "'";

            result = biz.insertData(sqlStr);

            return result;
        }
    }
}
