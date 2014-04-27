using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Conveyance.Models;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Threading.Tasks;

namespace Conveyance.Hubs
{
    // [Authorize]
    public class NotificationHub : Hub
    {
        public void Initialize(string userID)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //TODO : get Hub Authorize working
                string name;

                name = db.Users.FirstOrDefault(p => p.Id == userID).UserName;
                
                Groups.Add(Context.ConnectionId, name);
            }
        }
        public void GetNotifications(string userID,int caseID)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var notifications = db.Notification.Where(p=>p.ReceiverID==userID&&p.CaseID==caseID).OrderByDescending(p=>p.SendDateTime);
                var ret = (from notification in notifications
                           select new
                           {
                               NotificationID = notification.NotificationID,
                               Message = notification.Message,
                               Sender = notification.Sender.UserName,
                               SendDateTime = notification.SendDateTime,
                               URL = notification.URL,
                               Checked = notification.Checked
                           }).ToArray();
                Clients.Caller.loadNotifications(ret);

                //set unview notification number
                int counter = 0;
                foreach (Notification n in notifications)
                {
                    if (!n.Checked)
                        counter++;
                }
                Clients.Caller.setCounter(counter);
            }
        }
        public void GetInvitations(string userID)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var notifications = db.Notification.Where(p => p.ReceiverID == userID && p.IsInvitation).OrderByDescending(p => p.SendDateTime);
                var ret = (from notification in notifications
                           select new
                           {
                               NotificationID = notification.NotificationID,
                               Message = notification.Message,
                               Sender = notification.Sender.UserName,
                               SendDateTime = notification.SendDateTime,
                               URL = notification.URL,
                               Checked = notification.Checked
                           }).ToArray();
                Clients.Caller.loadInvitations(ret);

                //set unview notification number
                int counter = 0;
                foreach (Notification n in notifications)
                {
                    if (!n.Checked)
                        counter++;
                }
                Clients.Caller.setCounter(counter);
            }
        }
        public void AddNotification(Notification notification)
        {
            notification.SendDateTime = DateTime.Now;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string UserName = notification.ReceiverID;
                var receiver = db.Users.FirstOrDefault(x => x.UserName == notification.ReceiverID);
                if (receiver == null)
                {
                    //ERROR
                    Clients.Caller.error("Username does not exist");
                    return;
                }
                notification.ReceiverID = receiver.Id;

                db.Notification.Add(notification);
                db.SaveChanges();
                Clients.Group(UserName).newNotification();
                Clients.Caller.acknowledge();
            }
        }

        public void AddInvitation(Notification notification)
        {
            

           
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //check if user send invitation to himself
                ApplicationUser user = db.Users.Find(notification.SenderID);
                if (string.Compare(user.UserName, notification.ReceiverID) == 0)
                {
                    Clients.Caller.error("Can not send invitation to yourself");
                    return;
                }

                notification.SendDateTime = DateTime.Now;
                string UserName = notification.ReceiverID;
                var receiver = db.Users.FirstOrDefault(x => x.UserName == notification.ReceiverID);
                if (receiver == null)
                {
                    //ERROR
                    Clients.Caller.error("Username does not exist");
                    return;
                }
                notification.ReceiverID = receiver.Id;

                db.Notification.Add(notification);
                db.SaveChanges();
                Clients.Group(UserName).newInvitation();
                Clients.Caller.acknowledge();
            }
        }
    }
}