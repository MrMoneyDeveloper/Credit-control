using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBaseApp.Models
{
    public class RoundRobinQueue : BaseModel
    {
        [Column(Order = 10)]
        [Display(Name = "Responsibility Type")]
        public Int32 AssignedToUserId { get; set; }

        [Column(Order = 11)]
        [Display(Name = "Responsibility Type")]
        public int ResponsibilityTypeId { get; set; }
        [ForeignKey("ResponsibilityTypeId")]
        public ResponsibilityType ResponsibilityType { get; set; }

        [Column(Order = 12)]
        [Display(Name = "Allocated Account")]
        public int AllocatedAccountId { get; set; }
        [ForeignKey("AllocatedAccountId")]
        public AllocatedAccount AllocatedAccount { get; set; }

        [Column(Order = 13)]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Current Task Start Date")]
        [Column(Order = 14)]
        public DateTime? CurrentTaskDateTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Current Task Start Date")]
        [Column(Order = 15)]
        public DateTime? EndTaskDateTime { get; set; }
    }
}