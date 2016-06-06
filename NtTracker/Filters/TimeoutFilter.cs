using System.DirectoryServices.Protocols;
using System.Web.Mvc;
using System.Web.Routing;

namespace NtTracker.Filters
{
    public class TimeoutFilter : HandleErrorAttribute
    {
        /// <summary>
        /// If the user tries to log in while already logged, or log out after 
        /// his session has expired, redirect him to the login page instead of 
        /// showing an anti-forgery error.
        /// </summary>
        public TimeoutFilter()
        {
            ExceptionType = typeof (HttpAntiForgeryException);
        }

        public override void OnException(ExceptionContext filterContext)
        {
            var action = filterContext.RouteData.Values["action"].ToString();
            if (!(action.Equals("LogOff") || action.Equals("Login"))) return;

            //If exception is an LDAP authentication exception don't handle it
            if (filterContext.Exception is LdapException) return;

            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { action = "Login", controller = "UserAccount" }));
        }
    }
}