using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch.Model
{
    public class Response
    {
        public int ResponseID { get; set; }
        public int RespondentID { get; set; }
        public int QuestionID { get; set; }
        public string AnswerValue { get; set; }

    }
}