using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AITResearch.Repositories
{
    /// <summary>
    /// repository to handle survey result views.
    /// </summary>
    public class ViewRepo
    {
        private readonly string _connectionString;


        public ViewRepo()
        {
            //set connection string
            if (ConfigurationManager.ConnectionStrings["DevelopmentConnectionString"].ConnectionString.Equals("Dev"))
            {
                _connectionString = AppConstant.AppConnection.DevConnection;
            }
            else if (ConfigurationManager.ConnectionStrings["DevelopmentConnectionString"].ConnectionString.Equals("Test"))
            {
                _connectionString = AppConstant.AppConnection.TestConnection;
            }
            else { _connectionString = AppConstant.AppConnection.DevConnection; }
        }

        /// <summary>
        /// get survey answers using a stored procedure and return them in data table
        /// </summary>
        /// <returns>containing pivoted survey result</returns>
        public DataTable GetSurveyResultsPivot()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetSurveyResultsPivot, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while retrieving pivoted survey results.", sqlex);
            }
            catch (InvalidOperationException ioex)
            {
                throw new Exception("Invalid operation error occurred while retrieving pivoted survey results.", ioex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while retrieving pivoted survey results.", ex);
            }

            return dt;
        }
    }
}