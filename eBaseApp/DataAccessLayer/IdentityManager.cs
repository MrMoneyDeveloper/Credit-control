using eBaseApp.Controllers;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using eBaseApp.Models;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using eBaseApp.App_Start;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Helpers;
using System.Security.Principal;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using IdentityModel;
using eBaseApp.Keys;
using eBaseApp.Helpers;
using System.Text;
using System.Configuration;
using eBaseApp.Models.Cesar;
using static System.Net.Mime.MediaTypeNames;
using System.Web.Security;
using System.Net.Mail;
using System.Net.PeerToPeer;
using System.Data;

namespace eBaseApp.DataAccessLayer
{
    public class IdentityManager
    {
        private eServicesDbContext core;

        public IdentityManager()
        {
            core = new eServicesDbContext();
            UserManager = new UserManager<SystemIdentityUser>(new UserStore<SystemIdentityUser>(core));
        }

        public IdentityManager(eServicesDbContext context)
        {
            core = context;
            UserManager = new UserManager<SystemIdentityUser>(new UserStore<SystemIdentityUser>(core));
        }

        public UserManager<SystemIdentityUser> UserManager { get; set; }
        public String CurrentUser { get; set; }
        public Boolean RoleExists(string name) => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(core)).RoleExists(name);
        public Boolean UserExists(string name) => new UserManager<SystemIdentityUser>(new UserStore<SystemIdentityUser>(core)).FindByName(name) != null;
        public Boolean CreateRole(string name) => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(core)).Create(new IdentityRole(name)).Succeeded;
        public Boolean CreateUser(SystemIdentityUser user, string password) => UserManager.Create(user, password).Succeeded;
        public Boolean AddUserToRole(string userId, string roleName) => UserManager.AddToRole(userId, roleName).Succeeded;
        public Boolean RemoveUserInRole(string userId, string roleName) => UserManager.RemoveFromRole(userId, roleName).Succeeded;
        public Boolean FindByEmailOrName(string name) => (new UserManager<SystemIdentityUser>(new UserStore<SystemIdentityUser>(core)).FindByName(name) == null && new UserManager<SystemIdentityUser>(new UserStore<SystemIdentityUser>(core)).FindByEmail(name) == null);
        private SystemIdentityUser aIdentityUser(String email, String username) => UserManager.FindByEmail(email) ?? UserManager.FindByName(username);
        public SystemUser CreateIdentitySystemUser(List<Claim> claims)
        {
            try
            {
                String emailAddress = String.Empty;
                String phone = String.Empty;
                String firstName = String.Empty;
                String lastName = String.Empty;
                List<String> roles = new List<String>();

                ClaimsPrincipal identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                String username = identity.Claims.Where(c => c.Type == "sub")
                        .Select(c => c.Value).SingleOrDefault();

                if (String.IsNullOrEmpty(username)) return null;

                foreach (var c in claims)
                {
                    switch (c.Type)
                    {
                        case IamKeys.Roles:
                            String r = c.Value;
                            List<String> rls = r.Split(',').ToList();
                            foreach (var d in rls)
                                roles.Add(d.Split('/').LastOrDefault());
                            roles.Remove("everyone");
                            break;
                        case IamKeys.PhoneNumber:
                            phone = c.Value;
                            break;
                        case IamKeys.GivenName:
                            firstName = c.Value;
                            break;
                        case IamKeys.FamilyName:
                            lastName = c.Value;
                            break;
                        case IamKeys.EmailAddress:
                            emailAddress = c.Value;
                            break;
                    }
                }

                EkuActiveDirectory ekuActiveDirectoryUser = new EkurhuleniActiveDirectory().GetEkuDetails(username);
                SystemIdentityUser user = new SystemIdentityUser
                {
                    UserName = username,
                    Email = String.IsNullOrEmpty( emailAddress ) ? ekuActiveDirectoryUser.EmailAddress : emailAddress,
                    PhoneNumber = phone,
                    PhoneNumberConfirmed = true,
                    isActiveDirectoryUser = true
                };
                String code = PasswordGenerator.GeneratePassword( true, true, true, false, false, 6 );

                user = new SystemIdentityUser
                {
                    UserName = username,
                    Email = String.IsNullOrEmpty(emailAddress) ? ekuActiveDirectoryUser.EmailAddress : emailAddress,
                    EmailConfirmed = true,
                    PhoneNumber = phone ?? ekuActiveDirectoryUser.MobileNumber,
                    isInternalUser = true,
                    isActiveDirectoryUser = true,
                    RoundRobinIsActive = true,
                    ServiceNo = ekuActiveDirectoryUser.EmployeeNumber ?? GeneratePassword(6),
                    SystemUser = new SystemUser()
                    {
                        FirstName = firstName ?? ekuActiveDirectoryUser.FirstName ?? user.UserName,
                        LastName = String.IsNullOrEmpty(lastName) ? ekuActiveDirectoryUser.LastName : lastName,
                        UserName = username ?? ekuActiveDirectoryUser.UserName,
                        MobileNumber = phone ?? ekuActiveDirectoryUser.MobileNumber,
                        IdentificationNumber = ekuActiveDirectoryUser.IdentificationNumber,
                        EmailAddress = String.IsNullOrEmpty( emailAddress ) ? ekuActiveDirectoryUser.EmailAddress : emailAddress,
                        IsPasswordReset = true,
                        ServiceNo = ekuActiveDirectoryUser.EmployeeNumber ?? GeneratePassword( 6 ),
                        IsActive = true,
                        IsDeleted = false,
                        IsLocked = false,
                        IsIAMRegistered = true,
                        ModifiedDateTime = DateTime.Now,
                        isInternalUser = true,
                        isActiveDirectoryUser = true,
                    }
                };

                String randomPassword = GeneratePassword( 10 );
                IdentityResult result = UserManager.Create( user, randomPassword ) ?? new IdentityResult();

                if (result.Succeeded)
                {

                    String EmailOrSmsMessage = string.Empty;
                    String EmailOrSmsMessage2 = string.Empty;
                    List<String> Wso2_roles = ConfigurationManager.AppSettings["Wso2Roles"].Split('|').ToList();
                    List<String> Application_roles = ConfigurationManager.AppSettings["SystemRoles"].Split('|').ToList();

                    if (roles.Intersect(Wso2_roles).Any())
                    {
                        for (int i = 0; i < roles.Count(); i++)
                        {
                            String cRole = roles[i];
                            if (Wso2_roles.Contains(cRole))
                            {
                                int Index = Wso2_roles.FindIndex(w => w.Contains(cRole));
                                String aRole = Application_roles[Index];
                                AddUserToRole(user.Id, aRole);
                            }
                        }

                        EmailOrSmsMessage = "Please note the following Active " +
                                            "Directory User has been registered " +
                                            "in the system successfully";
                    }
                    else
                    {
                        EmailOrSmsMessage = "Please note the following Active Directory User has been registered in " +
                                            "the system with no roles assiged to it, see the registration information " +
                                            "to assign relavant roles in active directory";
                    }

                    String registerdUser = string.Format(
                        "<br/><br/><b>Active Directory Registration</b><br/>Username: {0}<br/>Full Name: {1}<br/>Email: {2}<br/>",
                        user.UserName, user.SystemUser.FullName, user.SystemUser.EmailAddress);

                    Email email = new Email();
                    CesarSMS sms = new CesarSMS();
                    Models.Application applicationAccess =
                        core.Applications.Where(x => x.Key == ApplicationKeys.eBaseApplication).FirstOrDefault();
                    Status statusIdsms = core.Status.FirstOrDefault(o => o.Key == StatusKeys.SMSPending);
                    List<SystemIdentityUser> BackOfficeAdmins = GetUsersInRole("Administrators").Where(d => d.Id != user.Id).ToList();

                    foreach (SystemIdentityUser backOfficeAdmin in BackOfficeAdmins)
                    {
                        email.GenerateEmail(backOfficeAdmin.SystemUser.EmailAddress, String.Format("{0}: Active Directory User Registraion", applicationAccess.ApplicationAbbreviation),
                                    EmailOrSmsMessage + registerdUser,
                                   backOfficeAdmin.SystemUser.Id.ToString(CultureInfo.InvariantCulture), false, AppSettingKeys.EservicesDefaultEmailTemplate, backOfficeAdmin.SystemUser.FullName);

                        if (backOfficeAdmin.SystemUser.EmailAddress == null && backOfficeAdmin.SystemUser.MobileNumber != null)
                            sms.GenerateSMS(backOfficeAdmin.SystemUser.MobileNumber,
                                             EmailOrSmsMessage + registerdUser,
                                           backOfficeAdmin.SystemUser.Id.ToString(CultureInfo.InvariantCulture), statusIdsms.Id, backOfficeAdmin.SystemUser.FullName);
                    }

                    //Email registering user
                    List<String> SelectedRoles = UserManager.GetRoles(user.Id).ToList();
                    String rolesArray = String.Empty;
                    foreach (var item in SelectedRoles)
                    {
                        if (String.IsNullOrEmpty(rolesArray))
                            rolesArray = String.Format("{0}", item);
                        else
                            rolesArray = String.Format("{0},{1}", rolesArray, item);
                    }

                    String emailSubject = String.Format("{0} System: User Registration", applicationAccess.Name);
                    String emailBody = String.Empty;

                    emailBody = String.Format("<b>You have been successfully added onto {0} System.</b><br/><br/>", applicationAccess.Name) +
                      "<b>Login Details:</b><br/>" +
                      "Active Directory Username: " + user.UserName + "<br/>" +
                      "Password: " + "Please use your own Active Directory password<br/>" +
                      "Application Access: " + applicationAccess.Name + "<br/>" +
                      "Role(s): " + rolesArray ?? "No roles assigned<br/><br/>";

                    email.GenerateEmail(user.Email, emailSubject, emailBody, user.SystemUserId.ToString(), false, AppSettingKeys.EservicesDefaultEmailTemplate, user.SystemUser.FullName);
                }
                else
                {
                    return user?.SystemUser;
                }

                return user?.SystemUser ?? new SystemUser();
            }
            catch (Exception)
            {
                return new SystemUser();
            }
        }

        public bool UpdateIdentitySystemUser(String emailAddress, String Username)
        {
            Boolean result = false;
            try
            {
                SystemIdentityUser aUser = aIdentityUser(emailAddress, Username);
                if (aUser == null) return result;

                List<String> roles = new List<String>();
                List<String> Wso2_roles = ConfigurationManager.AppSettings["Wso2Roles"].Split('|').ToList();
                List<String> Application_roles = ConfigurationManager.AppSettings["SystemRoles"].Split('|').ToList();
                ClaimsPrincipal identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                String wRoles = identity.Claims.Where(c => c.Type == IamKeys.Roles)
                        .Select(c => c.Value).SingleOrDefault();
                List<String> aRoles = GetUserRoles(aUser.Id) as List<String>;

                foreach (var d in wRoles.Split(',').ToList())
                    roles.Add(d.Split('/').LastOrDefault());


                if (roles.Intersect(Wso2_roles).Any())
                {
                    for (int i = 0; i < roles.Count(); i++)
                    {
                        String cRole = roles[i];
                        if (Wso2_roles.Contains(cRole))
                        {
                            int Index = Wso2_roles.FindIndex(w => w.Contains(cRole));
                            String aRole = Application_roles[Index];
                            if (!aRoles.Contains(aRole))
                                AddUserToRole(aUser.Id, aRole);
                        }
                    }
                }


                for (int i = 0; i < aRoles.Count(); i++)
                {
                    String cRole = aRoles[i];
                    int Index = Application_roles.FindIndex(w => w.Contains(cRole));
                    String aRole = Wso2_roles[Index];
                    if (!roles.Contains(aRole))
                        AddUserToRole(aUser.Id, cRole);
                }
                    
                result = true;
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

        public IEnumerable<String> GetUserRoles(String UserId) => UserManager.GetRoles(UserId);

        public IEnumerable<SystemIdentityUser> GetUsersInRole(String role)
        {
            String roleId = core.Roles.Where(x => x.Name == role).FirstOrDefault().Id;

            if (roleId == null)
                return Enumerable.Empty<SystemIdentityUser>();

            return UserManager.Users.Where(o => o.Roles.Any(s => s.RoleId == roleId)).ToList();
        }

        #region Generate User Password
        public string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder();
            var rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        #endregion
    }
}