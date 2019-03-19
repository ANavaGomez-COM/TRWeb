using System;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Configuration;



namespace TRWeb
{
    public partial class WaterAdmin : System.Web.UI.Page
    {
        private string resultText = string.Empty;
        private string packagePath = "";
        private string importFilePath = "";
        private string reportFilePath = "";
        private string logFilePath = "";
        private string errorMessage = "";
        private BizData biz = new BizData();
        protected void Page_Load(object sender, EventArgs e)
        {
            biz = new BizData();
            DataTable configDT = new DataTable();
            string sqlStr = "";

            //btnProcess.Attributes.Add("onclick", " this.disabled = true; " +
            //    ClientScript.GetPostBackEventReference(btnProcess, null) + ";");

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

                        if (row[0].ToString().Equals("reportFilePath"))
                        {
                            reportFilePath = row[1].ToString();
                        }
                        if (row[0].ToString().Equals("logFilePath"))
                        {
                            logFilePath = row[1].ToString();
                        }

                    }
                }
            }
            else
            {
                errorMessage = "The environment variables were not found.";
            }
            if (Upload.HasFile == false)
            {
               // UploadButton.Enabled = false;
                RunImport.Enabled = false;
            }

        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (Upload.HasFile == false)
            {
                // No file uploaded!
                UploadDetails.Text = "Please first select a file to upload...";
                lblSSIS.Text = "";
                lblUpload.Text = "";
            }
            else
            {
                string fileExt =  System.IO.Path.GetExtension(Upload.FileName);
             

                lblSSIS.Text = "";
                lblUpload.Text = "";
                UploadDetails.Text = "";
                if (fileExt == ".xls" || fileExt == ".xlsx" ) 
                {
                    // Save the file
                    string filePath = importFilePath;
                    //Upload.SaveAs(filePath + Upload.FileName);
                    Upload.SaveAs(filePath +"CustAcct_Import"+fileExt);
                    //Display user freindly message of file stats
                    int RowCounter = 0;
                    RowCounter = System.IO.File.ReadAllLines(filePath + "CustAcct_Import"+fileExt).Length - 1;
                    UploadDetails.Text = string.Format(
                       @"Uploaded file: {0}<br />
                       File size (in bytes): {1:N0}<br />
                       Content-type: {2:N0}",

                         Upload.FileName,
                         Upload.FileBytes.Length,
                         Upload.PostedFile.ContentType);



                    if (RowCounter != 0)
                    {

                        RunImport.Enabled = true;
                    }
                }
                else
                {
                    lblUpload.Text = "Please select only an Excel(.xls or .xlsx) file to upload.";
                }
                
            }
        }



 

        protected void RunImport_Click(object sender, EventArgs e)
        {
           
            Application app = new Application();
            Package package = null;
            var reader = new AppSettingsReader();
            string password = (reader.GetValue("ssisPwd", typeof(string))).ToString();
           // lblSSIS.Text = password;
            if (UploadDetails.Text == "" || UploadDetails.Text == "Please first select a file to upload...")
            {
                // No file uploaded!
                lblSSIS.Text = "Please first select a file to upload...";
                return;
            }
            else
                lblSSIS.Text = "Import Starting....";

            try
            {
                lblSSIS.Text = "Import Starting....";

                app.PackagePassword = password;

                //package = app.LoadPackage(@"\\ERPBIZTALKTEST\\DevERPDataExports\\ERPDataExports\\ERPLoadWTRACCT.dtsx", null); ///for Excel
               
                package = app.LoadPackage(packagePath + "ERPLoadWTRACCT.dtsx", null);
                //package = app.LoadPackage(@"\\ERPBIZTALKTEST\\DevERPDataExports\\ERPDataExports\\ERPLoadWTRACCTText.dtsx", null); ///for Text

                resultText = resultText + "The process import Water Accounts is complete." + Environment.NewLine + Environment.NewLine;
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
                UploadDetails.Text = "";

                lblSSIS.Text = resultText;// "Process ran successfully.  You will be emailed the result shortly.";

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
        
    }
}