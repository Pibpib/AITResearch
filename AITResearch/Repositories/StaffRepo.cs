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
    /// repository for managing Staff database
    /// </summary>
    public class StaffRepo
    {
        private readonly string _connectionString;

        public StaffRepo()
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
        /// get staff record by username and password
        /// </summary>
        /// <param name="username">staff username</param>
        /// <param name="password">staff password</param>
        /// <returns>staff object if found or null</returns>
        /// <exception cref="Exception"></exception>
        public Staff GetStaffByUsernameAndPassword(string username, string password)
        {
            Staff staff = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetStaffByUsernameAndPassword, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                staff = new Staff
                                {
                                    StaffID = Convert.ToInt32(reader["StaffID"]),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Role = reader["Role"].ToString()
                                };
                            }
                            catch (InvalidCastException icex)
                            {
                                throw new Exception("Data type conversion error while reading staff data.", icex);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to retrieve staff record in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while retrieving staff.", ex);
            }

            return staff;
        }

        /// <summary>
        /// insert a new staff record
        /// </summary>
        /// <param name="username">staff username</param>
        /// <param name="password">staff password</param>
        /// <param name="role">staff role (default: Staff)</param>
        /// <returns>True if insertion was successful or false if failed</returns>
        public bool InsertStaff(string username, string password, string role = "Staff")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.InsertStaff, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", role);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to insert staff in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while creating staff account.", ex);
            }
        }

        /// <summary>
        /// check whether a username is already taken
        /// </summary>
        /// <param name="username">username to check</param>
        /// <returns>True if username exists or false if failed</returns>
        /// <exception cref="Exception"></exception>
        public bool IsUsernameTaken(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.IsUsernameTaken, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to check whether a username is already taken in the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while checking username.", ex);
            }
        }
    }
}