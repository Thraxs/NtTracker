using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace NtTracker
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieName = ".NtTracker.ApplicationCookie",
                LoginPath = new PathString("/UserAccount/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["AuthExpireTimeSpan"])),
                SlidingExpiration = bool.Parse(ConfigurationManager.AppSettings["AuthSlidingExpiration"]),
                Provider = new CookieAuthenticationProvider
                {
                    //Configure authentication redirection to redirect on login
                    OnApplyRedirect = context =>
                    {
                        var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

                        //Set redirectUrl
                        Uri uri = new Uri(context.RedirectUri);
                        string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];

                        RouteValueDictionary routeValues = new RouteValueDictionary
                        {
                            { context.Options.ReturnUrlParameter, returnUrl }
                        };

                        var actionUri = url.Action("Login", "UserAccount", routeValues);
                        context.Response.Redirect(actionUri);
                    }
                }
            });
        }
    }
}