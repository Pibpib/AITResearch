using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AITResearch.Repositories
{
    /// <summary>
    /// repository class to manage Respondent data and related operations.
    /// </summary>
    public class RespondentRepo
    {
        private readonly string _connectionString;

        public RespondentRepo()
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
        /// add respondent to sql
        /// </summary>
        /// <returns>ID of the newly created respondent</returns>
        public int CreateRespondent()
        {
            try
            {
                int respondentId = 0;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.CreateRespondent, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@IsAnonymous", true);
                    respondentId = (int)cmd.ExecuteScalar();
                }
                
                return respondentId;
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to create respondent in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateRespondent: " + ex.Message);
            }
        }

        /// <summary>
        /// update respondent to not anonymous for registering 
        /// </summary>
        /// <param name="respondentId">ID of the respondent to update</param>
        /// <returns>True if the update was successful, false if failed</returns>
        public bool UpdateRespondentToNotAnonymous(int respondentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.UpdateRespondentToNotAnonymous, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RespondentID", respondentId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to update respondent's anonymous status in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateRespondentToNotAnonymous: " + ex.Message);
            }
        }
    }
}