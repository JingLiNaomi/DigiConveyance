using Conveyance.Hubs;
using Conveyance.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Conveyance.Controllers
{
    [Authorize]
    public class ConveyanceController : HubController<NotificationHub>
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [Authorize(Roles = "Admin,Solicitor")]
        //
        // GET: /Conveyance/BuyerSolicitorPanel/5
        public ActionResult BuyerSolicitorPanel(int CaseID, string Tab = "A")
        {
            if (db.Cases.Find(CaseID) == null)
            {
                return HttpNotFound();
            }
            //Check if client is enrollend
            if (db.Cases.Find(CaseID).ClientID == null)
                return View("ClientNotEnrolled");

            //Retrieve relevant modules
            Case @case = db.Cases.FirstOrDefault(p => p.CaseID == CaseID);
            int ClientModuleSetID = @case.ClientModuleSetID;
            List<Module> Modules = new List<Module>();
            if (@case.SolicitorModuleSetID == null)      //solicitor module set not created yet
            {
                Modules = db.Module.Where(p => (p.ModuleSetID == ClientModuleSetID && (p.InitiateParty == Instruction.Buy))).OrderBy(p => p.Position).ToList();
            }
            else
            {
                int SolicitorModuleSetID = (int)@case.SolicitorModuleSetID;
                Modules = db.Module.Where(p => (p.ModuleSetID == ClientModuleSetID && (p.InitiateParty == Instruction.Buy) || (p.ModuleSetID == SolicitorModuleSetID))).OrderBy(p => p.Position).ToList();
            }

            //Build view model
            ConveyanceViewModel model = new ConveyanceViewModel();
            model.Case = @case;
            model.Modules = Modules;
            model.Comments = GetModuleComments(Modules);
            model.Files = GetModuleFiles(Modules);
            model.Tab = Tab;

            ViewBag.CaseID = CaseID;
            ViewBag.CaseName = @case.CaseName;
            ViewBag.SolicitorName = @case.Solicitor.FirstName + " " + @case.Solicitor.LastName;
            if (@case.OpCaseID != null)
                ViewBag.OpSolicitorName = @case.OpCase.Solicitor.FirstName + " " + @case.OpCase.Solicitor.LastName;
            else
                ViewBag.OpSolicitorName = "*solicitor not enrolled*";
            return View(model);
        }

        private List<Comment> GetModuleComments(List<Module> Modules)
        {
            List<Comment> Comments = new List<Comment>();
            foreach (Comment comment in db.Comment)
            {
                //check if the comment is belonged to one of the modules
                foreach (Module m in Modules)
                {
                    if (comment.ModuleID == m.ModuleID)
                    {
                        Comments.Add(comment);
                        break;
                    }
                }
            }

            return Comments;
        }

        private List<Conveyance.Models.File> GetModuleFiles(List<Module> Modules)
        {
            List<Conveyance.Models.File> Files = new List<Conveyance.Models.File>();
            foreach (Conveyance.Models.File f in db.File)
            {
                //check if the comment is belonged to one of the modules
                foreach (Module m in Modules)
                {
                    if (f.ModuleID == m.ModuleID)
                    {
                        Files.Add(f);
                        break;
                    }
                }
            }

            return Files;
        }

        [Authorize(Roles = "Admin,Solicitor")]
        public ActionResult SellerSolicitorPanel(int CaseID, string Tab = "A")
        {
            if (db.Cases.Find(CaseID) == null)
            {
                return HttpNotFound();
            }
            //Check if client is enrollend
            if (db.Cases.Find(CaseID).ClientID == null)
                return View("ClientNotEnrolled");

            //Retrieve relevant modules
            Case @case = db.Cases.FirstOrDefault(p => p.CaseID == CaseID);
            int ClientModuleSetID = @case.ClientModuleSetID;
            List<Module> Modules = new List<Module>();
            if (@case.SolicitorModuleSetID == null)      //solicitor module set not created yet
            {
                Modules = db.Module.Where(p => (p.ModuleSetID == ClientModuleSetID && (p.InitiateParty == Instruction.Sell))).OrderBy(p => p.Position).ToList();
            }
            else
            {
                int SolicitorModuleSetID = (int)@case.SolicitorModuleSetID;
                Modules = db.Module.Where(p => (p.ModuleSetID == ClientModuleSetID && (p.InitiateParty == Instruction.Sell) || (p.ModuleSetID == SolicitorModuleSetID))).OrderBy(p => p.Position).ToList();

            }

            //Build view model
            ConveyanceViewModel model = new ConveyanceViewModel();
            model.Case = @case;
            model.Modules = Modules;
            model.Comments = GetModuleComments(Modules);
            model.Files = GetModuleFiles(Modules);
            model.Tab = Tab;

            ViewBag.CaseID = CaseID;
            ViewBag.CaseName = @case.CaseName;
            if (@case.OpCaseID != null)
                ViewBag.OpSolicitorName = @case.OpCase.Solicitor.FirstName + " " + @case.OpCase.Solicitor.LastName;
            else
                ViewBag.OpSolicitorName = "*solicitor not enrolled*";
            return View(model);
        }


        public ActionResult BuyerPanel(int CaseID, string Tab = "A")
        {
            if (db.Cases.Find(CaseID) == null)
            {
                return HttpNotFound();
            }
            Case @case = db.Cases.FirstOrDefault(p => p.CaseID == CaseID);
            int ClientModuleSetID = @case.ClientModuleSetID;
            List<Module> Modules = db.Module.Where(p => p.ModuleSetID == ClientModuleSetID).OrderBy(p => p.Position).ToList();

            //Build view model
            ConveyanceViewModel model = new ConveyanceViewModel();
            model.Case = @case;
            model.Modules = Modules;
            model.Comments = GetModuleComments(Modules);
            model.Files = GetModuleFiles(Modules);
            model.Tab = Tab;

            ViewBag.CaseID = CaseID;
            ViewBag.CaseName = @case.CaseName;
            return View(model);
        }
        public ActionResult SellerPanel(int CaseID, string Tab = "A")
        {
            if (db.Cases.Find(CaseID) == null)
            {
                return HttpNotFound();
            }
            Case @case = db.Cases.FirstOrDefault(p => p.CaseID == CaseID);
            int ClientModuleSetID = @case.ClientModuleSetID;
            List<Module> Modules = db.Module.Where(p => p.ModuleSetID == ClientModuleSetID).OrderBy(p => p.Position).ToList();

            //Build view model
            ConveyanceViewModel model = new ConveyanceViewModel();
            model.Case = @case;
            model.Modules = Modules;
            model.Comments = GetModuleComments(Modules);
            model.Files = GetModuleFiles(Modules);
            model.Tab = Tab;

            ViewBag.CaseID = CaseID;
            ViewBag.CaseName = @case.CaseName;
            return View(model);
        }

        [Authorize(Roles = "Admin,Agent")]
        public ActionResult AgentPanel(int CaseID)
        {
            if (db.Cases.Find(CaseID) == null)
            {
                return HttpNotFound();
            }
            //a pure display notification only panel, no interaction
            return View();
        }

        //Upload a file
        public ActionResult Process_SDF(int CaseID, int ModuleID, HttpPostedFileBase file, string viewName)
        {
            Case theCase = db.Cases.Find(CaseID);
            Module module = db.Module.Find(ModuleID);
            module.Attempt = module.Attempt + 1;
            db.SaveChanges();

            string path = Path.Combine(Server.MapPath("~/Upload/"), theCase.CaseName);
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            int index = file.FileName.LastIndexOf('.');
            string fileExt = file.FileName.Substring(index, file.FileName.Length - index);
            string filename = file.FileName.Substring(0, index);
            string FileName = string.Concat(path, "/" + filename + "_v" + module.Attempt + fileExt);

            file.SaveAs(FileName);

            //add record in File table
            Models.File File = new Models.File();
            File.CaseID = CaseID;
            File.Path = "/Upload/" + theCase.CaseName + "/" + filename + "_v" + module.Attempt + fileExt;
            File.UserID = User.Identity.GetUserId();
            File.DateTime = DateTime.Now;
            File.Description = module.TextC;
            File.Version = module.Attempt;
            File.ModuleID = ModuleID;
            db.File.Add(File);

            //update modules status
            module.Status = Status.InAction;
            db.SaveChanges();

            FileName = File.Path.Substring(File.Path.LastIndexOf('/') + 1);
            string action = "uploaded a file : " + FileName + " for '" + module.TextC + "'";
            //generate notification to file receiver
            sendModuleNotification(action, ModuleID, CaseID);

            //generate log
            log(action, CaseID, ModuleID);
            return RedirectToAction(viewName, new { CaseID = CaseID, Tab = module.Stage.ToString() });

        }



        //Accept SDF
        public string Accept_SDF(int CaseID, int ModuleID)
        {
            try
            {
                Module module = db.Module.Find(ModuleID);
                module.Checked = true;
                module.Status = Status.Completed;
                db.SaveChanges();
                //check for stage status changes
                updateStage(CaseID);

                //generate notification to file receiver
                string action = "accepted a file for: '" + module.TextC + "'";
                sendModuleNotification(action, ModuleID, CaseID);

                //generate log
                log(action, CaseID, ModuleID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }

        //Reset SDF
        public ActionResult Reset_SDF(int CaseID, int ModuleID, string viewName)
        {
            Case theCase = db.Cases.Find(CaseID);
            Module module = db.Module.Find(ModuleID);
            module.Checked = false;
            module.Status = Status.RequireAction;
            db.SaveChanges();
            //check for stage status changes
            updateStage(CaseID);

            string action = "Reset a module : " + module.TextC + "'";
            //generate notification to file receiver
            sendModuleNotification(action, ModuleID, CaseID);

            //generate log
            log(action, CaseID, ModuleID);
            return RedirectToAction(viewName, new { CaseID = CaseID, Tab = module.Stage.ToString() });
        }
        public string saveInf(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module m = db.Module.Find(mID);
                m.Checked = true;
                m.Status = Status.Completed;
                db.SaveChanges();
                string action = "informed that : '" + m.TextS + "'";
                sendModuleNotification(action, mID, cID);
                //generate log
                log(action, cID, mID);

                //send email notification
                Case @case = db.Cases.Find(cID);
                string subject = "Residential Conveyance Notification";
                string body = User.Identity.GetUserName() + " " + action;
                string toEmail = @case.Client.Email;
                SendEmail(subject, body, toEmail);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }
        public string ResetInf(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module module = db.Module.Find(mID);
                module.Checked = false;
                module.Status = Status.RequireAction;
                updateStage(cID);
                db.SaveChanges();
                //generate log and notification
                string action = "reset module: '" + module.TextS + "'";
                sendModuleNotification(action, mID, cID);
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }
        public bool SendEmail(string subject, string body, string toEmail)
        {
            var fromAddress = new MailAddress("ResidentialConveyance@gmail.com", "Conveyance");
            var toAddress = new MailAddress(toEmail, "");
            const string fromPassword = "Conveyance123";
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string SaveAdvice(string CaseID, string ModuleID, string AdviceText)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module m = db.Module.Find(mID);
                m.TextC = AdviceText;
                db.SaveChanges();
                //update modules status
                m.Status = Status.InAction;
                db.SaveChanges();

                //send notification
                string action = "sent an advice: '" + m.TextC + "'";
                sendModuleNotification(action, mID, cID);
                //generate log
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }

        public string AckAdvice(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module module = db.Module.Find(mID);
                module.Checked = true;
                module.Status = Status.Completed;
                updateStage(cID);
                db.SaveChanges();
                //generate log and notiifcation
                string action = "acknowledged advice : '" + module.TextC + "'";
                sendModuleNotification(action, mID, cID);
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }

        public string ResetAdvice(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module module = db.Module.Find(mID);
                module.Checked = false;
                module.Status = Status.RequireAction;
                updateStage(cID);
                db.SaveChanges();
                //generate log and notification
                string action = "reset module: '" + module.TextC + "'";
                sendModuleNotification(action, mID, cID);
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }

        public string SaveTK(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module m = db.Module.Find(mID);
                m.Checked = true;
                m.Status = Status.Completed;
                db.SaveChanges();
                updateStage(cID);
                //generate log
                string action = "ticked off task : '" + m.TextS + "'";
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }

        public string ResetTK(string CaseID, string ModuleID)
        {
            int mID = Convert.ToInt16(ModuleID);
            int cID = Convert.ToInt16(CaseID);
            try
            {
                Module m = db.Module.Find(mID);
                m.Checked = false;
                m.Status = Status.RequireAction;
                db.SaveChanges();
                updateStage(cID);
                //generate log
                string action = "reset task : '" + m.TextS + "'";
                log(action, cID, mID);
                return "success";
            }
            catch
            {
                return "fail";
            }
        }


        /*Update stage 
         * Not started: When no module in stage is completed
         * In progress: When part of modules in stage is completed
         * Completed: When all modules in stage is completed
         */
        private void updateStage(int CaseID)
        {
            Case @case = db.Cases.Find(CaseID);
            for (Stage i = Stage.A; i <= Stage.F; i++)
            {
                Progress progress = Progress.NotStarted;
                int counter = 0; //to count completed modules
                var modules = db.Module.Where(p => (p.ModuleSetID == @case.ClientModuleSetID || p.ModuleSetID == @case.SolicitorModuleSetID) && p.Stage == i);
                foreach (Module m in modules)
                {
                    if (m.Checked)
                    {
                        progress = Progress.InProcess;
                        counter++;
                    }
                }
                if (counter == modules.Count())
                {
                    progress = Progress.Completed;
                }

                switch (i)
                {
                    case Stage.A:
                        {
                            @case.ProgressA = progress;
                            break;
                        }
                    case Stage.B:
                        {
                            @case.ProgressB = progress;
                            break;
                        }
                    case Stage.C:
                        {
                            @case.ProgressC = progress;
                            break;
                        }
                    case Stage.D:
                        {
                            @case.ProgressD = progress;
                            break;
                        }
                    case Stage.E:
                        {
                            @case.ProgressE = progress;
                            break;
                        }
                    case Stage.F:
                        {
                            @case.ProgressF = progress;
                            break;
                        }
                }
                if (@case.ProgressA == Progress.Completed && @case.ProgressB == Progress.Completed && @case.ProgressC == Progress.Completed && @case.ProgressD == Progress.Completed && @case.ProgressE == Progress.Completed && @case.ProgressF == Progress.Completed)
                {
                    @case.IsCompleted = true;
                    @case.CompleteDateTime = DateTime.Now;
                }
                else
                {
                    @case.IsCompleted = false;
                    @case.CompleteDateTime = null;
                }
                db.SaveChanges();
            }
        }
        public JsonResult AddComment(int CaseID, int ModuleID, string comment)
        {
            try
            {
                Module module = db.Module.Find(ModuleID);
                Comment c = new Comment();
                c.ModuleID = ModuleID;
                c.SenderID = User.Identity.GetUserId();
                c.Content = comment;
                c.SendDateTime = DateTime.Now;
                db.Comment.Add(c);
                db.SaveChanges();

                //Generate notification 
                string action = "commented on " + module.TextC + " : " + c.Content;
                sendModuleNotification(action, ModuleID, CaseID);
                //generate log
                log(action, CaseID, ModuleID);
                return Json(new CommentJsonModel { username = User.Identity.GetUserName(), content = c.Content, datetime = c.SendDateTime.ToString() });
            }
            catch
            {
                return null;
            }
        }

        public void OnClickNotification(int NotificationID, string url, string Tab)
        {
            Notification n = db.Notification.Find(NotificationID);
            if (n == null)
                return;

            n.Checked = true;
            db.SaveChanges();
            Response.Redirect(url + "&Tab=" + Tab);
        }

        private void sendModuleNotification(string msg, int ModuleID, int CaseID)
        {
            Module module = db.Module.Find(ModuleID);
            Case @case = db.Cases.Find(CaseID);
            string receiverID = "";
            string url = "";
            //figure out receiver id and url
            Communication communication = module.Communication;
            if (communication == Communication.SolicitorToBuyer || communication == Communication.BuyerToSolicitor)
            {
                if (User.IsInRole("Solicitor")) //receiver is buyer
                {
                    receiverID = @case.ClientID;
                    url = Url.Content("~") + "Conveyance/BuyerPanel?CaseID=" + CaseID + "&Tab=" + module.Stage.ToString();
                }
                else  //receiver is buyer solicitor
                {
                    receiverID = @case.SolicitorID;
                    url = Url.Content("~") + "Conveyance/BuyerSolicitorPanel?CaseID=" + CaseID + "&Tab=" + module.Stage.ToString();

                }

            }
            else if (communication == Communication.SolicitorToSeller || communication == Communication.SellerToSolicitor)
            {
                if (User.IsInRole("Solicitor")) //receiver is seller
                {
                    receiverID = @case.ClientID;
                    url = Url.Content("~") + "Conveyance/SellerPanel?CaseID=" + CaseID + "&Tab=" + module.Stage.ToString();
                }
                else  //receiver is seller solicitor
                {
                    receiverID = @case.SolicitorID;
                    url = Url.Content("~") + "Conveyance/SellerSolicitorPanel?CaseID=" + CaseID + "&Tab=" + module.Stage.ToString();
                }
            }
            else if (communication == Communication.BSolicitorToSSolicitor || communication == Communication.SSolicitorToBSolicitor)
            {
                if (@case.Instruction == Instruction.Buy)  //receiver is seller solicitor
                {
                    receiverID = db.Cases.Find(@case.OpCaseID).SolicitorID;
                    url = Url.Content("~") + "Conveyance/SellerSolicitorPanel?CaseID=" + @case.OpCaseID + "&Tab=" + module.Stage.ToString();
                }
                else  //receiver is buyer solicitor
                {
                    receiverID = db.Cases.Find(@case.OpCaseID).SolicitorID;
                    url = Url.Content("~") + "Conveyance/BuyerSolicitorPanel?CaseID=" + @case.OpCaseID + "&Tab=" + module.Stage.ToString();
                }
            }
            sendNotification(msg, receiverID, url, CaseID);
        }

        private void log(string content, int CaseID, int ModuleID)
        {
            Conveyance.Models.Log log = new Conveyance.Models.Log();
            log.DateTime = DateTime.Now;
            log.UserID = User.Identity.GetUserId();
            log.Content = content;
            log.CaseID = CaseID;
            log.ModuleID = ModuleID;
            db.Log.Add(log);
            db.SaveChanges();
        }

        private void sendNotification(String message, string receiverID, string url, int CaseID)
        {
            Notification notification = new Notification();
            notification.SendDateTime = DateTime.Now;
            notification.SenderID = User.Identity.GetUserId();
            notification.Message = message;
            notification.ReceiverID = receiverID;
            notification.URL = url;
            notification.CaseID = CaseID;
            string UserName = db.Users.Find(receiverID).UserName;
            db.Notification.Add(notification);
            db.SaveChanges();
            var ret = new
            {
                Message = notification.Message,
                Sender = User.Identity.GetUserName(),
                // PostedByAvatar = imgFolder + (String.IsNullOrEmpty(post.UserProfile.AvatarExt) ? defaultAvatar : post.PostedBy + "." + post.UserProfile.AvatarExt),
                SendDateTime = notification.SendDateTime,
                URL = notification.URL
            };

            Hub.Clients.Group(UserName).newNotification(ret);
        }
    }
}

