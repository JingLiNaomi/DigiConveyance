using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Conveyance.Models
{
    public enum Stage
    { A,B,C,D,E,F}
    public enum Status
    { RequireAction, InAction,Completed}
    public enum Type
    { RQF, SDF, FORM, SDC, PKD, CFM, ADV,RQSIG,INF,TK }

    public enum Communication
    { SolicitorToSeller, SolicitorToBuyer,SSolicitorToBSolicitor,BSolicitorToSSolicitor,SellerToSolicitor,BuyerToSolicitor}
    public class Module
    {
        public int ModuleID { get; set; }
        public Type Type { get; set; }
        public string TextS { get; set; }
        public string TextC { get; set; }
        public bool Checked { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public bool Bool1 { get; set; }
        public bool Bool2 { get; set; }
        [ForeignKey("ModuleSet")]
        public int ModuleSetID { get; set; }
        public Stage Stage { get; set; }
        public Communication Communication { get; set; }
        public Instruction InitiateParty { get; set; }
        public int Position { get; set; }
        public int Attempt { get; set; }   //For SDF to count number of uploads
        public Status Status { get; set; }
        public virtual ModuleSet ModuleSet { get; set; }

    }
}