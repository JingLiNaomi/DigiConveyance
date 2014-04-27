using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public enum Progress
    {NotStarted,InProcess,Completed }

    public enum Instruction
    { Buy, Sell }
    public class Case
    {
        public int CaseID { get; set; }
        [Display(Name = "Case Name")]
        public string CaseName { get; set; }
        [Display(Name = "Create Time")]
        public DateTime? CreateDateTime { get; set; }
        [Display(Name = "Complete Time")]
        public DateTime? CompleteDateTime { get; set; }
        public bool IsCompleted { get; set; }
        public string Reference { get; set; }
        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public Instruction Instruction { get; set; }
        [ForeignKey("OpCase")]
        public int? OpCaseID { get; set; }
        public string ClientID { get; set; }
        public string SolicitorID { get; set; }
        public string AgentID { get; set; }
        [ForeignKey("ClientModuleSet")]
        public int ClientModuleSetID { get; set; }
        [ForeignKey("SolicitorModuleSet")]
        public int? SolicitorModuleSetID { get; set; }
        public int TemplateID { get; set; }
        public DateTime? StartDateA { get; set; }
        public DateTime? CompleteDateA { get; set; }
        public DateTime? StartDateB { get; set; }
        public DateTime? CompleteDateB { get; set; }
        public DateTime? StartDateC { get; set; }
        public DateTime? CompleteDateC { get; set; }
        public DateTime? StartDateD { get; set; }
        public DateTime? CompleteDateD { get; set; }
        public DateTime? StartDateE { get; set; }
        public DateTime? CompleteDateE { get; set; }
        public DateTime? StartDateF { get; set; }
        public DateTime? CompleteDateF { get; set; }
        public Progress ProgressA { get; set; }
        public Progress ProgressB { get; set; }
        public Progress ProgressC { get; set; }
        public Progress ProgressD { get; set; }
        public Progress ProgressE { get; set; }
        public Progress ProgressF { get; set; }
        public virtual ApplicationUser Client { get; set; }
        public virtual ApplicationUser Solicitor { get; set; }
        public virtual Case OpCase { get; set; }
        public virtual City City { get; set; }
        public virtual ModuleSet ClientModuleSet { get; set; }
        public virtual ModuleSet SolicitorModuleSet { get; set; }
        public virtual Template Template { get; set; }

    }
}