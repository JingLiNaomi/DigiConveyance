using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public class ConveyanceViewModel
    {
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Comment> Comments  { get; set; }
        public IEnumerable<File> Files { get; set; }
        public string Tab { get; set; }
        public Case Case { get; set; }
    }
}