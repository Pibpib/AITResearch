using System;
using AITResearch.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch.Model
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string InputType { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentQuestionID { get; set; }
        public string ConditionValue { get; set; }
        public bool IsRequired { get; set; }
        public int? TopicID { get; set; }
        public int? MaxAnswer {  get; set; }
        public int? MinAnswer { get; set; }
        public string ValidationPattern { get; set; }

        // Navigation property
        public List<Choice> Choices { get; set; }
    }
}