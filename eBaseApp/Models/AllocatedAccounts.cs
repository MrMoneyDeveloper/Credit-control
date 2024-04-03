using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using eBaseApp.DataAccessLayer;

namespace eBaseApp.Models
{
    public class AllocatedAccount : BaseModel
    {
        [Column(Order = 2)]
        public Int32 ContractorId { get; set; }
        [ForeignKey("ContractorId")]
        [Display(Name = "Contractor")]
        [ScriptIgnore]
        public Contractor Contractor { get; set; }

        [Column(Order = 3)]
        public Int32 ActionTypeId { get; set; }
        [ForeignKey("ActionTypeId")]
        [Display(Name = "Action Type")]
        [ScriptIgnore]
        public ActionType ActionType { get; set; }

        [Column(Order = 4)]
        [Display(Name = "Account")]
        public Int32 CUT_OFF_PROCESSING_ID { get; set; }

        [Column(Order = 5)]
        public Int32? StatusId { get; set; }
        [ForeignKey("StatusId")]
        [Display(Name = "Status")]
        [ScriptIgnore]
        public Status Status { get; set; }

        [NotMapped]
        public R_CC_CUT_OFF_PROCESSING R_CC_CUT_OFF_PROCESSING { get; set; }
    }
}