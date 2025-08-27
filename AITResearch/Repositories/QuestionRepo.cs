using AITResearch.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITResearch.Repositories
{
    /// <summary>
    /// Repository to manage Question table operations.
    /// Handles getting questions, top-level questions, and child questions.
    /// </summary>
    public class QuestionRepo
    {
        private readonly string _connectionString;

        public QuestionRepo()
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
        /// maps a SQL data reader row to a Question object.
        /// </summary>
        /// <param name="reader">the SQL data reader containing question data</param>
        /// <returns>Question object</returns>
        private Question MapQuestion(SqlDataReader reader)
        {
            try
            {
                return new Question
                {
                    QuestionID = (int)reader["QuestionID"],
                    QuestionText = reader["QuestionText"].ToString(),
                    InputType = reader["InputType"].ToString().ToLower(),
                    DisplayOrder = (int)reader["DisplayOrder"],
                    ParentQuestionID = reader["ParentQuestionID"] != DBNull.Value ? (int?)reader["ParentQuestionID"] : null,
                    ConditionValue = reader["ConditionValue"]?.ToString(),
                    IsRequired = (bool)reader["IsRequired"],
                    TopicID = reader["TopicID"] != DBNull.Value ? (int?)reader["TopicID"] : null,
                    MaxAnswer = reader["MaxAnswer"] != DBNull.Value ? (int?)reader["MaxAnswer"] : null,
                    MinAnswer = reader["MinAnswer"] != DBNull.Value ? (int?)reader["MinAnswer"] : null,
                    ValidationPattern = reader["ValidationPattern"]?.ToString()
                };
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while mapping question", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MapQuestion: " + ex.Message);
            }
        }

        /// <summary>
        /// get question by its question ID
        /// </summary>
        /// <param name="questionId">unique identifier for each question</param>
        /// <returns>question object or null if not found</returns>
        public Question GetQuestion(int questionId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetQuestionById, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@QID", questionId);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read()) return MapQuestion(reader);
                }
                
                return null;
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while fetching question", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetQuestion: " + ex.Message);
            }
        }

        /// <summary>
        /// get the question where parent question id = null and ordered by display ascendingly
        /// </summary>
        /// <returns>first top-level Question object, or null if none found</returns>
        public Question GetFirstTopLevelQuestion()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetFirstTopLevelQuestion, conn))
                {
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.Read()) return MapQuestion(reader);
                }
                
                return null;
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while fetching first top-level question", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetFirstTopLevelQuestion: " + ex.Message);
            }
        }

        /// <summary>
        /// get the next question where parent question id = null after current display order
        /// </summary>
        /// <param name="currentDisplayOrder">displayOrder of the current question</param>
        /// <returns>next top-level Question object, or null if none found</returns>
        public Question GetNextTopLevelQuestion(int currentDisplayOrder)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(AppConstant.SqlQueries.GetNextTopLevelQuestion, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Order", currentDisplayOrder);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read()) return MapQuestion(reader);
                }
                
                return null;
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while fetching next top-level question", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNextTopLevelQuestion: " + ex.Message);
            }
        }

        /// <summary>
        /// get child questions from the parent question and list of answer choice IDs
        /// </summary>
        /// <param name="parentQuestionId">ID of the parent question if this is a conditional sub-question</param>
        /// <param name="answerChoiceIds">list of answer choice IDs selected for the parent question</param>
        /// <returns>list of child Question objects</returns>
        public List<Question> GetChildQuestions(int parentQuestionId, List<string> answerChoiceIds)
        {
            try
            {
                var list = new List<Question>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string sql;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (answerChoiceIds != null && answerChoiceIds.Count > 0)
                    {
                        //build parameterized IN clause
                        var paramNames = answerChoiceIds.Select((id, index) => $"@CondVal{index}").ToList();
                        string inClause = string.Join(",", paramNames);

                        //format SQL template with dynamic IN clause
                        cmd.CommandText = string.Format(AppConstant.SqlQueries.GetChildQuestionsTemplate, $"ConditionValue IN ({inClause}) OR ConditionValue IS NULL");

                        cmd.Parameters.AddWithValue("@PID", parentQuestionId);

                        for (int i = 0; i < answerChoiceIds.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(paramNames[i], answerChoiceIds[i]);
                        }
                    }
                    else
                    {
                        // if no answer selected, get only child questions with ConditionValue IS NULL
                        cmd.CommandText = AppConstant.SqlQueries.GetChildQuestionsNoCondition;
                        cmd.Parameters.AddWithValue("@PID", parentQuestionId);
                    }

                    //execute and map results
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapQuestion(reader));
                        }
                    }
                }
                return list;
            }
            catch (SqlException sqlex)
            {
                throw new Exception("Database error occurred while fetching child questions", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetChildQuestions: " + ex.Message);
            }
        }
    }
}