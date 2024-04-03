using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace eBaseApp.Models
{
    public class EkuActiveDirectory
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmployeeNumber { get; set; }
        public string RoleArray { get; set; }
        public string EmailAddress { get; set; }
        public string SystemUserTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Code { get; set; }
        public string CCC { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public int StatusId { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool IsTemporaryPassword { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public Status Status { get; set; }
    }

}