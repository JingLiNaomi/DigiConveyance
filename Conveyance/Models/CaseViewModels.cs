using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class CreateCaseViewModel
    {
        [Required]
        public Instruction Instruction { get; set; }
        [Required]
        [StringLength(15,ErrorMessage="Maximum 15 characters")]
        public string Reference { get; set; }
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Maximum 10 characters")]
        public string Postcode { get; set; }
    }

    public class InviteCaseViewModel
    {
        [Required]
        [Display(Name="Client's email")]
        [EmailAddress]
        public string Email { get; set; }
        public int CaseID { get; set; }
     
    }
}