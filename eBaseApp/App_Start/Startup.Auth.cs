#region using
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using eBaseApp.Models;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security;
using System.Net;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Web.Helpers;
using System.Security.Claims;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using eBaseApp.DataAccessLayer;
using System.Configuration;
using eBaseApp.App_Start;
#endregion

namespace eBaseApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(eServicesDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                { 
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, SystemIdentityUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            #region // Uncomment the following lines to enable logging in with third party login providers
            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
            #endregion

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(),
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                Authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["Authority"], ConfigurationManager.AppSettings["Tenant"]),
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["logout"],

                #region // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                //PostLogoutRedirectUri = redirectUri,
                #endregion
                
                Scope = OpenIdConnectScope.OpenId,
                SaveTokens = true,
                ResponseType = OpenIdConnectResponseType.Code,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false // This is a simplification
                },

                Configuration = new OpenIdConnectConfiguration
                {
                    AuthorizationEndpoint = string.Format("{0}/oauth2/authorize", ConfigurationManager.AppSettings["Authority"]),
                    TokenEndpoint = string.Format("{0}/oauth2/token", ConfigurationManager.AppSettings["Authority"]),
                    UserInfoEndpoint = string.Format("{0}/oauth2/userinfo", ConfigurationManager.AppSettings["Authority"])
                },

                #region // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                #endregion

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed,

                    AuthorizationCodeReceived = async (notification) =>
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            #region // JK.20210826a - Used to change the unique identifier claim for the users.

                            #endregion

                            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;

                            OpenIdConnectConfiguration configuration = await notification.Options.ConfigurationManager.GetConfigurationAsync(notification.Request.CallCancelled);
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenEndpoint);
                            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                {OpenIdConnectParameterNames.ClientId, notification.Options.ClientId},
                                {OpenIdConnectParameterNames.ClientSecret, notification.Options.ClientSecret},
                                {OpenIdConnectParameterNames.Code, notification.ProtocolMessage.Code},
                                {OpenIdConnectParameterNames.GrantType, "authorization_code"},
                                {OpenIdConnectParameterNames.ResponseType, "code"},
                                {OpenIdConnectParameterNames.RedirectUri, notification.Options.RedirectUri}
                            });

                            HttpResponseMessage response = null;

                            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                            response = await client.SendAsync(request, notification.Request.CallCancelled);
                            response.EnsureSuccessStatusCode();

                            JObject payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                            #region // use the access token to retrieve claims from userinfo

                            #endregion

                            HttpClient uiclient = new HttpClient();
                            UserInfoResponse uiresponse = await uiclient.GetUserInfoAsync(new UserInfoRequest
                            {
                                Address = string.Format("{0}/oauth2/userinfo", ConfigurationManager.AppSettings["Authority"]),
                                Token = payload.Value<string>(OpenIdConnectParameterNames.AccessToken)
                            });

                            if (uiresponse.IsError) throw new Exception(uiresponse.Error);

                            IEnumerable<Claim> uiclaims = uiresponse.Claims;

                            AuthenticationProperties props = new AuthenticationProperties();
                            notification.AuthenticationTicket = new AuthenticationTicket(StartupAuthHelper.GetIdentityUiclaims
                                (new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.GivenName, ClaimTypes.Role), uiclaims, payload), props);
                        }
                    }
                }
            });
        }

        #region /// <summary>
   /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        #endregion
     
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}