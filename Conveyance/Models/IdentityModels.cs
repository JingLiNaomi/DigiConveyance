using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Conveyance.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string Webpage { get; set; }
        public virtual City City { get; set; }
      

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {

        }

        public DbSet<Case> Cases { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<ModuleSet> ModuleSet { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<Template> Template { get; set; }

    }
}