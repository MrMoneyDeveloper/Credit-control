using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace eBaseApp.Models
{
    public class AppSetting : BaseType
    {
        [Required]
        [Column(Order = 20)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}