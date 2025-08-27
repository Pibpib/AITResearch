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
    /// Repository to handle retrieval of question choices from the database.
    /// </summary>
    public class ChoiceRepo
    {
        private readonly string _connectionString;

        //set connection string
        public ChoiceRepo()
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
        /// Retrieves a list of choices for a given question.
        /// </summary>
        /// <param name="questionId">identifier for question.</param>
        /// <returns>List of <see cref="Choice"/> objects associated with the question.</returns>
        public List<Choice> GetChoices(int questionId)
        {
            List<Choice> choices = new List<Choice>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetChoicesByQuestionId, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@QuestionID", questionId);

                    //execute query and read results
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //get data to Choice object and add to list
                        choices.Add(new Choice
                        {
                            ChoiceID = (int)reader["ChoiceID"],
                            QuestionID = (int)reader["QuestionID"],
                            ChoiceLabel = reader["ChoiceLabel"].ToString(),
                            DisplayOrder = (int)reader["DisplayOrder"]
                        });
                    }
                    reader.Close();
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to retrieve choices from the database.", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetChoices: " + ex.Message);
            }
            return choices;
        }

        /// <summary>
        /// get age range by choice id
        /// </summary>
        /// <param name="choiceId">identifier for the choice</param>
        /// <returns>number of min age and max age</returns>
        public (int MinAge, int? MaxAge)? GetAgeRangeByChoice(int choiceId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetAgeRangeByChoice, conn);
                    cmd.Parameters.AddWithValue("@ChoiceID", choiceId);

                    var result = cmd.ExecuteScalar()?.ToString();
                    if (string.IsNullOrEmpty(result))
                        return null;

                    // Expecting ChoiceLabel format: "18-24" or "55+"
                    if (result.Contains("-"))
                    {
                        var parts = result.Split('-');
                        return (int.Parse(parts[0]), int.Parse(parts[1]));
                    }
                    else if (result.EndsWith("+"))
                    {
                        return (int.Parse(result.TrimEnd('+')), null);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Failed to retrieve age range for choice from the database", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAgeRangeByChoice method: " + ex.Message, ex);
            }
        }

    }
}