using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class Log
    {
        public  int LogID { get; set; }
        public  string UserID { get; set; }
        public  string Content { get; set; }
        public DateTime DateTime { get; set; }
        public int CaseID { get; set; }
        public int? ModuleID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Case Case { get; set; }
        public virtual Module Module { get; set; }
    }
}