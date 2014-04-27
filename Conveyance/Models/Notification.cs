using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public DateTime SendDateTime { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public bool Checked { get; set; }
        public bool IsInvitation { get; set; }
        public int CaseID { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        public virtual Case Case { get; set; }
    }
}