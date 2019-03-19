using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace TRWeb
{
    public class BizData
    {
        private SqlCommand myCommand;
        private SqlConnection myConnection;

        private void initializeConnection() 
        {
            var reader = new AppSettingsReader();
            var env = (reader.GetValue("env", typeof(string))).ToString();

            string wcConnStr = "";
            if (env.Equals("Prod"))
            {
                wcConnStr = "ProdBizcashConnStr";
            }
            else if (env.Equals("Test"))
            {
                wcConnStr = "TestBizcashConnStr";
            }
            else
            {
                wcConnStr = "DevBizcashConnStr";
            }

            string connStr = ConfigurationManager.ConnectionStrings[wcConnStr].ToString();
            myConnection = new SqlConnection(connStr);
            myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.Connection.Open();
        }

        public DataTable getData(string sqlString)
        {

            initializeConnection();
            DataTable dt = new DataTable();

            try
            {

                SqlDataAdapter da = new SqlDataAdapter(sqlString, myConnection);
                DataSet ds = new DataSet();

                da.SelectCommand.CommandText = sqlString;
                da.SelectCommand.ExecuteNonQuery();
                    
                da.Fill(ds, "Table");
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                terminateConnection();
            }
            return dt;
        }

        public string updateData(string sqlString)
        {

            initializeConnection();

            string message = "";

            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(sqlString, myConnection);

                da.UpdateCommand = cmd;
                da.UpdateCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            finally
            {
                if (message == "")
                {
                    message = "Update Successful";
                }
                terminateConnection();
            }
            return message;
        }

        public string insertData(string sqlString)
        {

            initializeConnection();

            string message = "";

            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(sqlString, myConnection);

                da.InsertCommand = cmd;
                da.InsertCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            finally
            {
                if (message == "")
                {
                    message = "Update Successful";
                }
                terminateConnection();
            }
            return message;
        }

        public string deleteData(string sqlString)
        {

            initializeConnection();

            string message = "";

            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(sqlString, myConnection);

                da.DeleteCommand = cmd;
                da.DeleteCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            finally
            {
                if (message == "")
                {
                    message = "Update Successful";
                }
                terminateConnection();
            }
            return message;
        }

        private void terminateConnection()
        {
            if (myConnection.State == ConnectionState.Open)
            {
                myConnection.Close();
                myConnection.Dispose();
            }
        }
    }
}