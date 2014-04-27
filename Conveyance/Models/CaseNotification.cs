using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class CaseNotification
    {
        public Case Case { get; set; }
        public int NotificationNumber { get; set; }
    }
}