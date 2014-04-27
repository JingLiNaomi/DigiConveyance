using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class File
    {
        public int FileID { get; set; }
        [ForeignKey("Case")]
        public int CaseID { get; set; }
        [ForeignKey("Module")]
        public int ModuleID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public int Version { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Case Case { get; set; }
        public virtual Module Module { get; set; }
    }
}