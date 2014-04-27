using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Conveyance.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json;

namespace Conveyance.Controllers
{
    [Authorize]
    public class CasesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: /Cases/
        public ActionResult Index(int? Sort = 0)
        {
            String UserID = User.Identity.GetUserId();
            List<Case> cases = new List<Case>();
            if (Sort == 1)
                cases = db.Cases.Where(p => string.Compare(p.SolicitorID, UserID) == 0 || string.Compare(p.ClientID, UserID) == 0).OrderBy(p => p.CaseName).ToList();
            else if (Sort == 2)
                cases = db.Cases.Where(p => string.Compare(p.SolicitorID, UserID) == 0 || string.Compare(p.ClientID, UserID) == 0).OrderBy(p => p.CreateDateTime).ToList();
            else
                cases = db.Cases.Where(p => string.Compare(p.SolicitorID, UserID) == 0 || string.Compare(p.ClientID, UserID) == 0).ToList();

            //encapsulate case into CaseNotification model

            List<CaseNotification> models = new List<CaseNotification>();
            foreach (Case c in cases)
            {
                CaseNotification model = new CaseNotification();
                model.Case = c;
                model.NotificationNumber = db.Notification.Where(p => p.ReceiverID == UserID && p.CaseID == c.CaseID && !p.Checked).ToList().Count();
                models.Add(model);
            }
            ViewBag.UserID = UserID;
            return View(models);
        }

        // GET: /Cases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = User.Identity.GetUserId();
            return View(@case);
        }

        // GET: /Cases/Create
        public ActionResult Create()
        {
            //populate instruction dropdownlist
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Buy", Value = Instruction.Buy.ToString() });
            items.Add(new SelectListItem { Text = "Sell", Value = Instruction.Sell.ToString() });
            ViewBag.DropDownInstruction = items;

            //populate city dropdownlist
            var Cities = db.City.ToList();
            List<SelectListItem> cityItems = new List<SelectListItem>();
            foreach (City c in Cities)
            {
                cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
            }
            ViewBag.DropDownCities = cityItems;
            //populate template dropdownlist
            var Templates = db.Template.ToList();
            List<SelectListItem> templateItems = new List<SelectListItem>();
            foreach (Template t in Templates)
            {
                templateItems.Add(new SelectListItem { Text = t.Name, Value = t.TemplateID.ToString() });
            }
            ViewBag.DropDownTemplates = templateItems;

            ViewBag.UserID = User.Identity.GetUserId();
            return View();
        }

        // POST: /Cases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Reference,Postcode,AddressLine1,AddressLine2,Country")] CreateCaseViewModel @case, string DropDownInstruction, string DropDownCities, string DropDownTemplates)
        {
            if (string.Compare(DropDownInstruction, Instruction.Buy.ToString()) == 0)
                @case.Instruction = Instruction.Buy;
            else
                @case.Instruction = Instruction.Sell;
            @case.City = DropDownCities;
            int TemplateID = Convert.ToInt16(DropDownTemplates);
            if (ModelState.IsValid)
            {
                Case newCase = new Case();
                newCase.Instruction = @case.Instruction;
                newCase.Reference = @case.Reference;
                newCase.AddressLine1 = @case.AddressLine1;
                newCase.AddressLine2 = @case.AddressLine2;
                newCase.CityID = Convert.ToInt16(@case.City);
                newCase.Country = @case.Country;
                newCase.Postcode = RemoveSpace(@case.Postcode);
                newCase.CaseName = @case.Instruction + "_" + newCase.Postcode + "_" + @case.Reference;
                newCase.CreateDateTime = (DateTime)DateTime.Now;
                newCase.StartDateA = newCase.CreateDateTime;
                newCase.SolicitorID = User.Identity.GetUserId();
                newCase.TemplateID = TemplateID;
                newCase.ClientModuleSetID = CreateClientModules(newCase.SolicitorID + newCase.CaseName, newCase.Instruction, TemplateID);
                db.Cases.Add(newCase);
                db.SaveChanges();


                Case addedCase = db.Cases.FirstOrDefault(p=>p.CaseName==newCase.CaseName&&p.ClientModuleSetID==newCase.ClientModuleSetID);
                //Create a new folder for case
                string path = Path.Combine(Server.MapPath("~/Upload/"), newCase.CaseName+"_"+addedCase.CaseID);
                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                //log new case creation
                log("created new case : "+addedCase.CaseName, addedCase.CaseID);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMsg = "Creating new case failed, please try again later";
            //populate instruction dropdownlist
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Buy", Value = Instruction.Buy.ToString() });
            items.Add(new SelectListItem { Text = "Sell", Value = Instruction.Sell.ToString() });
            ViewBag.DropDownInstruction = items;

            //populate city dropdownlist
            var Cities = db.City.ToList();
            List<SelectListItem> cityItems = new List<SelectListItem>();
            foreach (City c in Cities)
            {
                cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
            }
            //populate template dropdownlist
            var Templates = db.Template.ToList();
            List<SelectListItem> templateItems = new List<SelectListItem>();
            foreach (Template t in Templates)
            {
                templateItems.Add(new SelectListItem { Text = t.Name, Value = t.TemplateID.ToString() });
            }
            ViewBag.DropDownTemplates = templateItems;

            ViewBag.DropDownCities = cityItems;
            ViewBag.UserID = User.Identity.GetUserId();
            return View();
        }
        public string RemoveSpace(string inputString)
        {
            string result = inputString.Trim();
            List<char> list = result.ToList();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i) == ' ')
                    {
                        list.RemoveAt(i);
                        i = 0;
                    }
                }
                return String.Concat(list);
            }
            catch
            {
                return result;
            }
        }
        private int CreateClientModules(string moduleSetName, Instruction instruction,int TemplateID)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            //create new Module Configuration
            ModuleSet clientModuleSet = new ModuleSet();
            clientModuleSet.Name = moduleSetName;
            db.ModuleSet.Add(clientModuleSet);
            db.SaveChanges();

            int clientModuleSetID = db.ModuleSet.FirstOrDefault(p => string.Compare(p.Name, moduleSetName) == 0).ModuleSetID;

            // read file into a string and deserialize JSON to a type
            Template template = db.Template.Find(TemplateID);
            string path = template.Path;
            List<Module> configModules = JsonConvert.DeserializeObject<List<Module>>(System.IO.File.ReadAllText(@path));

            foreach (Module module in configModules)
            {
                if (module.Communication != Communication.BSolicitorToSSolicitor && module.Communication != Communication.SSolicitorToBSolicitor)
                {
                    module.ModuleSetID = clientModuleSetID;
                    db.Module.Add(module);
                }
            }
            db.SaveChanges();
      
            return clientModuleSetID;
        }

        // GET: /Cases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }

            //populate city dropdownlist
            var Cities = db.City.ToList();
            List<SelectListItem> cityItems = new List<SelectListItem>();
            foreach (City c in Cities)
            {
                if (@case.CityID == c.CityID)
                    cityItems.Add(new SelectListItem { Selected = true, Text = c.CityName, Value = c.CityID.ToString() });
                else
                    cityItems.Add(new SelectListItem { Selected = false, Text = c.CityName, Value = c.CityID.ToString() });

            }

            ViewBag.DropDownCities = cityItems;
            ViewBag.UserID = User.Identity.GetUserId();
            return View(@case);
        }

        // POST: /Cases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CaseID,CaseName,CreateDateTime,CompleteDateTime,Status,ClientID,SolicitorID,OpCaseID,Reference,AddressLine1,AddressLine2,Postcode")] Case @case, string DropDownCities)
        {
            @case.CaseName = @case.Instruction + "_" + @case.Reference + "_" + @case.Postcode;
            @case.CityID = Convert.ToInt16(DropDownCities);
            @case.Country = "United Kingdom";
            if (ModelState.IsValid)
            {
                db.Entry(@case).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = User.Identity.GetUserId();
            return View(@case);
        }

        // GET: /Cases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = User.Identity.GetUserId();
            return View(@case);
        }

        // POST: /Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Case @case = db.Cases.Find(id);
            @case.OpCaseID = null;
            db.Cases.Remove(@case);
            //delete linked modules
            db.Module.RemoveRange(db.Module.Where(p => p.ModuleSetID == @case.ClientModuleSetID || p.ModuleSetID == @case.SolicitorModuleSetID));
            //delete linked moduleSets
            if (db.ModuleSet.Find(@case.ClientModuleSetID) != null)
                db.ModuleSet.Remove(db.ModuleSet.Find(@case.ClientModuleSetID));
            if (@case.SolicitorModuleSetID!=null)
                db.ModuleSet.Remove(db.ModuleSet.Find(@case.SolicitorModuleSetID));
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Cases/Files/5
        public ActionResult Files(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            List<Models.File> files = new List<Models.File>();
            if (User.IsInRole("Solicitor"))
            {
                //retrieve modules
                List<Module> modules = db.Module.Where(p => (p.ModuleSetID == @case.ClientModuleSetID || p.ModuleSetID == @case.SolicitorModuleSetID) && p.Type == Models.Type.SDF).ToList();
                foreach (Module module in modules)
                {
                    List<Models.File> mfiles = db.File.Where(p => p.ModuleID == module.ModuleID).ToList();
                    files.AddRange(mfiles);
                }
                ViewBag.SelfID = User.Identity.GetUserId();
                if (@case.ClientID != null)
                    ViewBag.ClientID = @case.ClientID;
                if (@case.OpCaseID != null)
                    ViewBag.OpSolicitorID = @case.OpCase.SolicitorID;
                return View("SolicitorFiles", files);
            }
            else
            {
                //retrieve modules
                List<Module> modules = db.Module.Where(p => p.ModuleSetID == @case.ClientModuleSetID && p.Type == Models.Type.SDF).ToList();
                foreach (Module module in modules)
                {
                    List<Models.File> mfiles = db.File.Where(p => p.ModuleID == module.ModuleID).ToList();
                    files.AddRange(mfiles);
                }

                ViewBag.SelfID = User.Identity.GetUserId();
                ViewBag.SolicitorID = @case.SolicitorID;
                return View("ClientFiles", files);
            }

        }

        // GET: /Cases/Invite/5
        public ActionResult Invite(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.CaseID = @case.CaseID;
            ViewBag.UserID = User.Identity.GetUserId();
            return View();
        }
        // POST: /Cases/Invite/5
        //To invite client by sending email
        [HttpPost]
        public ActionResult Invite(InviteCaseViewModel model)
        {
            string Email = model.Email;
            int CaseID = model.CaseID;
            if (ModelState.IsValid)
            {
                var fromAddress = new MailAddress("ResidentialConveyance@gmail.com", "Conveyance");
                var toAddress = new MailAddress(Email, "");
                const string fromPassword = "Conveyance123";
                const string subject = "Conveyance Invitation";
                string body = "Invitation to case " + Request.Url.GetLeftPart(UriPartial.Authority) + "/cases/EnrollClient/" + CaseID;
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
                }
                catch
                {
                    ViewBag.ErrorMsg = "Failed to send invitation, please try again later";
                    ViewBag.CaseID = CaseID;
                    return View();
                }


                ViewBag.SuccessMsg = "Invitation sent";
                ViewBag.CaseID = CaseID;
                //log invitation
                log("sent invitation to client via email: "+Email , CaseID);
                return View();
            }
            else
            {
                ViewBag.ErrorMsg = "Failed to send invitation";
                ViewBag.CaseID = CaseID;
                return View();
            }
        }

        // GET: /Cases/EnrollClient/5
        public ActionResult EnrollClient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            String UserID = User.Identity.GetUserId();
            @case.ClientID = UserID;
            db.SaveChanges();

            ViewBag.CaseName = @case.CaseName;
            ViewBag.UserID = User.Identity.GetUserId();
            //log client enrollment
            log("client enrolled to case : " + @case.CaseName, @case.CaseID);
            return View();
        }

        [Authorize(Roles = "Solicitor, Admin")]
        // GET: /Cases/EnrollSolicitor/5
        // id: case id of inviting party
        public ActionResult EnrollSolicitor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.CaseID = @case.CaseID;
            ViewBag.CaseName = @case.CaseName;
            ViewBag.Solicitor = @case.Solicitor.UserName;

            //Populate cases dropdownlist
            String UserID = User.Identity.GetUserId();
            var cases = db.Cases.Where(p => string.Compare(p.SolicitorID, UserID) == 0);
            List<SelectListItem> caseItems = new List<SelectListItem>();
            foreach (Case c in cases)
            {
                caseItems.Add(new SelectListItem { Text = c.CaseName, Value = c.CaseID.ToString() });
            }

            ViewBag.DropDownCases = caseItems;
            ViewBag.UserID = User.Identity.GetUserId();
            return View();
        }


        [HttpPost]
        public ActionResult EnrollSolicitor(int CaseID, string DropDownCases)
        {
            Case @case = db.Cases.First(p => p.CaseID == CaseID);
            if (@case != null && DropDownCases != null)
            {
                int cID = Convert.ToInt16(DropDownCases);
                @case.OpCaseID = cID;
                Case OpCase = db.Cases.First(p => p.CaseID == cID);
                OpCase.OpCaseID = @case.CaseID;
                db.SaveChanges();

                //create SolicitorModuleSet
                string S_moduleSetName = @case.CaseName + db.Cases.Find(@case.OpCaseID).CaseName;
                Template template = db.Template.Find(@case.TemplateID);
                int SolicitorModuleSetID = CreateSolicitorModuleSet(S_moduleSetName,template.Path);
                UpdateStageStatus(CaseID);
                @case.SolicitorModuleSetID = SolicitorModuleSetID;
                //create SolicitorModuleSet for OpCase
                string S_moduleSetNameOp =db.Cases.Find(@case.OpCaseID).CaseName+ @case.CaseName;
                Template OpTemplate = db.Template.Find(OpCase.TemplateID);
                int OpSolicitorModuleSetID = CreateSolicitorModuleSet(S_moduleSetNameOp, OpTemplate.Path);
                UpdateStageStatus(OpCase.CaseID);
                OpCase.SolicitorModuleSetID = OpSolicitorModuleSetID;
                db.SaveChanges();
                //log solicitor enrollment
                log("solicitor enrolled to case : " + @case.CaseName, @case.CaseID);
                log("Enrolled case : " + OpCase.CaseName, OpCase.CaseID);
                ViewBag.SuccessMsg = "Enroll successfully";
            }
            else if (DropDownCases == null)
            {
                //ERROR
                ViewBag.ErrorMsg = "No case chosen";
            }
            else
            {
                //ERROR
                ViewBag.ErrorMsg = "Case " + @case.CaseName + " not found, enroll failed";
            }
            ViewBag.CaseID = @case.CaseID;
            ViewBag.CaseName = @case.CaseName;
            ViewBag.Solicitor = @case.Solicitor.UserName;

            //Populate cases dropdownlist
            String UserID = User.Identity.GetUserId();
            var cases = db.Cases.Where(p => string.Compare(p.SolicitorID, UserID) == 0);
            List<SelectListItem> caseItems = new List<SelectListItem>();
            foreach (Case c in cases)
            {
                caseItems.Add(new SelectListItem { Text = c.CaseName, Value = c.CaseID.ToString() });
            }

            ViewBag.DropDownCases = caseItems;
            return View();
        }

        private int CreateSolicitorModuleSet(string moduleSetName,string path)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            //create new Module Configuration
            ModuleSet SolicitorModuleSet = new ModuleSet();
            SolicitorModuleSet.Name = moduleSetName;
            db.ModuleSet.Add(SolicitorModuleSet);
            db.SaveChanges();

            int SolicitorModuleSetID = db.ModuleSet.FirstOrDefault(p => string.Compare(p.Name, moduleSetName) == 0).ModuleSetID;
            // read file into a string and deserialize JSON to a type
            List<Module> configModules = JsonConvert.DeserializeObject<List<Module>>(System.IO.File.ReadAllText(@path));

            foreach (Module module in configModules)
            {
                if (module.Communication == Communication.BSolicitorToSSolicitor || module.Communication == Communication.SSolicitorToBSolicitor)
                {
                    module.ModuleSetID = SolicitorModuleSetID;
                    db.Module.Add(module);
                }
            }
            db.SaveChanges();
            return SolicitorModuleSetID;
        }

        private void UpdateStageStatus(int CaseID)
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
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateA = DateTime.Now;
                                @case.StartDateB = DateTime.Now;
                            }
                            break;
                        }
                    case Stage.B:
                        {
                            @case.ProgressB = progress;
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateB = DateTime.Now;
                                @case.StartDateC = DateTime.Now;
                            }
                            break;
                        }
                    case Stage.C:
                        {
                            @case.ProgressC = progress;
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateC = DateTime.Now;
                                @case.StartDateD = DateTime.Now;
                            }
                            break;
                        }
                    case Stage.D:
                        {
                            @case.ProgressD = progress;
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateD = DateTime.Now;
                                @case.StartDateE = DateTime.Now;
                            }
                            break;
                        }
                    case Stage.E:
                        {
                            @case.ProgressE = progress;
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateE = DateTime.Now;
                                @case.StartDateF = DateTime.Now;
                            }
                            break;
                        }
                    case Stage.F:
                        {
                            @case.ProgressF = progress;
                            if (progress == Progress.Completed)
                            {
                                @case.CompleteDateF = DateTime.Now;
                                @case.IsCompleted = true;
                            }
                            break;
                        }
                }
                db.SaveChanges();
            }
        }

        public ActionResult Log(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }

            //get logs for specific user
            string UserID = User.Identity.GetUserId();
            List<Conveyance.Models.Log> logs = new List<Log>();
            if (UserID == @case.SolicitorID)
            { 
                //all logs available to solicior
                if (@case.OpCaseID == null)
                {
                    logs.AddRange(db.Log.Where(p => p.CaseID == id).OrderBy(p => p.DateTime));
                }
                else
                {
                    logs.AddRange(db.Log.Where(p => p.CaseID == id||(p.CaseID==@case.OpCaseID && (p.Module.Communication== Communication.BSolicitorToSSolicitor||p.Module.Communication == Communication.SSolicitorToBSolicitor))).OrderBy(p => p.DateTime));
                }
            }
            else if (UserID == @case.ClientID)
            {
                logs.AddRange(db.Log.Where(p => p.CaseID == id && ((p.UserID==UserID)||(p.UserID==p.Case.SolicitorID&&(p.Module.Communication==Communication.SolicitorToBuyer||p.Module.Communication==Communication.SolicitorToSeller)&&p.Module.Type!=Models.Type.TK))).OrderBy(p => p.DateTime));
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CaseName = @case.CaseName;
            return View(logs);
        }
        private void log(string content, int CaseID)
        {
            Conveyance.Models.Log log = new Conveyance.Models.Log();
            log.DateTime = DateTime.Now;
            log.UserID = User.Identity.GetUserId();
            log.Content = content;
            log.CaseID = CaseID;
            db.Log.Add(log);
            db.SaveChanges();
        }

        public void GotoConveyance(int CaseID)
        {
            Instruction instruction = db.Cases.FirstOrDefault(p => p.CaseID == CaseID).Instruction;
            if (User.IsInRole("Solicitor"))
            {
                if (instruction == Instruction.Buy)
                    Response.Redirect(Url.Content("~") + "Conveyance/BuyerSolicitorPanel?CaseID=" + CaseID);
                else
                    Response.Redirect(Url.Content("~") + "Conveyance/SellerSolicitorPanel?CaseID=" + CaseID);
            }
            else if (User.IsInRole("Agent"))
                Response.Redirect(Url.Content("~") + "Conveyance/AgentPanel?CaseID=" + CaseID);
            else
            {
                if (instruction == Instruction.Buy)
                    Response.Redirect(Url.Content("~") + "Conveyance/BuyerPanel?CaseID=" + CaseID);
                else
                    Response.Redirect(Url.Content("~") + "Conveyance/SellerPanel?CaseID=" + CaseID);
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
