using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch
{
    public class AppConstant
    {
        public static class AppConnection
        {
            public const string DevConnection = "Data Source=SQL5110.site4now.net; Initial Catalog=db_9ab8b7_25dda13429; User Id=db_9ab8b7_25dda13429_admin; Password=eVNGum3W; Connect Timeout=240;";
            public const string TestConnection = "";
            public const string ProdConnection = "";
            public const string BetaConnection = "";
        }

        public class SqlQueries
        {
            public const string InsertAttendance =
                "INSERT INTO Attendance (RespondentID, Date, IPAddress) VALUES (@RespondentID, @Date, @IPAddress)";
            public const string GetChoicesByQuestionId =
                "SELECT ChoiceID, QuestionID, ChoiceLabel, DisplayOrder FROM Choice WHERE QuestionID = @QuestionID";
            public const string GetQuestionById =
                "SELECT * FROM Question WHERE QuestionID=@QID";
            public const string GetFirstTopLevelQuestion =
                "SELECT TOP 1 * FROM Question WHERE ParentQuestionID IS NULL ORDER BY DisplayOrder";
            public const string GetNextTopLevelQuestion =
                @"SELECT TOP 1 * FROM Question 
                               WHERE DisplayOrder > @Order AND ParentQuestionID IS NULL
                               ORDER BY DisplayOrder";
            public const string GetChildQuestionsTemplate =
                @"SELECT * FROM Question 
                  WHERE ParentQuestionID=@PID 
                  AND ({0}) 
                  ORDER BY DisplayOrder";
            public const string GetChildQuestionsNoCondition =
                @"SELECT * FROM Question 
                  WHERE ParentQuestionID=@PID AND ConditionValue IS NULL
                  ORDER BY DisplayOrder";
            public const string UpdateRespondentProfile =
                @"UPDATE RespondentProfile 
                  SET FirstName = @FirstName, 
                  LastName = @LastName, 
                  PhoneNumber = @PhoneNumber, 
                  DOB = @DOB
                  WHERE RespondentID = @RespondentID";
            public const string CreateRespondent =
                @"INSERT INTO Respondent (CreatedAt, IsAnonymous)
                  VALUES (@CreatedAt, @IsAnonymous); 
                  SELECT CAST(SCOPE_IDENTITY() AS int);";
            public const string UpdateRespondentToNotAnonymous =
                "UPDATE Respondent SET isAnonymous = 0 WHERE RespondentID = @RespondentID";
            public const string SaveResponse =
                @"INSERT INTO Response (RespondentID, QuestionID, AnswerValue)
                  VALUES (@RespondentID, @QuestionID, @AnswerValue)";
            public const string GetStaffByUsernameAndPassword =
                "SELECT StaffID, Username, Password, Role FROM Staff WHERE Username = @Username AND Password = @Password";
            public const string InsertStaff =
                "INSERT INTO Staff (Username, Password, Role) VALUES (@Username, @Password, @Role)";
            public const string IsUsernameTaken =
                "SELECT COUNT(1) FROM Staff WHERE Username = @Username";
            public const string GetSurveyResultsPivot =
                "dbo.usp_GetSurveyResultsPivot1";
            public const string GetRespondentAgeRangeChoice =
                "SELECT AnswerValue FROM Response WHERE RespondentID=@RespID AND QuestionID=2";
            public const string GetAgeRangeByChoice=
                "SELECT ChoiceLabel FROM Choice WHERE ChoiceID = @ChoiceID";
        }
    }
}