using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        [ForeignKey("Module")]
        public int ModuleID { get; set; }
        public string SenderID { get; set; }
        public string Content { get; set; }
        public DateTime SendDateTime { get; set; }
        public virtual Module Module { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }
}