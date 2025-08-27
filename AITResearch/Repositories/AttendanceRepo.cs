using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AITResearch.Repositories
{
    /// <summary>
    /// Repository to manage respondent's attendance records.
    /// Handles database operations related to Attendance table.
    /// </summary>
    public class AttendanceRepo
    {
        private readonly string _connectionString;

        ///set connection string
        public AttendanceRepo()
        {
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
        /// Inserts a new attendance record for a given respondent.
        /// </summary>
        /// <param name="respondentId">Unique identifier for each respondent</param>
        /// <param name="ipAddress">IP address from which the attendance was logged</param>
        public void CreateAttendance(int respondentId, string ipAddress)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.InsertAttendance, conn))
                {
                    conn.Open();
                    //add parameter value
                    cmd.Parameters.AddWithValue("@RespondentID", respondentId);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }   
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to create attendance record in the database.", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateAttendance: " + ex.Message);
            }
        }

    }
}