using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AITResearch.Model
{
    public class Attendance
    {
        public int AttendID { get; set; }
        public int RespondentID { get; set; }
        public DateTime Date { get; set; }
        public string IPAddress { get; set; }
    }
}