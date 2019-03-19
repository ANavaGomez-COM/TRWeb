using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Server;
using System.Web.Services.Protocols;
using TRWeb.sqlentdb1ReportService;
using System.Collections;
using System.IO;
using System.Text;
using System.Data;
using System.Net.Mail;
using System.Diagnostics;
using System.Configuration;
using System.Threading;

/* 	Riki 1-29-15 RIKI#2 It needs to run a external program the OCR like how it runs the e-payment script 
	ITJBS = Jane S = Jane Schneider
	ITKG = kg = Kim G = Kim Grittner
	ITJBS 2019-01-24: Added TiPSS code (SR-17-0375).
*/

namespace TRWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        private string userName = "";
        private Auth a = new Auth();
        private BizData biz = new BizData();

        private string groupSelectedValue = "";
        private string functionSelectedValue = "";
        private string errorMessage = "";
        private string packagePath = "";
        private string importFilePath = "";
        private string ePayDestPath = "";
        private string reportFilePath = "";
        private string logFilePath = "";
        private string KUBRAUploadfiles = "";

        // kg 9/10/2015 - Moved into functions and populated from web.config
        //private string batFile = "C:\\DevERPDataExports\\runEpaymentProgram.bat";
        //private string OCRAccela = "C:\\DevERPDataExports\\OCRAccela.bat"; // RIKI#2
        private string processSummaryReport = "";
        private string onlineProcessSummaryReport = "";
        private string[] files = new string[] { };
        private string folder = string.Empty;
        static string fileDate = string.Empty;
        static ArrayList actualFile = new ArrayList();
        private string resultText = string.Empty;
        //private IFormatProvider result;

        protected void Page_Init(object sender, EventArgs e)
        {
            ArrayList userInfo = new ArrayList();
            userName = a.formatUserName();
            userInfo = a.getActiveUserByUserID(userName);
            Session["User"] = userInfo;
            if (userInfo.Count > 0)
            {
                a = ((Auth)userInfo[0]);
                welcomeHeader.InnerHtml = "Welcome " + a.UserFirstName + " " + a.UserLastName +
                    "&nbsp&nbsp&nbsp&nbsp" + DateTime.Now;

                resetPage();

                cmbGroup.Items.Clear();
                if (a.UserGroup == "IT")
                {
                    cmbGroup.Items.Add("Treasurer");
                    cmbGroup.Items.Add("Housing");
                    cmbGroup.Items.Add("Police");
                    cmbGroup.Items.Add("Water");
                    cmbGroup.Items.Add("Parking");
                    cmbGroup.Items.Add("Water Admin");
					cmbGroup.Items.Add("Court");        //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
                }
                else if (a.UserGroup == "Water Admin")
                {
                    //ceb
                    //cmbGroup.Items.Add("Water Admin");
                    //cmbGroup.Enabled = false;

                    cmbGroup.Items.Add("Water");
                    cmbGroup.Enabled = false;
                    //ceb
                }
                else if (a.UserGroup == "Treasurer")
                {
                    cmbGroup.Items.Add("Treasurer");
                    cmbGroup.Enabled = false;
                }
                else if ((a.UserGroup == "Housing") || (a.UserGroup == "CDA"))
                {
                    cmbGroup.Items.Add("Housing");
                    cmbGroup.Enabled = false;
                }
                else if (a.UserGroup == "Water")
                {
                    cmbGroup.Items.Add("Water");
                    cmbGroup.Enabled = false;
                }
                else if (a.UserGroup == "Police")
                {
                    cmbGroup.Items.Add("Police");
                    cmbGroup.Enabled = false;
                }
                else if (a.UserGroup == "Parking")
                {
                    cmbGroup.Items.Add("Parking");
                    cmbGroup.Enabled = false;
                }
				else if (a.UserGroup == "Court")        //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
                {
                    cmbGroup.Items.Add("Court");      
                    cmbGroup.Enabled = false;
                }
                else
                {
                    welcomeHeader.InnerHtml = "You must be approved to use this site" + DateTime.Now;
                }

                if (IsPostBack == false)
                {
                    this.cmbGroup_SelectedIndexChanged(this.cmbGroup, new EventArgs());
                }
                else
                {
                    groupSelectedValue = Session["sessGrpSelVal"].ToString();
                    functionSelectedValue = Session["sessFuncSelVal"].ToString();
                }
            }
            else
            {
                Response.Write("User: " + userName + " ACCESS DENIED");
                //Response.Redirect("~/UserError.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            biz = new BizData();
            DataTable configDT = new DataTable();
            string sqlStr = "";

            btnProcess.Attributes.Add("onclick", " this.disabled = true; " +
                ClientScript.GetPostBackEventReference(btnProcess, null) + ";");

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
                        if (row[0].ToString().Equals("importFilePath"))
                        {
                            importFilePath = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("ePayDestPath"))
                        {
                            ePayDestPath = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("reportFilePath"))
                        {
                            reportFilePath = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("logFilePath"))
                        {
                            logFilePath = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("processSummaryReport"))
                        {
                            processSummaryReport = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("OnlineProcessSummaryReport"))
                        {
                            onlineProcessSummaryReport = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("sourceFolderPath"))
                        {
                            folder = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("KUBRAUploadfiles"))
                        {
                            KUBRAUploadfiles = row[1].ToString();
                        }
                    }
                }
            }
            else
            {
                errorMessage = "The environment variables were not found.";
            }

            if (IsPostBack == false)
            {
                return; // itjas debugging
                importFileCleanup();
                logFileCleanup();
                reportFileCleanup();
            }
        }

        protected void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFunction.Items.Clear();
            groupSelectedValue = cmbGroup.SelectedValue;
            Session["sessGrpSelVal"] = groupSelectedValue;

            if (groupSelectedValue.Equals("Housing"))
            {
                cmbFunction.Items.Add("Create Elite File");
            }
            else if (groupSelectedValue.Equals("Water"))
            {
                cmbFunction.Items.Add("Create Water File");
            }
            else if (groupSelectedValue.Equals("Police"))
            {
                cmbFunction.Items.Add("Create Parking Tickets File");
            }
            else if (groupSelectedValue.Equals("Treasurer"))
            {
                cmbFunction.Items.Add("Update Posted Date in OCR");
                cmbFunction.Items.Add("Load OCR File");
                // cmbFunction.Items.Add("Load US Bank File");
                cmbFunction.Items.Add("Load Online Files");
                cmbFunction.Items.Add("Create Tax File");
                cmbFunction.Items.Add("Create Dane Co Tax File");
            }
            else if (groupSelectedValue.Equals("Parking"))
            {
                cmbFunction.Items.Add("Send Parking File");
            }
            else if (groupSelectedValue.Equals("Water Admin"))
            {
                cmbFunction.Items.Add("Water Administration");
            }
            else if (groupSelectedValue.Equals("Court"))                //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
            {
                cmbFunction.Items.Add("Create Municipal Court File");
            }

            this.cmbFunction_SelectedIndexChanged(this.cmbFunction, new EventArgs());
        }

        protected void cmbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            functionSelectedValue = cmbFunction.SelectedValue;
            Session["sessFuncSelVal"] = functionSelectedValue;

            if (functionSelectedValue.Equals("Update Posted Date in OCR"))
            {
                resetPage();
                setScreen();
            }
            else if (functionSelectedValue.Equals("Load OCR File"))
            {
                resetPage();
                setScreen();
                cldFileDate.SelectedDate = DateTime.Now;
                this.cldFileDate_SelectionChanged(this.cldFileDate, new EventArgs());
            }
            //else if (functionSelectedValue.Equals("Load US Bank File"))
            else if (functionSelectedValue.Equals("Load Online Files"))
            {
                resetPage();
                setScreen();
                // kg added 12/4/2015 - read files from sftp site
                var reader = new AppSettingsReader();
                var batFileKUBRA = (reader.GetValue("KUBRABatFile", typeof(string))).ToString();
                if (File.Exists(batFileKUBRA))
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();

                    proc.StartInfo.FileName = batFileKUBRA;
                    proc.StartInfo.RedirectStandardError = false;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.StartInfo.UseShellExecute = false;
                    proc.Start();
                    proc.WaitForExit();
                }
                cldFileDate.SelectedDate = DateTime.Now;
                this.cldFileDate_SelectionChanged(this.cldFileDate, new EventArgs());
            }
            else if (functionSelectedValue.Equals("Create Elite File"))
            {
                resetPage();
                btnProcess.Visible = true;
            }

            else if (functionSelectedValue.Equals("Create Parking Tickets File"))
            {
                resetPage();
                btnProcess.Visible = true;
            }
            else if (functionSelectedValue.Equals("Create Water File"))
            {
                resetPage();
                btnProcess.Visible = true;
            }
            else if (functionSelectedValue.Equals("Create Tax File"))
            {
                resetPage();
                btnProcess.Visible = true;
            }
            else if (functionSelectedValue.Equals("Create Dane Co Tax File"))
            {
                resetPage();
                setScreen();
                btnProcess.Visible = true;
            }
            else if (functionSelectedValue.Equals("Send Parking File"))
            {
                resetPage();
                setScreen();
                btnProcess.Visible = true;
            }
            else if (functionSelectedValue.Equals("Create Municipal Court File"))       //Jane S, 2019-01-24: Added for TiPSS, SR-17-0375
            {
                resetPage();
                btnProcess.Visible = true;
            }
        }

        protected bool isDP5Valid(string fName)
        {
            bool retCode = false;
            Int64 parmUserCount = 0;
            decimal parmUserTotal = 0;
            Parameters parms;
            string isDANEFilledIn = "N";

            var reader = new AppSettingsReader();
            //string password = "OFu=m?ujriK[";
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();

            Application app = new Application();
            Package package = null;

            try
            {
                retCode = Int64.TryParse(tbCount.Text, out parmUserCount);          // See if they entered a rowcount number
                if (retCode)
                {
                    retCode = decimal.TryParse(tbTotal.Text, out parmUserTotal);    // See if they entered a total number
                }
                if (retCode)    // Now time to run package which will compare the Checksum values and create a row in table: ProcessValidation
                {
                    if(txtDCoPayDate.Text != "" && txtDCoPenDate.Text != "" && txtDCoTaxYear.Text != "")       // If they filled in ALL of the Dane fields,
                    {
                        isDANEFilledIn = "Y";           // Set isDANEFilledIn to be true
                    }
                    app.PackagePassword = password;
                    package = app.LoadPackage(packagePath + "ERPValidateDP5.dtsx", null);
                    parms = package.Parameters;
                    parms["ParmBatchID"].Value = "";
                    parms["ParmImportFileName"].Value = fName;

                    parms["ParmUserCount"].Value = parmUserCount;
                    parms["ParmUserTotal"].Value = parmUserTotal;

                    parms["ParmWorkStationID"].Value = userName;
                    parms["ParmWorkStationName"].Value = "myComputer";

                    parms["ParmDaneFilledIn"].Value = isDANEFilledIn;

                    DTSExecResult results = package.Execute();
                    if (results == Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure)
                    {
                        foreach (Microsoft.SqlServer.Dts.Runtime.DtsError local_DtsError in package.Errors)
                        {
                            retCode = false;        // Error occurred, set returncode to false
                            resultText = resultText + "Package Execution results: " + local_DtsError.Description.ToString() + Environment.NewLine;
                        }
                    }
                    else
                    {
                        resultText = resultText + "Process ran successfully." + Environment.NewLine + Environment.NewLine;
                        // Now read the database to discover the results
                        string sqlStr = "SELECT TOP 1 * FROM ProcessValidation WHERE Status <> 'Processing' AND SourceType = 'VALDP5' AND CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE) ORDER BY PackageStartDate DESC";
                        biz = new BizData();
                        DataTable configDT = new DataTable();
                        configDT = biz.getData(sqlStr);

                        if (configDT.Rows.Count > 0)
                        {
                            foreach (DataRow row in configDT.Rows)
                            {
                                if(row["Status"].ToString() == "Valid")
                                {
                                    resultText = "Validation results: Valid" + Environment.NewLine + "Press the 'Process' button to proceed.";
                                }
                                else
                                {
                                    retCode = false;
                                    resultText = "Validation results: " + row["Status"].ToString() + Environment.NewLine + "Validation Error: " + row["ValidationError"].ToString();
                                }
                            }
                        }
                        else
                        {
                            resultText = "Unable to read ProcessValidation Table for this DP5 file.";
                            retCode = false;
                        }
                    }
                }
                else
                {
                    resultText = "You must enter a Count and Total for the DP5 file.";
                }
            }
            catch (Exception)
            {
                retCode = false;        // Error occurred, set returncode to false
                //resultText = Error;
                throw;
            }
            return retCode;
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            if (functionSelectedValue == "Update Posted Date in OCR")
            {
                resultText = "";
                bool isValid = true;
                if (txtCashCode.Text.Trim().Length < 1)
                {
                    resultText = "You must enter a Work Source Code." + System.Environment.NewLine;
                    isValid = false;
                }
                if (txtPostDate.Text.Trim().Length < 1)
                {
                    resultText = resultText + "You must enter a Post Date." + System.Environment.NewLine;
                    isValid = false;
                }
                if (isValid)    // If they did enter a Post Date
                {
                    DateTime dtPostDate = Convert.ToDateTime(txtPostDate.Text);                 // Get the post date
                    DateTime dtNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());      // Get the current date as a short date
                    int days = (dtNow.Date - dtPostDate.Date).Days;
                    if (days < 0 || days > 14)
                    {
                        resultText = resultText + "Post Date must be today or within the last 14 days." + System.Environment.NewLine;
                        isValid = false;
                    }
                }
                if (txtBeginSeq.Text.Trim().Length < 1)
                {
                    resultText = resultText + "You must enter a Sequence Number Begin." + System.Environment.NewLine;
                    isValid = false;
                }
                if (txtEndSeq.Text.Trim().Length < 1)
                {
                    resultText = resultText + "You must enter a Sequence Number End." + System.Environment.NewLine;
                    isValid = false;
                }
                if (isValid)    // If they entered a Sequence Begin and End
                {
                    if (Int32.Parse(txtBeginSeq.Text) > Int32.Parse(txtEndSeq.Text))    // Check that the begin sequence is equal or greater than end sequence
                    {
                        resultText = resultText + "Sequence Number Begin must be less than or equal to the Sequence Number End." + System.Environment.NewLine;
                        isValid = false;
                    }
                }
                btnProcess.Enabled = isValid;
            }
            else if (functionSelectedValue == "Load OCR File")
            {
                foreach (string filePath in actualFile)
                {
                    string tmpFileName = filePath.ToString();
                    int position = tmpFileName.LastIndexOf('\\');
                    string fileName = tmpFileName.Substring(position + 1);
                    string destFilePath = importFilePath + fileName;
                    File.Copy(filePath, destFilePath, true);
                    string tempePayDestPath = "";

                    if (fileName.Contains("DP5"))
                    {
                        if (isDP5Valid(fileName))
                        {
                            btnProcess.Enabled = true;
                        }
                        else
                        {
                            btnProcess.Enabled = false;
                        }
                        break;  // Only handle the first DP5 file encountered.
                    }
                }
            }
            else if (functionSelectedValue == "Load Online Files")
            {
                foreach (string filePath in actualFile)
                {
                    string tmpFileName = filePath.ToString();
                    int position = tmpFileName.LastIndexOf('\\');
                    string fileName = tmpFileName.Substring(position + 1);
                    string destFilePath = importFilePath + fileName;
                    File.Copy(filePath, destFilePath, true);
                    string tempePayDestPath = "";

                    if (fileName.Contains("COM") || IsKubraFile(fileName))
                    {
                        if (isComValid(fileName))
                        {
                            btnProcess.Enabled = true;
                        }
                        else
                        {
                            btnProcess.Enabled = false;
                            break;  // Exit foreach if error encountered.
                        }
                    }
                }
            }
            lblResult.Visible = true;
            txtResult.Visible = true;
            txtResult.Text = resultText;
        }

        protected bool isComValid(string fName)
        {
            bool retCode = false;
            Parameters parms;
            var reader = new AppSettingsReader();
            //string password = "OFu=m?ujriK[";
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();
            Application app = new Application();
            Package package = null;

            try
            {
                app.PackagePassword = password;
                package = app.LoadPackage(packagePath + "ERPValidateCOM.dtsx", null);
                parms = package.Parameters;
                parms["ParmBatchID"].Value = "";
                parms["ParmImportFileName"].Value = fName;
                parms["ParmWorkStationID"].Value = userName;
                parms["ParmWorkStationName"].Value = "myComputer";
                // Now run the package
                DTSExecResult results = package.Execute();
                if (results == Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure)
                {
                    foreach (Microsoft.SqlServer.Dts.Runtime.DtsError local_DtsError in package.Errors)
                    {
                        retCode = false;        // Error occurred, set returncode to false
                        resultText = resultText + "Package Execution results: " + local_DtsError.Description.ToString() + Environment.NewLine;
                    }
                }
                else
                {
                    resultText = resultText + "Process ran successfully." + Environment.NewLine + Environment.NewLine;
                    // Now read the database to discover the results
                    string sqlStr = "SELECT TOP 1 * FROM ProcessValidation WHERE Status <> 'Processing' AND SourceType = 'VALCOM' AND CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE) ORDER BY PackageStartDate DESC";
                    biz = new BizData();
                    DataTable configDT = new DataTable();
                    configDT = biz.getData(sqlStr);

                    if (configDT.Rows.Count > 0)
                    {
                        foreach (DataRow row in configDT.Rows)
                        {
                            if (row["Status"].ToString() == "Valid")
                            {
                                retCode = true;
                                resultText = "Validation results: Valid" + Environment.NewLine + "Press the 'Process' button to proceed.";
                            }
                            else
                            {
                                retCode = false;
                                resultText = "Validation results: " + row["Status"].ToString() + Environment.NewLine + "Validation Error: " + row["ValidationError"].ToString();
                            }
                        }
                    }
                    else
                    {
                        resultText = "Unable to read ProcessValidation Table for this file.";
                        retCode = false;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retCode;
        }


        protected void btnProcess_Click(object sender, EventArgs e)
        {
            string runType = "";
            //string batFile = "C:\\DevERPDataExports\\runEpaymentProgram.bat";
            //var reader = new AppSettingsReader();                                         //Jane, 2018-12-17: moved to runEPaymentProcess() for AIMS project.
            //var batFile = (reader.GetValue("batFile", typeof(string))).ToString();        //Jane, 2018-12-17: moved to runEPaymentProcess() for AIMS project.

            if (functionSelectedValue == "Update Posted Date in OCR")
            {
                biz = new BizData();
                DataTable dt = new DataTable();
                string sqlStr = "";

                sqlStr = "DECLARE @r nvarchar(250); exec UpdateInitiaonDateByTransconfirmIDByCashCode '" +
                    txtCashCode.Text + "', '" + txtPostDate.Text + "', '" + txtBeginSeq.Text + "', '" +
                    txtEndSeq.Text + "', @r OUT; SELECT @r AS 'Message';";

                dt = biz.getData(sqlStr);

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        resultText = row[col].ToString();
                    }
                }
            }
            else if (functionSelectedValue == "Create Elite File")
            {
                processExport(functionSelectedValue);
            }
            else if (functionSelectedValue == "Create Water File")
            {
                processOnline();
                processExport(functionSelectedValue);
            }
            else if (functionSelectedValue == "Create Parking Tickets File")
            {
                processOnline();
                processExport(functionSelectedValue);
            }
            else if (functionSelectedValue == "Create Tax File")
            {
                processOnline();
                processExport(functionSelectedValue);
            }
            else if (functionSelectedValue == "Create Dane Co Tax File")
            {
                processExport(functionSelectedValue);
            }
            else if (functionSelectedValue == "Send Parking File")
            {
                processUpload(functionSelectedValue);
            }
            else if (functionSelectedValue == "Create Municipal Court File")        //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
            {
                //processOnline();                      //Jane, 2019-11-28: Will be needed once Court Tyler Cashiering piece is in place.
                processExport(functionSelectedValue);
            }
            else
            {
                //string waterFiles = "None";
                // US Bank
                foreach (string filePath in actualFile)
                {
                    string tmpFileName = filePath.ToString();
                    int position = tmpFileName.LastIndexOf('\\');
                    string fileName = tmpFileName.Substring(position + 1);
                    string destFilePath = importFilePath + fileName;
                    File.Copy(filePath, destFilePath, true);
                    string tempePayDestPath = "";

                    if (fileName.Contains("DP5"))
                    {
                        txtResult.Text = "";
                        processImport("CDA", fileName);
                        processImport("Water", fileName);
                        processImport("Police", fileName);
                        processImport("Assessor", fileName);
                        processImport("ClerksLicense", fileName);
                        if (txtDCoPayDate.Text != "" && txtDCoPenDate.Text != "" && txtDCoTaxYear.Text != "")
                        {
                            processImport("DaneCo", fileName);
                        }
                        runType = "Remittance";
                    }
                    else if (fileName.Contains("COM_TX2"))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        processImport("Assessor", fileName);
                        runType = "Online";
                    }
                    else if (fileName.Contains("COM_MMT"))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        processImport("Metro", fileName);
                        runType = "Online";
                    }
                    else if (fileName.Contains("COM_TRE")) //Riki 8-11-15: New Parking Ticket File
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        runEPaymentProcess();                               //Jane, 2018-11-14: Added for AIMS project, SR-17-0256.
                        processImport("Police", fileName);                        
                        //Jane S, 2019-01-24: Per Dani F, the new Court SSIS package (ERPLoadMMCCOM.dtsx) will process only the COM_TRE file and **not** COM_CDA.
                        processImport("Court", fileName);						
                        runType = "Online";
                    }
                    else if (fileName.Contains("COM_CDA"))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        processImport("CDA", fileName);
                        // processImport("Police", fileName);               // Kim G 8/4/2017 - Commented out old call to Parking Ticket file
                        processImport("Ambulance", fileName);
                        processImport("Appliance", fileName);
                        processImport("Misc", fileName);
                        processImport("ParksGift", fileName);
                        processImport("NeighborhoodConf", fileName);
                        runType = "Online";
                    }
                    else if (fileName.Contains("COM_WAT"))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        //processImport("Water", waterFiles);
                        processImport("Water", fileName);
                        runType = "Online";

                        // kg - 12/2/2015 - Old code when water processed two files at once
                        //if (waterFiles == "None")
                        //{
                        //    waterFiles = fileName;
                        //}
                        //else
                        //{
                        //    waterFiles = waterFiles + "," + fileName;
                        //}
                    }
                    else if (fileName.Contains("a900.cty"))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);
                        //processImport("Water", waterFiles);
                        processImport("WaterBill", fileName);
                        runType = "Online";

                        //if (waterFiles == "None")
                        //{
                        //    waterFiles = fileName;
                        //}
                        //else
                        //{
                        //    waterFiles = waterFiles + "," + fileName;
                        //}
                    }
                    //else if (tmpFileName.Contains("MMP_REMITTANCE_"))   // Parking
                    else if (IsKubraFile(fileName))
                    {
                        tempePayDestPath = ePayDestPath + fileName;
                        File.Copy(filePath, tempePayDestPath, true);

                        if (fileName.ToUpper().Substring(0, 2) == "PU")
                        {
                            processImport("KubraParking", fileName);
                        }
                        else if (fileName.ToUpper().Substring(0, 2) == "TX")
                        {
                            processImport("KubraTax", fileName);
                        }
                        else if (fileName.ToUpper().Substring(0, 2) == "WU")
                        {
                            processImport("KubraWater", fileName);
                        }
                    }
                }
                if (runType == "Online")
                {
                    runEPaymentProcess();                       //Jane, 11/14/2018: AIMS project, SR-17-0256.

/***** Jane, 2018-11-14: For AIMS Project, SR-17-0256. Put code into a function, as called twice. ***************
                    resultText += "EPayment Process Is Running.";
                    if (File.Exists(batFile))
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();

                        proc.StartInfo.FileName = batFile;
                        proc.StartInfo.RedirectStandardError = false;
                        proc.StartInfo.RedirectStandardOutput = false;
                        proc.StartInfo.UseShellExecute = false;
                        proc.Start();
                        proc.WaitForExit();
                    }
****************************************************************************************************************/
                    //ScriptManager.RegisterStartupScript(lnkEPay, lnkEPay.GetType(), "text", "lnkEPay_Click()", true);
                }
                if (runType != "")
                {
                    string filePath = reportFilePath + "ProcessSummary" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
                    string filePath2 = "";

                    if (runType == "Online")
                    {
                        //This report was previously called OnlineProcessSummary; it has been renamed DepositWorksheet, because too many reports have "Process" and "Summary" in their name.
                        //filePath2 = reportFilePath + "OnlineProcessSummary" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".xls";
                        filePath2 = reportFilePath + "DepositWorksheet" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".xls";             //Added by ITJBS, 8/2/2017.
                        runOnlineSummaryReport(runType, filePath2);
                    }

                    runSummaryReport(runType, filePath);
                    sendEmailwAttach(runType, filePath, filePath2);
                }
            }
            lblResult.Visible = true;
            txtResult.Visible = true;
            txtResult.Text = resultText;
        }

        protected void processUpload(string uploadType)
        {
            try
            {
                switch (uploadType)
                {
                    case "Send Parking File":

                        var reader = new AppSettingsReader();
                        var batKUBRAUpload = (reader.GetValue("KUBRAUploadBatFile", typeof(string))).ToString();
                        if (File.Exists(batKUBRAUpload))
                        {
                            //// Begin old method
                            //System.Diagnostics.Process proc = new System.Diagnostics.Process();

                            //proc.StartInfo.FileName = batKUBRAUpload;
                            //proc.StartInfo.RedirectStandardError = false;
                            //proc.StartInfo.RedirectStandardOutput = false;
                            //proc.StartInfo.UseShellExecute = false;

                            ////// KG Added to help with not working at first
                            ////// http://stackoverflow.com/questions/5519328/executing-batch-file-in-c-sharp
                            ////int endByteDirectory = batKUBRAUpload.LastIndexOf("\\");
                            ////string wkDirectory = batKUBRAUpload.Substring(0, endByteDirectory);
                            ////proc.StartInfo.CreateNoWindow = true;
                            ////proc.StartInfo.WorkingDirectory = wkDirectory;
                            ////// end new code

                            //proc.Start();
                            //proc.WaitForExit();
                            //resultText = resultText + "Files uploaded successfully." + Environment.NewLine + Environment.NewLine;
                            //// End old method

                            ////// KG Added close method
                            ////proc.Close();


                            // KG Tried new method
                            int ExitCode;
                            ProcessStartInfo ProcessInfo;
                            Process process;

                            ProcessInfo = new ProcessStartInfo(batKUBRAUpload);
                            ProcessInfo.CreateNoWindow = true;
                            ProcessInfo.UseShellExecute = false;
                            int endByteDirectory = batKUBRAUpload.LastIndexOf("\\");
                            string wkDirectory = batKUBRAUpload.Substring(0, endByteDirectory);
                            ProcessInfo.WorkingDirectory = wkDirectory;
                            // *** Redirect the output ***
                            ProcessInfo.RedirectStandardError = true;
                            ProcessInfo.RedirectStandardOutput = true;

                            process = Process.Start(ProcessInfo);
                            process.WaitForExit();

                            // *** Read the streams ***
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            if(error == "")     // If no error occured,
                            {
                                // Now archive the files
                                Thread.Sleep(3000);     // Sleep for 3 seconds to be sure the upload is done
                                var ArchiveBatch = batKUBRAUpload.Replace("Upload", "UploadArchive");
                                System.Diagnostics.Process.Start(ArchiveBatch);
                                resultText = "File(s) Uploaded.";
                            }
                            else    // If an error occurred,
                            {
                                resultText = output + "Error: " + error;
                            }
                            ExitCode = process.ExitCode;

                            //MessageBox.Show("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
                            //MessageBox.Show("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
                            //MessageBox.Show("ExitCode: " + ExitCode.ToString(), "ExecuteCommand");
                            process.Close();
                            // End new method
                        }
                        else
                        {
                            resultText = resultText + "Files NOT uploaded.  Could not find batch file: " + batKUBRAUpload + Environment.NewLine + Environment.NewLine;
                        }
                        btnProcess.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                resultText = resultText + "Error during File upload.  Error: " + ex.Message + Environment.NewLine + Environment.NewLine;
                throw;
            }
        }

        protected void processOnline()
        {
            Application app = new Application();
            Package package = null;
            //package Microsoft.SqlServer.DTS.Runtime.DtsObject

            var reader = new AppSettingsReader();
            //string password = "OFu=m?ujriK[";
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();

            try
            {
                app.PackagePassword = password;

                if (functionSelectedValue == "Create Water File")
                {
                    package = app.LoadPackage(packagePath + "ERPLoadWTRONL.dtsx", null);
                }
                else if (functionSelectedValue == "Create Parking Tickets File")
                {
                    package = app.LoadPackage(packagePath + "ERPLoadPARONL.dtsx", null);
                }
                else if (functionSelectedValue == "Create Tax File")
                {
                    package = app.LoadPackage(packagePath + "ERPLoadPRPONL.dtsx", null);
                }
                else if (functionSelectedValue == "Create Municipal Court File")                //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
                {
                //Jane, 2019-01-24: Next line will get added after Tyler creates Municipal Court piece in Tyler Cashiering.
                    //package = app.LoadPackage(packagePath + "ERPLoadMMCONL.dtsx", null);      //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
                }
				
                resultText = resultText + "The process import table is complete." + Environment.NewLine + Environment.NewLine;
                DTSExecResult results = package.Execute();
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
        }

        protected void processExport(string type)
        {
            Application app = new Application();
            Package package = null;
            var reader = new AppSettingsReader();
            //string password = "OFu=m?ujriK[";
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();

            try
            {
                app.PackagePassword = password;

                if (type == "Create Elite File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportCDA.dtsx", null);
                }
                else if (type == "Create Parking Tickets File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportPAR.dtsx", null);
                }
                else if (type == "Create Water File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportWater.dtsx", null);
                }
                else if (type == "Create Tax File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportPRP.dtsx", null);
                }
                else if (type == "Create Dane Co Tax File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportDAN.dtsx", null);
                }
                else if (type == "Create Ambulance Billing File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportAMB.dtsx", null);
                }
                else if (type == "Create Appliance Sticker File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportSAS.dtsx", null);
                }
                else if (type == "Create Parks Gift File") //12-10-14 Riki - Added because Parks wants to get the file too now. See SR-14-0000474
                {
                    package = app.LoadPackage(packagePath + "ERPExportPGF.dtsx", null);
                }
                else if (type == "Create Olbrich File") //01-13-15 Riki - Added because Olbrich would like to receive a file in TRCommon\Olbrich
                {
                    package = app.LoadPackage(packagePath + "ERPExportOLB.dtsx", null);
                }
                else if (type == "Create Municipal Court File") //Jane S, 2019-01-24: Added for TiPSS project, SR-17-0375.
                {
                    package = app.LoadPackage(packagePath + "ERPExportMMC.dtsx", null);
                }                
				else if (type == "Create Kubra Parking File")
                {
                    package = app.LoadPackage(packagePath + "ERPExportPKU.dtsx", null);
                }

                resultText = resultText + "The process " + functionSelectedValue + " is complete." + Environment.NewLine + Environment.NewLine;
                DTSExecResult results = package.Execute();
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
        }

        protected void processImport(string group, string fileName)
        {
            Application app = new Application();
            Package package = null;
            Parameters parms;
            //            string password = "OFu=m?ujriK[";

            try
            {
                //string OCRAccela = "C:\\DevERPDataExports\\OCRAccela.bat"; // RIKI#2
                var reader = new AppSettingsReader();
                string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();
                var OCRAccela = (reader.GetValue("OCRAccela", typeof(string))).ToString();
                app.PackagePassword = password;

                if (functionSelectedValue == "Load OCR File")
                {
                    if (group == "CDA")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadCDADP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Water")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadWtrDP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Police")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPARDP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "ClerksLicense")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadCLLDP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                        if (File.Exists(OCRAccela)) // RIKI#2 
                        {
                            System.Diagnostics.Process procOCRAccela = new System.Diagnostics.Process();

                            procOCRAccela.StartInfo.FileName = OCRAccela;
                            procOCRAccela.StartInfo.RedirectStandardError = false;
                            procOCRAccela.StartInfo.RedirectStandardOutput = false;
                            procOCRAccela.Start();
                            procOCRAccela.WaitForExit();
                        }
                    }
                    else if (group == "Assessor")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPRPDP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "DaneCo")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadDANDP5.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                        parms["PaymentDate"].Value = Convert.ToDateTime(txtDCoPayDate.Text);
                        parms["PenDate"].Value = Convert.ToDateTime(txtDCoPenDate.Text);
                        parms["TaxYear"].Value = txtDCoTaxYear.Text;
                    }
                }
                //else if (functionSelectedValue == "Load US Bank File")
                else if (functionSelectedValue == "Load Online Files")
                {
                    if (group == "CDA")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadCDACOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Water")
                    {
                        // 12/2/2015 - KG Split ERPLoadWTRCom.dtsx into two packages
                        package = app.LoadPackage(packagePath + "ERPLoadWTRCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileNameXML"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "WaterBill")
                    {
                        // 12/2/2015 - KG Split ERPLoadWTRCom.dtsx into two packages
                        package = app.LoadPackage(packagePath + "ERPLoadWTRBIL.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileNameCon"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Police")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPARCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Ambulance")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadAMBCom.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Appliance")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadSASCom.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Metro")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadMETCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    //Jane S, 2019-01-24: Added Court for TiPSS Project, SR-17-0375.
                    else if (group == "Court")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadMMCCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Misc")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadMSCCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "ParksGift")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPGFCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "NeighborhoodConf")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadNCRCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "Assessor")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPRPCOM.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "KubraParking")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPKUKUB.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    else if (group == "KubraTax")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadPRPKUB.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                    // 2017-10-12, ITJBS: Added KubraWater for new ERPLoadWTRKUB package.
                    else if (group == "KubraWater")
                    {
                        package = app.LoadPackage(packagePath + "ERPLoadWTRKUB.dtsx", null);
                        parms = package.Parameters;
                        parms["ParmBatchID"].Value = "";
                        parms["ParmImportFileName"].Value = fileName;
                        parms["ParmWorkStationID"].Value = userName;
                        parms["ParmWorkStationName"].Value = "myComputer";
                    }
                }
                else
                {
                    package = app.LoadPackage(packagePath + "NoFile.dtsx", null);
                }

                // Run Package code follows
                resultText = resultText + "The process for " + group + " file " + fileName + " is complete." + Environment.NewLine + Environment.NewLine;
                DTSExecResult results = package.Execute();
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

                // Run additional Package if necessary
                if (group == "Appliance")
                {
                    processExport("Create Appliance Sticker File");
                }
                else if (group == "Ambulance")
                {
                    processExport("Create Ambulance Billing File");
                }
                else if (group == "ParksGift") //12-10-14 Riki - Added because Parks wants to get the file too now. See SR-14-0000474
                {
                    processExport("Create Parks Gift File");
                }
                else if (group == "Misc") //01-13-15 Riki - Added because Olbrich would like to receive a file in TRCommon\Olbrich
                {
                    processExport("Create Olbrich File");
                }
                else if (group == "KubraParking")
                {
                    processExport("Create Kubra Parking File");
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
        }

        protected void runSummaryReport(string type, string filePath)
        {
            var reader = new AppSettingsReader();
            var LDAPsvcAcct = (reader.GetValue("LDAPsvcAcct", typeof(string))).ToString();
            var LDAPsvcPass = (reader.GetValue("LDAPsvcPass", typeof(string))).ToString();

            ReportExecutionService reportServClient = new ReportExecutionService();
            //reportServClient.Credentials = new System.Net.NetworkCredential("CITY\\Bizcash", "4u2loginbts1!");
            reportServClient.Credentials = new System.Net.NetworkCredential(LDAPsvcAcct, LDAPsvcPass);

            ExecutionHeader execHeader = new ExecutionHeader();
            TrustedUserHeader userHeader = new TrustedUserHeader();
            ServerInfoHeader serviceInfo = new ServerInfoHeader();
            ExecutionInfo execInfo = new ExecutionInfo();

            byte[] result = null;
            string reportPath = processSummaryReport;
            string format = "pdf";
            string historyID = null;
            string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";
            string extension;
            string mimetype;
            string encoding;
            string[] streamIds = null;

            Warning[] warnings = new Warning[1];
            warnings[0] = new Warning();

            ParameterValue[] parameters = new ParameterValue[4];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "DateType";
            parameters[0].Value = "Process";
            parameters[1] = new ParameterValue();
            parameters[1].Name = "BeginDate";
            parameters[1].Value = DateTime.Today.ToString("MM/dd/yyyy");
            parameters[2] = new ParameterValue();
            parameters[2].Name = "EndDate";
            parameters[2].Value = DateTime.Today.ToString("MM/dd/yyyy");
            parameters[3] = new ParameterValue();
            parameters[3].Name = "SourceType";
            parameters[3].Value = type;

            reportServClient.ExecutionHeaderValue = execHeader;

            execInfo = reportServClient.LoadReport(reportPath, historyID);

            reportServClient.SetExecutionParameters(parameters, "en-us");

            result = reportServClient.Render(format, devInfo, out extension, out encoding, out mimetype
                , out warnings, out streamIds);

            execInfo = reportServClient.GetExecutionInfo();

            FileStream stream = File.OpenWrite(filePath);

            stream.Write(result, 0, result.Length);
            stream.Close();
        }

        protected void runOnlineSummaryReport(string type, string filePath)
        {
            var reader = new AppSettingsReader();
            var LDAPsvcAcct = (reader.GetValue("LDAPsvcAcct", typeof(string))).ToString();
            var LDAPsvcPass = (reader.GetValue("LDAPsvcPass", typeof(string))).ToString();

            ReportExecutionService reportServClient = new ReportExecutionService();
            //reportServClient.Credentials = new System.Net.NetworkCredential("CITY\\Bizcash", "4u2loginbts1!");
            reportServClient.Credentials = new System.Net.NetworkCredential(LDAPsvcAcct, LDAPsvcPass);

            ExecutionHeader execHeader = new ExecutionHeader();
            TrustedUserHeader userHeader = new TrustedUserHeader();
            ServerInfoHeader serviceInfo = new ServerInfoHeader();
            ExecutionInfo execInfo = new ExecutionInfo();

            byte[] result = null;
            string reportPath = onlineProcessSummaryReport;
            string format = "excel";
            string historyID = null;
            string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";
            string extension;
            string mimetype;
            string encoding;
            string[] streamIds = null;

            Warning[] warnings = new Warning[1];
            warnings[0] = new Warning();

            ParameterValue[] parameters = new ParameterValue[4];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "DateType";
            parameters[0].Value = "Process";
            parameters[1] = new ParameterValue();
            parameters[1].Name = "BeginDate";
            parameters[1].Value = DateTime.Today.ToString("MM/dd/yyyy");
            parameters[2] = new ParameterValue();
            parameters[2].Name = "EndDate";
            parameters[2].Value = DateTime.Today.ToString("MM/dd/yyyy");
            parameters[3] = new ParameterValue();
            parameters[3].Name = "SourceType";
            parameters[3].Value = type;

            reportServClient.ExecutionHeaderValue = execHeader;

            execInfo = reportServClient.LoadReport(reportPath, historyID);

            reportServClient.SetExecutionParameters(parameters, "en-us");

            result = reportServClient.Render(format, devInfo, out extension, out encoding, out mimetype
                , out warnings, out streamIds);

            execInfo = reportServClient.GetExecutionInfo();

            FileStream stream = File.OpenWrite(filePath);

            stream.Write(result, 0, result.Length);
            stream.Close();
        }

        protected void sendEmailwAttach(string type, string filePath, string filePath2)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient();

            biz = new BizData();
            DataTable emailDT = new DataTable();
            string sqlStr = "";

            sqlStr = "exec GetProcessSummaryEmail " + type;
            emailDT = biz.getData(sqlStr);

            if (emailDT.Rows.Count > 0)
            {
                foreach (DataRow row in emailDT.Rows)
                {
                    foreach (DataColumn col in emailDT.Columns)
                    {
                        mail.To.Add(row[col].ToString());
                    }
                }

                mail.Subject = type + " Process Summary";
                mail.Body = "The attached report lists the " + type + " payments that were just processed";

                System.Net.Mail.Attachment attachment;
                // Add 1st attachment (old report)
                attachment = new System.Net.Mail.Attachment(filePath);
                mail.Attachments.Add(attachment);

                // Add 2nd attachment (new report)
                if (filePath2 != "")
                {
                    attachment = new System.Net.Mail.Attachment(filePath2);
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Send(mail);
            }
        }

        protected string getKUBRAUploadfiles()
        {
            var _getKUBRAUploadfiles = "<ol>"; // "<ul style='list-style-type:none'>";

            files = new string[] { };
            files = Directory.GetFiles(KUBRAUploadfiles);
            foreach (string file in files)
            {
                _getKUBRAUploadfiles += "<li>" + file + "</li>";
            }
            if (_getKUBRAUploadfiles == "<ol>")
            {
                _getKUBRAUploadfiles = "";
            }
            else
            {
                _getKUBRAUploadfiles += "</ol>"; // "</ul>";
            }
            return _getKUBRAUploadfiles;
        }

        protected void cldFileDate_SelectionChanged(object sender, EventArgs e)
        {
            bool isKubraFile = false;
            lstFiles.Items.Clear();
            actualFile.Clear();
            btnProcess.Visible = true;
            txtResult.Text = "";

            files = new string[] { };
            files = Directory.GetFiles(folder);

            if ((functionSelectedValue == "Load OCR File") || (functionSelectedValue == "Load Online Files"))   // KG Added 12/29/2016 to support Validation
            {
                btnProcess.Enabled = false;
            }
            foreach (string file in files)
            {
                string lastModifiedDate = File.GetLastWriteTime(file).ToString("MM/dd/yyyy");

                string tmpFileName = file.ToString();
                int position = tmpFileName.LastIndexOf('\\');
                string fileName = tmpFileName.Substring(position + 1);

                isKubraFile = IsKubraFile(fileName);    // Figure out if this is a Kubra file based on the name
/******************************************************************************************************************* 
*   Code modified per SR-16-0000321. No longer using date in file name as basis for TR Web calendar date; 
*   now using file last modified date, like the US Bank files, per Dani Fossum. 
*   Note: Need to keep the IsKubraFile function for other reasons. - Jane S., 10/25/2016
******************************************************************************************************************/
                //*** Start comment-out by Jane S, 10/25/2016 ***
                //if (isKubraFile)                                  // If this is a Kubra file, get the date from the name of file
                //{ 
                //    lastModifiedDate = fileName.Substring(4,2) + "/" 
                //            + fileName.Substring(6,2) + "/20"  
                //            + fileName.Substring(2,2)           ;  // Use the file name for the last modified date
                //}
                //*** End comment-out by Jane S, 10/25/2016 ***

                //if (fileName.ToUpper().Right(4) == ".TXT")          // Look for KUBRA Files to be identified by filename
                //{
                //    isKubraFile = false;
                //    if ((fileName.Substring(0, 2).ToUpper() == "TX") ||     // If this is a KUBRA Preface
                //    (fileName.Substring(0, 2).ToUpper() == "PU"))
                //    {
                //        isKubraFile = true;
                //        lastModifiedDate = fileName.Substring(4,2) + "/" 
                //            + fileName.Substring(6,2) + "/20"  
                //            + fileName.Substring(2,2)           ;  // Use the file name for the last modified date
                //    }
                //}

                if (lastModifiedDate.Equals(cldFileDate.SelectedDate.ToString("MM/dd/yyyy")))
                {
                    if (functionSelectedValue == "Load OCR File")
                    {
                        //string tmpFileName = file.ToString();
                        if (tmpFileName.Contains("DP5"))
                        {
                            //int position = tmpFileName.LastIndexOf('\\');
                            //string fileName = tmpFileName.Substring(position + 1);
                            lstFiles.Items.Add(fileName);
                            actualFile.Add(file.ToString());
                        }
                    }
                    //else if (functionSelectedValue == "Load US Bank File")
                    else if (functionSelectedValue == "Load Online Files")
                    {
                        //string tmpFileName = file.ToString();
                        if (isKubraFile
                            || tmpFileName.Contains("COM")
                            || tmpFileName.Contains("a900.cty"))
                        {
                            //int position = tmpFileName.LastIndexOf('\\');
                            //string fileName = tmpFileName.Substring(position + 1);
                            lstFiles.Items.Add(fileName);
                            actualFile.Add(file.ToString());
                        }
                    }
                }
            }
        }

        public bool IsKubraFile(string FileName)    // Evaluates a filename and returns true if it is a Kubra file convention
        {
            bool ReturnValue = false;

            if (FileName.ToUpper().Right(4) == ".TXT")          // Look for KUBRA Files to be identified by filename
            {
                if ((FileName.Substring(0, 2).ToUpper() == "TX") ||     // If this is a KUBRA Preface
                    (FileName.Substring(0, 2).ToUpper() == "WU") ||      //Added Water Utility file. - ITJBS, 10/12/2017.
                    (FileName.Substring(0, 2).ToUpper() == "PU"))

                {
                    ReturnValue = true;
                }
            }
            return ReturnValue;
        }

        /********** Jane, 2018-11-14: Created function for AIMS project, SR-17-0256, though code was previously in btnProcess_Click() **********/
        public void runEPaymentProcess()
        {
            var reader = new AppSettingsReader();
            var batFile = (reader.GetValue("batFile", typeof(string))).ToString();

            resultText += "EPayment Process Is Running. ";
            if (File.Exists(batFile))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();

                proc.StartInfo.FileName = batFile;
                proc.StartInfo.RedirectStandardError = false;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
            }
        }

        public void importFileCleanup()
        {

            //return; // KG TEMP

            string cleanupFolder = importFilePath;
            string[] cleanupFiles = Directory.GetFiles(cleanupFolder);
            DateTime cutoffDate = DateTime.Now.AddDays(-10);

            foreach (string file in cleanupFiles)
            {
                DateTime lastModifiedDate = File.GetLastWriteTime(file);

                if (lastModifiedDate <= cutoffDate)
                {
                    File.Delete(file);
                }
            }
        }

        public void logFileCleanup()
        {
            string cleanupFolder = logFilePath;
            string[] cleanupFiles = Directory.GetFiles(cleanupFolder);
            DateTime cutoffDate = DateTime.Now.AddDays(-31);                    //Jane S, 2/9/2018: Update from -10 to -31 to keep log files longer.

            foreach (string file in cleanupFiles)
            {
                DateTime lastModifiedDate = File.GetLastWriteTime(file);

                if (lastModifiedDate <= cutoffDate)
                {
                    File.Delete(file);
                }
            }
        }

        public void reportFileCleanup()
        {
            string cleanupFolder = reportFilePath;
            string[] cleanupFiles = Directory.GetFiles(cleanupFolder);
            DateTime cutoffDate = DateTime.Now.AddDays(-10);

            foreach (string file in cleanupFiles)
            {
                DateTime lastModifiedDate = File.GetLastWriteTime(file);

                if (lastModifiedDate <= cutoffDate)
                {
                    File.Delete(file);
                }
            }
        }

        public void setScreen()
        {
            actualFile = new ArrayList();
            if (functionSelectedValue.Equals("Update Posted Date in OCR"))
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Enter the Work Source Code, Posted Date, Sequence Number Begin and Sequence Number End.  " +
                    "To update one sequence leave Sequence Number End set to 000000.  Click Process to begin update.";
                tblFileNames.Visible = true;
                btnProcess.Visible = true;
                btnProcess.Enabled = false;     // KG Added for validation step
                btnValidate.Visible = true;     // KG Added for validation step
                lblCashCode.Visible = true;
                txtCashCode.Visible = true;
                lblPostDate.Visible = true;
                txtPostDate.Visible = true;
                lblBeginSeq.Visible = true;
                txtBeginSeq.Visible = true;
                lblEndSeq.Visible = true;
                txtEndSeq.Visible = true;

                txtEndSeq.Text = "000000";
            }
            else if (functionSelectedValue.Equals("Load OCR File"))
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Select a date to process.  Ensure the appropriate file(s) to be processed are listed.  " +
                    "Then click Process to process the OCR file(s)." + Environment.NewLine + "The 3 Dane Co. fields need " +
                    "to be filled in only when processing Dane Co. Taxes otherwise leave blank.";
                tblFileNames.Visible = true;
                lblSelectDate.Visible = true;
                cldFileDate.Visible = true;
                lblFiles.Visible = true;
                lstFiles.Visible = true;
                btnProcess.Visible = true;
                btnProcess.Enabled = false;     // KG Added for validation step
                btnValidate.Visible = true;     // KG Added for validation step
                lblDCoPayDate.Visible = true;
                txtDCoPayDate.Visible = true;
                lblDCoPenDate.Visible = true;
                txtDCoPenDate.Visible = true;
                lblDCoTaxYear.Visible = true;
                txtDCoTaxYear.Visible = true;
                lblTotal.Visible = true;
                tbTotal.Visible = true;
                lblCount.Visible = true;
                tbCount.Visible = true;
            }
            // else if (functionSelectedValue.Equals("Load US Bank File"))
            else if (functionSelectedValue.Equals("Load Online Files"))
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Select a date to process.  Ensure the appropriate file(s) to be processed are listed.  " +
                    "Then click Process to process the US Bank file(s).";
                lblSelectDate.Visible = true;
                cldFileDate.Visible = true;
                tblFileNames.Visible = true;
                lblFiles.Visible = true;
                lstFiles.Visible = true;
                btnProcess.Visible = true;
                btnProcess.Enabled = false;     // KG Added for validation step
                btnValidate.Visible = true;     // KG Added for validation step
            }
            else if (functionSelectedValue.Equals("Send Parking File"))
            {
                btnRefresh.Visible = true;
                var displayVal = getKUBRAUploadfiles();
                lblDirection.Visible = true;
                if (displayVal == "")
                {
                    lblDirection.Text = "No files exist to upload.  <br />To upload files, copy to this location and click Refresh: " + KUBRAUploadfiles;
                    btnProcess.Enabled = false;
                }
                else
                {
                    lblDirection.Text = "The following files will be sent (uploaded) to KUBRA<br />" + displayVal;
                    btnProcess.Enabled = true;
                }
            }
            else
            {
                lblDirection.Visible = true;
                lblDirection.Text = "Click Process to test the email functionality.";
                btnProcess.Visible = true;
            }
        }

        public void resetPage()
        {
            lblDirection.Visible = false;

            lblResult.Visible = false;
            txtResult.Visible = false;
            txtResult.Text = "";

            lblSelectDate.Visible = false;
            cldFileDate.Visible = false;

            lblDCoPayDate.Visible = false;
            txtDCoPayDate.Visible = false;
            lblDCoPenDate.Visible = false;
            txtDCoPenDate.Visible = false;
            lblDCoTaxYear.Visible = false;
            txtDCoTaxYear.Visible = false;

            txtDCoPayDate.Text = "";
            txtDCoPenDate.Text = "";
            txtDCoTaxYear.Text = "";

            lblCashCode.Visible = false;
            txtCashCode.Visible = false;
            lblPostDate.Visible = false;
            txtPostDate.Visible = false;
            lblBeginSeq.Visible = false;
            txtBeginSeq.Visible = false;
            lblEndSeq.Visible = false;
            txtEndSeq.Visible = false;

            txtCashCode.Text = "";
            txtPostDate.Text = "";
            txtBeginSeq.Text = "";
            txtEndSeq.Text = "";

            tblFileNames.Visible = false;
            lblFiles.Visible = false;
            lstFiles.Visible = false;
            btnProcess.Enabled = true;      // KG Added 11/29/2016 to support validate functionality
            btnProcess.Visible = false;
            btnValidate.Visible = false;    // KG Added 11/29/2016 to support validate functionality
            lnkEPay.Visible = false;
            btnRefresh.Visible = false;

            lblTotal.Visible = false;
            tbTotal.Visible = false;
            lblCount.Visible = false;
            tbCount.Visible = false;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            resetPage();
            setScreen();
            btnProcess.Visible = true;
        }

    }


    static class Extensions
    {
        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}
