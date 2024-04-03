using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBaseApp.Models
{
    public class Contractor : BaseType
    {
        [Required]
        [Column(Order = 13)]
        [Display(Name = "Capacity")]
        public Int32 Capacity { get; set; }

        [Required]
        [Column(Order = 14)]
        [Display(Name = "Allocated")]
        public Int32 Allocated { get; set; }
    }
}