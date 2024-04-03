﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace eBaseApp.Models
{
    public class BaseType : BaseModel
    {
        [Required]
        [Column(Order = 10)]
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column(Order = 11)]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Column(Order = 12)]
        [Display(Name = "Key")]
        [MaxLength(100)]
        public string Key { get; set; }
    }
}