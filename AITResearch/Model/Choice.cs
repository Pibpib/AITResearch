using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch.Model
{
    public class Choice
    {
        public int ChoiceID { get; set; }
        public int QuestionID { get; set; }
        public string ChoiceLabel { get; set; }
        public int? DisplayOrder { get; set; }
    }
}