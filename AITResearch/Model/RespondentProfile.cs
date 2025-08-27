using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch.Model
{
    public class RespondentProfile
    {
        public int RespondentID { get; set; }   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
    }
}