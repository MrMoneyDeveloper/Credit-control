using eBaseApp.DataAccessLayer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace eBaseApp.App_Start
{
    public class StartupAuthHelper
    {
        public static ClaimsIdentity GetIdentityUiclaims(ClaimsIdentity id,IEnumerable<Claim> uiclaims,  JObject payload) 
        {
            id.AddClaims(uiclaims);
            id.AddClaim(new Claim(OpenIdConnectParameterNames.AccessToken, payload.Value<string>(OpenIdConnectParameterNames.AccessToken)));
            id.AddClaim(new Claim(OpenIdConnectParameterNames.ExpiresIn, DateTime.Now.AddSeconds(payload.Value<double>(OpenIdConnectParameterNames.ExpiresIn)).ToLocalTime().ToString()));
            id.AddClaim(new Claim(OpenIdConnectParameterNames.RefreshToken, payload.Value<string>(OpenIdConnectParameterNames.RefreshToken)));
            id.AddClaim(new Claim(OpenIdConnectParameterNames.IdToken, payload.Value<string>(OpenIdConnectParameterNames.IdToken)));
            id.AddClaim(new Claim(OpenIdConnectParameterNames.TokenType, payload.Value<string>(OpenIdConnectParameterNames.TokenType)));
            id.AddClaim(new Claim(OpenIdConnectParameterNames.Scope, payload.Value<string>(OpenIdConnectParameterNames.Scope)));
            id.AddClaim(new Claim(ClaimTypes.Authentication, "true"));
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, uiclaims.SingleOrDefault(d => d.Type == "sub").Value));

            return Uiclaims(id, uiclaims);
        }
        static ClaimsIdentity Uiclaims(ClaimsIdentity id, IEnumerable<Claim> uiclaims)
        {
            foreach (var c in uiclaims)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", c.Type, c.Value));
                switch (c.Type)
                {
                    case "sub":
                        id.AddClaim(new Claim(ClaimTypes.Name, c.Value));
                        break;
                    case "groups":
                        foreach (var r in wRoles(c.Value))
                        {
                            String role = String.Empty;

                            if (r.Contains("/")) 
                                role = r.Substring(r.IndexOf("/") + 1);

                            if (r.Contains("_")) 
                                role = role.Replace("_", " ");

                            id.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                        break;
                    case "email":
                        id.AddClaim(new Claim(ClaimTypes.NameIdentifier, c.Value));
                        id.AddClaim(new Claim(ClaimTypes.Email, c.Value));
                        break;
                    default:
                        break;
                }
            }
            return id;
        }
        static IEnumerable<String> wRoles(String m_value) => m_value.Split(',').ToList();
    }
}