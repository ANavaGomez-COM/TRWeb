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
using System.Data;
using System.Configuration;
using System.Xml;

namespace TRWeb
{
    public partial class Administration : System.Web.UI.Page
    {
        const string ERR_FILENOTFOUND = "***FILE NOT FOUND***";
        const string ERR_INVALID = "***INVALID***";
        const string ERR_MISMATCH = "***MISMATCH***";

        private string searchSelectedValue = "";

        private BizData biz = new BizData();
        private string packagePath = "";
        private string errorMessage = "";
        static string gridType = "";

        private GridViewRow grdViewRow;
        private Notification notif;
        private Source s;
        private Auth a;

        protected void Page_Load(object sender, EventArgs e)
        {
            biz = new BizData();
            DataTable configDT = new DataTable();
            string sqlStr = "";

            sqlStr = "SELECT Name, Value FROM AppConfig";
            configDT = biz.getData(sqlStr);

            if (configDT.Rows.Count > 0)
            {
                foreach (DataRow row in configDT.Rows)
                {
                    foreach (DataColumn col in configDT.Columns)
                    {
                        if (row[0].ToString().Equals("packagePath"))
                        {
                            packagePath = row[1].ToString();
                        }
                    }
                }
            }
            else
            {
                errorMessage = "The environment variables were not found.";
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            resetPage();

            cmbSearchType.Items.Clear();
            cmbSearchType.Items.Add("Source Type");
            cmbSearchType.Items.Add("Notification Email Address");
            cmbSearchType.Items.Add("Source Description");
            cmbSearchType.Items.Add("User Name");
            cmbSearchType.Items.Add("User Email Address");
            cmbSearchType.Items.Add("User Group");

            biz = new BizData();
            string sqlStr = "exec GetUserGroupList";
            DataTable uGrpList = new DataTable();
            uGrpList = biz.getData(sqlStr);
            cmbUserGroup.DataSource = uGrpList;
            cmbUserGroup.DataValueField = "GroupName";
            cmbUserGroup.DataTextField = "GroupName";
            cmbUserGroup.DataBind();

            if (IsPostBack == false)
            {
                this.cmbSearchType_SelectedIndexChanged(this.cmbSearchType, new EventArgs());
            }
            else
            {
                searchSelectedValue = Session["sessSearchSelVal"].ToString();
            }

            //update contextual display for administrative users--itjas, 9/26/2018
            UpdateEnvironmentContext();
        }

        protected void cmbSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchSelectedValue = cmbSearchType.SelectedValue;
            Session["sessSearchSelVal"] = searchSelectedValue;

            resetPage();
            setAdministrationScreen();

            if (searchSelectedValue.Equals("Source Type"))
            {
                this.cmbSearchList_SelectedIndexChanged(this.cmbSearchList, new EventArgs());
            }
        }

        protected void cmbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            processRequest(searchSelectedValue, false);          
        }

        protected void lstSourceList_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            string search = "";

            ArrayList sourceInfo = new ArrayList();
            ArrayList notifInfo = new ArrayList();
            ArrayList authInfo = new ArrayList();

            lstNotifEmailList.Items.Clear();
            lstUserEmailList.Items.Clear();

            s = new Source();
            search = lstSourceList.SelectedItem.Text;
            sourceInfo = s.getSourceInformation("Source Type", search);

            s = (Source)sourceInfo[0];

            txtSourceDescription.Text = s.Description;
            txtBillerProductCode.Text = s.BillerCode;
            txtSourceAttachment.Text = s.AttachmentName;

            notif = new Notification();
            string type = lstSourceList.SelectedItem.Text;
            notifInfo = notif.getNotificationEmailBySourceType(type);

            if (notifInfo.Count > 0)
            {
                lstNotifEmailList.DataSource = notifInfo;
                lstNotifEmailList.DataValueField = "Email";
                lstNotifEmailList.DataTextField = "Email";
                lstNotifEmailList.DataBind();

                notif = (Notification)notifInfo[0];

                chkActiveNotif.Checked = notif.Active;
                chkReceiveOnline.Checked = notif.OnlineEmail;
                chkReceiveRemittance.Checked = notif.RemittanceEmail;

                foreach (Notification n in notifInfo)
                {
                    a = new Auth();
                    authInfo = new ArrayList();

                    authInfo = a.getUserByEmail(n.Email);

                    if (authInfo.Count > 0)
                    {
                        a = (Auth)authInfo[0];
                        lstUserEmailList.Items.Add(a.UserEmail);
                    }
                }

                if (lstUserEmailList.Items.Count > 0)
                {
                    a = new Auth();
                    authInfo = new ArrayList();
                    string email = lstUserEmailList.Items[0].Text;
                    authInfo = a.getUserByEmail(email);
                    a = (Auth)authInfo[0];
                    txtUserID.Text = a.UserID;
                    txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                    cmbUserGroup.SelectedValue = a.UserGroup;
                    chkActiveUser.Checked = a.Active;
                    chkAdministrator.Checked = a.Administrator;
                }
            }
        }

        protected void lstNotifEmailList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList notifInfo = new ArrayList();

            notif = new Notification();
            string type = lstSourceList.SelectedItem.Text;
            string notifEmail = lstNotifEmailList.SelectedItem.Text;
            notifInfo = notif.getNotificationInformationByEmail(notifEmail, type);

            if (notifInfo.Count > 0)
            {
                notif = (Notification)notifInfo[0];

                chkActiveNotif.Checked = notif.Active;
                chkReceiveOnline.Checked = notif.OnlineEmail;
                chkReceiveRemittance.Checked = notif.RemittanceEmail;
            }
        }

        protected void lstUserEmailList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            a = new Auth();
            ArrayList authInfo = new ArrayList();

            string uEmail = lstUserEmailList.SelectedItem.Text;

            authInfo = a.getUserByEmail(uEmail);

            if (authInfo.Count > 0)
            {
                a = new Auth();
                a = (Auth)authInfo[0];
                txtUserID.Text = a.UserID;
                txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                cmbUserGroup.SelectedValue = a.UserGroup;
                chkActiveUser.Checked = a.Active;
                chkAdministrator.Checked = a.Administrator;
            }
        }

        protected void btnAddSource_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SourceAdd.aspx");
        }

        protected void btnUpdateSource_Click(object sender, EventArgs e)
        {
            s = new Source();
            s.updateSourceType(lstSourceList.SelectedItem.Text, txtSourceDescription.Text,
                txtSourceAttachment.Text, txtBillerProductCode.Text);
        }

        protected void btnDeleteSource_Click(object sender, EventArgs e)
        {
            if (chkHidden.Checked)
            {
                s = new Source();
                s.deleteSourceType(lstSourceList.SelectedItem.Text);
                chkHidden.Checked = false;
                resetPage();
                setAdministrationScreen();

                if (searchSelectedValue.Equals("Source Type"))
                {
                    this.cmbSearchList_SelectedIndexChanged(this.cmbSearchList, new EventArgs());
                }
            }
            else
            {
                confirmBox("Warning: deleting a source type will also delete all associated notifications " +
                    "and orphan all existing transactions associated with it.  If you wish to continue click OK then " +
                    "the Delete Source button.");
            }
        }

        protected void btnListSources_Click(object sender, EventArgs e)
        {
            s = new Source();
            gridType = "Source Type";
            grdResult.DataSource = s.getSourceInformation("All", "");
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }
        
        protected void btnAddEmailNotif_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NotifAdd.aspx");
        }

        protected void btnUpdateEmailNotif_Click(object sender, EventArgs e)
        {
            notif = new Notification();
            ArrayList notifInfo = new ArrayList();
            string type = lstSourceList.SelectedItem.Text;
            string notifEmail = lstNotifEmailList.SelectedItem.Text;

            notifInfo = notif.getNotificationInformationByEmail(notifEmail, type);
            notif = (Notification)notifInfo[0];

            notif.updateNotificationInformation(notif.EmailID, lstNotifEmailList.SelectedItem.Text
                , chkReceiveOnline.Checked, chkReceiveRemittance.Checked, chkActiveNotif.Checked);
        }

        protected void btnListNotifications_Click(object sender, EventArgs e)
        {
            notif = new Notification();
            gridType = "Notification Email Address";
            grdResult.DataSource = notif.getNotificationInformationAll();
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }
        
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AuthAdd.aspx");
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            a = new Auth();
            int spaceLoc = txtUserName.Text.IndexOf(" ");
            string email = lstUserEmailList.SelectedValue;
            string firstName = txtUserName.Text.Substring(0, spaceLoc);
            string lastName = txtUserName.Text.Substring(spaceLoc + 1);
            a.updateUserInformation(email, firstName, lastName, cmbUserGroup.SelectedItem.ToString(), chkActiveUser.Checked, chkAdministrator.Checked);
        }

        protected void btnListUsers_Click(object sender, EventArgs e)
        {
            a = new Auth();
            gridType = "User Email Address";
            grdResult.DataSource = a.getUserAll();
            grdResult.DataBind();
            grdResult.AlternatingRowStyle.BackColor = System.Drawing.Color.LightGray;
        }

        protected void grdResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            processRequest(gridType, true);
        }

        protected void grdResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdResult.PageIndex = e.NewPageIndex;
            grdResult.DataBind();
        }

        protected void grdResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dataTable = grdResult.DataSource as DataTable;

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

                grdResult.DataSource = dataView;
                grdResult.DataBind();
            }
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string newSortDirection = "";

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = "ASC";
                    break;

                case SortDirection.Descending:
                    newSortDirection = "DESC";
                    break;
            }

            return newSortDirection;
        }

        protected void processRequest(string callType, Boolean grdCall)
        {
            string search = "";

            ArrayList sourceInfo = new ArrayList();
            ArrayList notifInfo = new ArrayList();
            ArrayList authInfo = new ArrayList();

            lstSourceList.Items.Clear();
            lstNotifEmailList.Items.Clear();
            lstUserEmailList.Items.Clear();

            s = new Source();
            if (callType.Equals("Source Type"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[1].Text.Trim();
                }
                else
                {
                    search = cmbSearchList.SelectedItem.ToString().Substring(0, 6);
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("Source Description"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("Notification Email Address"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[2].Text.Trim();
                }
                else
                {
                    search = txtSearchSingle.Text;
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Name"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Email Address"))
            {
                if (grdCall)
                {
                    grdViewRow = grdResult.SelectedRow;
                    search = grdViewRow.Cells[4].Text.Trim();
                }
                else
                {
                    search = txtSearchSingle.Text;
                }
                sourceInfo = s.getSourceInformation(callType, search);
            }
            else if (callType.Equals("User Group"))
            {
                search = txtSearchSingle.Text;
                sourceInfo = s.getSourceInformation(callType, search);
            }

            if (sourceInfo.Count > 0)
            {
                lstSourceList.DataSource = sourceInfo;
                lstSourceList.DataValueField = "SourceType";
                lstSourceList.DataTextField = "SourceType";
                lstSourceList.DataBind();

                s = (Source)sourceInfo[0];

                txtSourceDescription.Text = s.Description;
                txtBillerProductCode.Text = s.BillerCode;
                txtSourceAttachment.Text = s.AttachmentName;
            }

            chkReceiveRemittance.Visible = false;
            lblReceiveRemittance.Visible = false;

            if (lstSourceList.Items.Count > 0)
            {
                notif = new Notification();
                string type = lstSourceList.Items[0].Text;
                notifInfo = notif.getNotificationEmailBySourceType(type);

                if ((type == "TRSDP5") || (type == "TRSONL"))
                {
                    chkReceiveRemittance.Visible = true;
                    lblReceiveRemittance.Visible = true;
                }


                if (notifInfo.Count > 0)
                {
                    lstNotifEmailList.DataSource = notifInfo;
                    lstNotifEmailList.DataValueField = "Email";
                    lstNotifEmailList.DataTextField = "Email";
                    lstNotifEmailList.DataBind();

                    notif = (Notification)notifInfo[0];

                    chkActiveNotif.Checked = notif.Active;
                    chkReceiveOnline.Checked = notif.OnlineEmail;
                    chkReceiveRemittance.Checked = notif.RemittanceEmail;

                    foreach (Notification n in notifInfo)
                    {
                        a = new Auth();
                        authInfo = new ArrayList();

                        authInfo = a.getUserByEmail(n.Email);

                        if (authInfo.Count > 0)
                        {
                            a = (Auth)authInfo[0];
                            lstUserEmailList.Items.Add(a.UserEmail);
                        }
                    }

                    if (lstUserEmailList.Items.Count > 0)
                    {
                        a = new Auth();
                        authInfo = new ArrayList();
                        string email = lstUserEmailList.Items[0].Text;
                        authInfo = a.getUserByEmail(email);
                        a = (Auth)authInfo[0];
                        txtUserID.Text = a.UserID;
                        txtUserName.Text = a.UserFirstName + " " + a.UserLastName;
                        cmbUserGroup.SelectedValue = a.UserGroup;
                        chkActiveUser.Checked = a.Active;
                        chkAdministrator.Checked = a.Administrator;
                    }
                }
            }
            if (lstSourceList.Items.Count > 0)
            {
                lstSourceList.SelectedIndex = 0;
            }
            if (lstNotifEmailList.Items.Count > 0)
            {
                lstNotifEmailList.SelectedIndex = 0;
            }
            if (lstUserEmailList.Items.Count > 0)
            {
                lstUserEmailList.SelectedIndex = 0;
            }
        }

        public void setAdministrationScreen()
        {
            if (searchSelectedValue.Equals("Source Type"))
            {
                s = new Source();

                lblSearch.Visible = true;
                cmbSearchList.Visible = true;

                cmbSearchList.DataSource = s.getSourceList();
                cmbSearchList.DataValueField = "SourceType";
                cmbSearchList.DataTextField = "Description";
                cmbSearchList.DataBind();
            }
            else if (searchSelectedValue.Equals("Source Description"))
            {
                lblSearch.Text = "Source Description";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("Notification Email Address"))
            {
                lblSearch.Text = "Email Address";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Name"))
            {
                lblSearch.Text = "User Name";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Email Address"))
            {
                lblSearch.Text = "Email Address";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
            else if (searchSelectedValue.Equals("User Group"))
            {
                lblSearch.Text = "User Group";
                lblSearch.Visible = true;
                txtSearchSingle.Visible = true;
                btnSearch.Visible = true;
            }
        }

        public void resetPage()
        {
            lblSearch.Visible = false;
            cmbSearchList.Visible = false;
            txtSearchSingle.Visible = false;
            btnSearch.Visible = false;

            lstNotifEmailList.Items.Clear();
            lstSourceList.Items.Clear();
            lstUserEmailList.Items.Clear();

            txtBillerProductCode.Text = "";
            txtSearchSingle.Text = "";
            txtSourceAttachment.Text = "";
            txtSourceDescription.Text = "";
            txtUserID.Text = "";
            txtUserName.Text = "";

            chkActiveNotif.Checked = false;
            chkActiveUser.Checked = false;
            chkAdministrator.Checked = false;
            chkReceiveOnline.Checked = false;
            chkReceiveRemittance.Checked = false;
        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        public Boolean confirmBox(string Message) 
        {
            Boolean resp = false;
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "MessageBox",
                "<script type='text/javascript'>confirm_proceed('" + Message + "');</script>");
            resp = chkHidden.Checked;
            return resp;
        }

        protected void btnRunTest_Click(object sender, EventArgs e)
        {
            Application app = new Application();
            Package package = null;
            string password = "OFu=m?ujriK[";
            string resultText = "";

            try
            {

                app.PackagePassword = password;
                package = app.LoadPackage(packagePath + "ERPLoadTest.dtsx", null);
                DTSExecResult results = package.Execute();                          // now execute the test package
                if (results == Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure)
                {
                    foreach (Microsoft.SqlServer.Dts.Runtime.DtsError local_DtsError in package.Errors)
                    {
                        resultText = resultText + "Package Execution results: " + local_DtsError.Description.ToString() + Environment.NewLine;
                    }
                }
                else
                {
                    resultText = resultText + "Process ran successfully.  You will be emailed the result shortly." + Environment.NewLine + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                package.Dispose();
                package = null;
            }
            lblResult.Text = resultText;
        }

        protected void btnShowHide_Clicked(object sender, EventArgs e)
        {
            if (txtWebConfigDet.Visible)
            {
                btnShowHide.Text = "Show Detail";
                txtWebConfigDet.Visible = false;
            }
            else
            {
                btnShowHide.Text = "Hide Detail";
                txtWebConfigDet.Visible = true;
            }
        }

        //Refresh text box showing quick reference showing various configurations for administrative staff (SR-18-0000277)--itjas, 10/10/2018
        protected void UpdateEnvironmentContext()
        {
            const string ENV_DEV = "Dev";
            const string ENV_PROD = "Prod";
            const string ENV_TEST = "Test";

            const string FIL_DTSCONFIG_EXPORT = @"ERPDataExports\ConfigExport.dtsConfig";
            const string FIL_DTSCONFIG_LOAD = @"ERPDataExports\ConfigLoad.dtsConfig";
            const string FIL_DTSCONFIG_LOADEPAY = @"ERPDataExports\ConfigLoadEpayment.dtsConfig";
            const string FIL_DTSCONFIG_TYLER = @"ERPDataExports\ConfigTyler.dtsConfig";

            const string KEY_BAT = "batFile";
            const string KEY_OCRAccela = "OCRAccela";
            const string KEY_KUBRABatFile = "KUBRABatFile";
            const string KEY_KUBRAUploadBatFile = "KUBRAUploadBatFile";

            const string SRV_DEV = "sqldt1\\dev";
            const string SRV_PROD = "sqlentdblstn";
            const string SRV_TEST = "sqldt1\\test";

            const string URL_DEV = "http://erpbiztalktest:90";
            const string URL_PROD = "http://erpbiztalk";
            const string URL_TEST = "http://erpbiztalktest";

            //initialization
            AppSettingsReader vReader = new AppSettingsReader();
            string vEnv = (string)vReader.GetValue("env", typeof(string));  //establish the environment from Web.config
            var vAppDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var vBasePath = vAppDomainBaseDirectory.Substring(0, vAppDomainBaseDirectory.IndexOf(@"\TRWeb\") + 1);
            string vErrorType = "";
            System.Drawing.Color vForeColor = System.Drawing.Color.Green;
            string vPathCashieringServicesWebConfig = "";
            string vPathERPDataExportsDTSConfigExport = "";
            string vPathERPDataExportsDTSConfigLoad = "";
            string vPathERPDataExportsDTSConfigEpay = "";
            string vPathERPDataExportsDTSConfigTyler = "";
            string vPathTRWebWebConfig = "";
            string vPathTRWebWebConfigERPDataExports = "";
            string vPrefix = "";
            string vDetail = "";
            string vURL = Request.Url.GetLeftPart(UriPartial.Authority);

            lblWebConfig_env.Font.Bold = true;
            lblWebConfig_env.Text = vEnv;

            switch (vEnv)
            {
                case ENV_DEV:
                case ENV_TEST:
                    vPathERPDataExportsDTSConfigExport = vEnv + @"ERPDataExports\" + FIL_DTSCONFIG_EXPORT;
                    vPathERPDataExportsDTSConfigLoad = vEnv + @"ERPDataExports\" + FIL_DTSCONFIG_LOAD;
                    vPathERPDataExportsDTSConfigEpay = vEnv + @"ERPDataExports\" + FIL_DTSCONFIG_LOADEPAY;
                    vPathERPDataExportsDTSConfigTyler = vEnv + @"ERPDataExports\" + FIL_DTSCONFIG_TYLER;
                    vPrefix = vEnv;
                    break;
                case ENV_PROD:
                    vPathERPDataExportsDTSConfigExport = @"ERPDataExports\" + FIL_DTSCONFIG_EXPORT;
                    vPathERPDataExportsDTSConfigLoad = @"ERPDataExports\" + FIL_DTSCONFIG_LOAD;
                    vPathERPDataExportsDTSConfigEpay = @"ERPDataExports\" + FIL_DTSCONFIG_LOADEPAY;
                    vPathERPDataExportsDTSConfigTyler = @"ERPDataExports\" + FIL_DTSCONFIG_TYLER;
                    break;
            }

            vPathTRWebWebConfig = vAppDomainBaseDirectory + "Web.config";
            vPathTRWebWebConfigERPDataExports = @"C:\" + vPrefix + @"ERPDataExports\";
            if (vURL.Contains("localhost"))
            {
                vPathCashieringServicesWebConfig = vBasePath + @"CashieringServices\Web.config";
                vPathERPDataExportsDTSConfigExport = vBasePath + vPathERPDataExportsDTSConfigExport;
                vPathERPDataExportsDTSConfigLoad = vBasePath + vPathERPDataExportsDTSConfigLoad;
                vPathERPDataExportsDTSConfigEpay = vBasePath + vPathERPDataExportsDTSConfigEpay;
                vPathERPDataExportsDTSConfigTyler = vBasePath + vPathERPDataExportsDTSConfigTyler;
            }
            else
            {
                vPathCashieringServicesWebConfig = vBasePath + @"CashieringService\Web.config"; //yeah, server is different than debugging environment!!1!--itjas, 10/11/2018
                vPathERPDataExportsDTSConfigExport = @"C:\" + vPathERPDataExportsDTSConfigExport;
                vPathERPDataExportsDTSConfigLoad = @"C:\" + vPathERPDataExportsDTSConfigLoad;
                vPathERPDataExportsDTSConfigEpay = @"C:\" + vPathERPDataExportsDTSConfigEpay;
                vPathERPDataExportsDTSConfigTyler = @"C:\" + vPathERPDataExportsDTSConfigTyler;
            }

            //TRWeb Web.config validations
            vDetail = "TRWeb Web.config (" + vPathTRWebWebConfig + ") validations. . . .";
            if (File.Exists(vPathTRWebWebConfig))
            {
                vDetail += "\n\tenv = \"" + vEnv + "\"";

                if (vURL.Contains("localhost")) { }
                else if (vEnv.Equals(ENV_DEV))
                {
                    if (vURL != URL_DEV)
                    {
                        vErrorType = ERR_MISMATCH;
                    }
                }
                else if (vEnv.Equals(ENV_TEST))
                {
                    if (vURL != URL_TEST)
                    {
                        vErrorType = ERR_MISMATCH;
                    }
                }
                else if (vEnv.Equals(ENV_PROD))
                {
                    if (vURL != URL_PROD)
                    {
                        vErrorType = ERR_MISMATCH;
                    }
                }
                else
                {
                    vErrorType = ERR_INVALID;
                }

                if (vErrorType.Length > 0)
                {
                    vDetail += " " + vErrorType;
                    vForeColor = System.Drawing.Color.Red;
                }

                //validate folders for important files in the TRWeb Web.config
                foreach (string vKey in new List<string> { KEY_BAT, KEY_OCRAccela, KEY_KUBRABatFile, KEY_KUBRAUploadBatFile })
                {
                    string vValue = (string)vReader.GetValue(vKey, typeof(string));
                    string vOutput = "\t" + vKey + " = \"" + vValue + "\"";
                    if (vValue.StartsWith(vPathTRWebWebConfigERPDataExports))
                    {
                        vDetail += "\n" + vOutput;
                    }
                    else
                    {
                        vDetail += "\n" + vOutput + " " + ERR_MISMATCH;
                        vForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else
            {
                vDetail += "\n\t" + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            //validate Cashiering Services connectionString
            vDetail += "\n\nCashieringServices Web.config (" + vPathCashieringServicesWebConfig + ") validations. . . .";

            if (File.Exists(vPathCashieringServicesWebConfig))
            {
                string vCashieringDetail = "";  //separate variable to accumulate detail within CashieringServices web.Config
                int vValidCashiering = 0;       //used to count valid entries
                ExeConfigurationFileMap Map = new ExeConfigurationFileMap();
                Map.ExeConfigFilename = vPathCashieringServicesWebConfig;
                System.Configuration.Configuration configurationManager = ConfigurationManager.OpenMappedExeConfiguration(Map, ConfigurationUserLevel.None);
                foreach (ConnectionStringSettings css in configurationManager.ConnectionStrings.ConnectionStrings)
                {
                    string vValue = css.ConnectionString;
                    string vServer = vValue.Split(';')[1];
                    if (css.Name == "bizcash_ConnectionString")
                    {
                        vValidCashiering += 1;
                        vCashieringDetail += String.Format("\n\t{0} \"{1}\"", css.Name, vServer);
                        vErrorType = "";
                        if (vEnv.Equals(ENV_DEV))
                        {
                            if (!vServer.EndsWith("=" + SRV_DEV))
                            {
                                vErrorType = ERR_MISMATCH;
                            }
                        }
                        else if (vEnv.Equals(ENV_TEST))
                        {
                            if (!vServer.EndsWith("=" + SRV_TEST))
                            {
                                vErrorType = ERR_MISMATCH;
                            }
                        }
                        else if (vEnv.Equals(ENV_PROD))
                        {
                            if (!vServer.EndsWith("=" + SRV_PROD))
                            {
                                vErrorType = ERR_MISMATCH;
                            }
                        }

                        if (vErrorType.Length > 0)
                        {
                            vCashieringDetail += " " + vErrorType;
                            vForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else if (css.Name == "Prop_ConnectionString")   //somewhat different than the rest of the details as there are no checks here--dev, test, and prod are the same--itjas, 10/23/2018
                    {
                        vValidCashiering += 1;
                        vCashieringDetail += String.Format("\n\t{0} \"{1}\"", css.Name, vServer);
                        if (vServer != "Server=webdblstn")
                        {
                            vCashieringDetail += " " + ERR_INVALID;
                            vForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                if (vValidCashiering != 2)
                {
                    vDetail += " " + ERR_INVALID;
                    vForeColor = System.Drawing.Color.Red;
                }
                vDetail += vCashieringDetail;
            }
            else
            {
                vDetail += " " + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            //validate Exports ConfigLoad.dtsConfig
            vDetail += "\n\nERPDataExports ConfigLoad.dtsConfig (" + vPathERPDataExportsDTSConfigLoad + ") validations. . . .";

            if (File.Exists(vPathERPDataExportsDTSConfigLoad))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(vPathERPDataExportsDTSConfigLoad);
                ValidateNodes(doc.FirstChild.ParentNode, @"ERPDataExports\", vPrefix, ref vDetail, ref vForeColor);
                ValidateNodes(doc.FirstChild.ParentNode, "ERPReports/", vPrefix, ref vDetail, ref vForeColor);
                switch (vEnv)
                {
                    case ENV_DEV:
                    case ENV_TEST:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", vEnv, ref vDetail, ref vForeColor);
                        break;
                    case ENV_PROD:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", "sqlentdblstn", ref vDetail, ref vForeColor);
                        break;
                }
            }
            else
            {
                vDetail += "\n\t" + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            //validate Exports ConfigExport.dtsConfig
            vDetail += "\n\nERPDataExports ConfigExport.dtsConfig (" + vPathERPDataExportsDTSConfigExport + ") validations. . . .";

            if (File.Exists(vPathERPDataExportsDTSConfigExport))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(vPathERPDataExportsDTSConfigExport);
                ValidateNodes(doc.FirstChild.ParentNode, @"ERPDataExports\", vPrefix, ref vDetail, ref vForeColor);
                ValidateNodes(doc.FirstChild.ParentNode, "ERPReports/", vPrefix, ref vDetail, ref vForeColor);
                switch (vEnv)
                {
                    case ENV_DEV:
                    case ENV_TEST:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", vEnv, ref vDetail, ref vForeColor);
                        break;
                    case ENV_PROD:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", "sqlentdblstn", ref vDetail, ref vForeColor);
                        break;
                }
            }
            else
            {
                vDetail += "\n\t" + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            //validate Exports ConfigLoadEpayment.dtsConfig
            vDetail += "\n\nERPDataExports ConfigLoadEpayment.dtsConfig (" + vPathERPDataExportsDTSConfigEpay + ") validations. . . .";

            if (File.Exists(vPathERPDataExportsDTSConfigEpay))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(vPathERPDataExportsDTSConfigEpay);
                switch (vEnv)
                {
                    case ENV_DEV:
                    case ENV_TEST:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", "test", ref vDetail, ref vForeColor);
                        break;
                    case ENV_PROD:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Data Source", "webdblstn", ref vDetail, ref vForeColor);
                        break;
                }
            }
            else
            {
                vDetail += "\n\t" + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            //validate Exports ConfigTyler.dtsConfig
            vDetail += "\n\nERPDataExports ConfigTyler.dtsConfig (" + vPathERPDataExportsDTSConfigTyler + ") validations. . . .";

            if (File.Exists(vPathERPDataExportsDTSConfigTyler))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(vPathERPDataExportsDTSConfigTyler);
                switch (vEnv)
                {
                    case ENV_DEV:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Initial Catalog", "_test", ref vDetail, ref vForeColor);
                        break;
                    case ENV_TEST:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Initial Catalog", "_train", ref vDetail, ref vForeColor);
                        break;
                    case ENV_PROD:
                        ValidateSubNodes(doc.FirstChild.ParentNode, "Initial Catalog", "cashiering", ref vDetail, ref vForeColor);
                        break;
                }
            }
            else
            {
                vDetail += "\n\t" + ERR_FILENOTFOUND;
                vForeColor = System.Drawing.Color.Red;
            }

            lblWebConfig_env.ForeColor = vForeColor;
            txtWebConfigDet.Text = vDetail;
        }

        //recursively search ConfigLoad.dtsConfig for relevant values--itjas, 10/10/2018
        void ValidateNodes(XmlNode pNode, string pSubFolder, string pPrefix, ref string pToolTip, ref System.Drawing.Color pForeColor)
        {
            if (pNode.Name == "ConfiguredValue")
            {
                if (pNode.InnerText.IndexOf(pSubFolder) > -1)   //a ConfiguredValue that we're interested in
                {
                    string vDelimiter = pSubFolder.Substring(pSubFolder.Length - 1);
                    pToolTip += "\n\t" + pNode.InnerText;
                    if (pNode.InnerText.IndexOf(vDelimiter + pPrefix + pSubFolder) == -1)
                    {
                        pToolTip += " " + ERR_MISMATCH;
                        pForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else if (pNode.HasChildNodes)
            {
                for (int i = 0; i < pNode.ChildNodes.Count; i++)
                {
                    ValidateNodes(pNode.ChildNodes[i], pSubFolder, pPrefix, ref pToolTip, ref pForeColor);
                }
            }
        }

        //recursively search ConfigLoad.dtsConfig for relevant values--itjas, 10/10/2018
        void ValidateSubNodes(XmlNode pNode, string pSubString, string pEnv, ref string pToolTip, ref System.Drawing.Color pForeColor)
        {
            if (pNode.Name == "ConfiguredValue")
            {
                foreach (string vSubnode in pNode.InnerText.Split(';'))
                {
                    if (vSubnode.IndexOf(pSubString) > -1) //a sub-node of ConfiguredValue that we're interested in
                    {
                        pToolTip += "\n\t" + vSubnode;
                        if (!vSubnode.ToUpper().EndsWith(pEnv.ToUpper()))
                        {
                            pToolTip += " " + ERR_MISMATCH;
                            pForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            else if (pNode.HasChildNodes)
            {
                for (int i = 0; i < pNode.ChildNodes.Count; i++)
                {
                    ValidateSubNodes(pNode.ChildNodes[i], pSubString, pEnv, ref pToolTip, ref pForeColor);
                }
            }
        }
    }
}