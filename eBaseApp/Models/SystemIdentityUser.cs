using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eBaseApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class SystemIdentityUser : IdentityUser
    {
        [Required]
        public int SystemUserId { get; set; }

        public string UnconfirmedEmail { get; set; }

        public string ServiceNo { get; set; }
        public bool RoundRobinIsActive { get; set; }
        public bool isInternalUser { get; set; }
        public bool isActiveDirectoryUser { get; set; }
        public bool isDeleted { get; set; }

        [ForeignKey("SystemUserId")]
        public virtual SystemUser SystemUser { get; set; }

        internal bool IsInRole()
        {
            throw new NotImplementedException();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SystemIdentityUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }


}