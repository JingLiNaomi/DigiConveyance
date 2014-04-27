using Conveyance.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Conveyance.Controllers
{
    [Authorize(Roles = "Admin")]
    
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        
        

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            //populate role dropdownlist
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Solicitor", Value = "Solicitor" });
            items.Add(new SelectListItem { Text = "Agent", Value = "Agent" });
            ViewBag.DropDownRoles = items;

            //populate city dropdownlist
            var Cities = db.City.ToList();
            List<SelectListItem> cityItems = new List<SelectListItem>();
            foreach (City c in Cities)
            {
                cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
            }

            ViewBag.DropDownCities = cityItems;
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegisterViewModel model, string DropDownRoles, string DropDownCities)
        {
            int count = db.Users.Count(p=>string.Compare(p.UserName,model.UserName)==0);
            if (count > 0)
            {
                //ERROR
                ViewBag.ErrorMsg = "Username exists already";

                //populate role dropdownlist
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Solicitor", Value = "Solicitor" });
                items.Add(new SelectListItem { Text = "Agent", Value = "Agent" });
                ViewBag.DropDownRoles = items;

                //populate city dropdownlist
                var Cities = db.City.ToList();
                List<SelectListItem> cityItems = new List<SelectListItem>();
                foreach (City c in Cities)
                {
                    cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
                }

                ViewBag.DropDownCities = cityItems;
                return View();
            
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Tel = model.Tel,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    CityID = Convert.ToInt16(DropDownCities),
                    Country = model.Country,
                    Postcode = model.Postcode,
                    Webpage = model.Webpage

                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    user = UserManager.FindByName(model.UserName);
                    UserManager.AddToRole(user.Id,DropDownRoles);
                    //SUCCESS
                    ViewBag.SuccessMsg = "User registered successfully";

                    //populate role dropdownlist
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "Solicitor", Value = "Solicitor" });
                    items.Add(new SelectListItem { Text = "Agent", Value = "Agent" });
                    ViewBag.DropDownRoles = items;

                    //populate city dropdownlist
                    var Cities = db.City.ToList();
                    List<SelectListItem> cityItems = new List<SelectListItem>();
                    foreach (City c in Cities)
                    {
                        cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
                    }

                    ViewBag.DropDownCities = cityItems;
                    return View();
                }
                else
                {
                    //ERROR
                    ViewBag.ErrorMsg = "Failed to register user, please try again later";

                    //populate role dropdownlist
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "Solicitor", Value = "Solicitor" });
                    items.Add(new SelectListItem { Text = "Agent", Value = "Agent" });
                    ViewBag.DropDownRoles = items;

                    //populate city dropdownlist
                    var Cities = db.City.ToList();
                    List<SelectListItem> cityItems = new List<SelectListItem>();
                    foreach (City c in Cities)
                    {
                        cityItems.Add(new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() });
                    }

                    ViewBag.DropDownCities = cityItems;
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form

            return View(model);
        }

        public ActionResult TemplateList()
        {
            return View(db.Template.Where(p=>true).ToList());
        }

        public ActionResult CreateTemplate()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTemplate([Bind(Include = "Name,Description")] Template template)
        {
            if (ModelState.IsValid)
            {
                template.CreateDateTime = DateTime.Now;
                //create new json file
                string fileName = template.Name + ".json";
                string pathString = Path.Combine(Server.MapPath("~/App_Data/Templates/"), fileName);

                if (!System.IO.File.Exists(pathString))
                {
                    System.IO.File.Create(pathString).Dispose();
                    template.Path = pathString;
                }
                else
                {
                    //file already exists, error
                    ViewBag.ErrMsg = "Name already exists";
                    return View();
                }

                db.Template.Add(template);
                db.SaveChanges();
                
                //retrieve newly inserted template
                Template t = db.Template.FirstOrDefault(p=>p.Name == template.Name);
                return RedirectToAction("Template", new { id = t.TemplateID });
            }

            ViewBag.ErrorMsg = "Creating new template failed, please try again later";
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Template template = db.Template.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }
            return View(template);
        }

     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Template template = db.Template.Find(id);
            //delete template json file
            if (System.IO.File.Exists(template.Path))
            {
                System.IO.File.Delete(template.Path);
            }
         
            //delete from database
            db.Template.Remove(template);
            db.SaveChanges();

            return RedirectToAction("TemplateList");
        }

        public ActionResult Template(int id)
        {
            Template template = db.Template.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }
            ViewBag.Path = template.Path;
            ViewBag.Name = template.Name;
            return View();
        }
        public string LoadModules(string path)
        {
            string text = System.IO.File.ReadAllText(@path);
            return text;
        }

        public string SaveModules(string Modules,string path)
        {
            try
            {
                //write string to a file
                System.IO.File.WriteAllText(@path, Modules);
                return "success";
            }
            catch
            {
                return "fail";
            }
           
        }
    
    
    
    }
}