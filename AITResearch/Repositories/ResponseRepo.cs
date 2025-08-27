using AITResearch.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AITResearch.Repositories
{
    /// <summary>
    /// repository for saving respondent response
    /// </summary>
    public class ResponseRepo
    {
        private readonly string _connectionString;

        public ResponseRepo()
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

        //save respondent's answer of the question id to sql
        public void SaveResponse(Response response)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.SaveResponse, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RespondentID", response.RespondentID);
                    cmd.Parameters.AddWithValue("@QuestionID", response.QuestionID);
                    cmd.Parameters.AddWithValue("@AnswerValue", (object)response.AnswerValue ?? DBNull.Value);

                    int affected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Rows affected: " + affected);
                    }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to save response in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveResponse: " + ex.Message);
            }
        }
    }
}
