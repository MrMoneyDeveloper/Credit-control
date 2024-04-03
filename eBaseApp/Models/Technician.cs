using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace eBaseApp.Models
{
    public class Technician : BaseModel
    {
        [Column(Order = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Column(Order = 3)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Column(Order = 4)]
        [Display(Name = "Mobile")]
        public string MobileNumber { get; set; }

        [Column(Order = 5)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Column(Order = 6)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Column(Order = 7)]
        public Int32 ContractorId { get; set; }
        [ForeignKey("ContractorId")]
        [Display(Name = "Contractor")]
        public Contractor Contractor { get; set; }


        [NotMapped]
        public String FullName
        {
            get
            {
                return String.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }
    }



}