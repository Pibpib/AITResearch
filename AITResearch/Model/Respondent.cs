using AITResearch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch
{
    public class Respondent
    {
        public int RespondentID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAnonymous { get; set; }
        public RespondentProfile Profile { get; set; }
    }
}