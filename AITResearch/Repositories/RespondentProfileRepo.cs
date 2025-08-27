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
    /// repository to manage operations on the RespondentProfile table.
    /// handles saving and updating respondent profile information in the database.
    /// </summary>
    public class RespondentProfileRepo
    {
        private readonly string _connectionString;

        public RespondentProfileRepo()
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
        /// update existing respondent profile in database
        /// </summary>
        /// <param name="profile">respondent profile with updated values</param>
        /// <returns>true if the update succeeded or  false if failed</returns>
        public bool UpdateRespondentProfile(RespondentProfile profile)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.UpdateRespondentProfile, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RespondentID", profile.RespondentID);
                    cmd.Parameters.AddWithValue("@FirstName", profile.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", profile.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DOB", profile.DOB);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to update respondent profile in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateRespondentProfile: " + ex.Message);
            }
        }

        public int? GetRespondentAgeRangeChoice(int respondentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetRespondentAgeRangeChoice, conn))
                {
                    cmd.Parameters.AddWithValue("@RespID", respondentId);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int choiceId))
                    {
                        return choiceId;
                    }
                    return null;
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to get respondent age range choice in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRespondentAgeRangeChoice: " + ex.Message);
            }
        }
    }
}