using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class Template
    {
        public int TemplateID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Maximum 30 characters")]
        public string Name { get; set; }
        public string Path { get; set; }
         [StringLength(200, ErrorMessage = "Maximum 200 characters")]
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}