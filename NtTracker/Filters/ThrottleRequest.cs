using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using NtTracker.Extensions;
using NtTracker.Resources.Shared;

namespace NtTracker.Filters
{
    /// <summary>
    /// Limits the number of requests that can be made for this controller action.
    /// </summary>
    public class ThrottleRequest : ActionFilterAttribute
    {
        /// <summary>
        /// Minimum time between requests.
        /// </summary>
        public int RequestDelay = 10;

        /// <summary>
        /// Shared strings resource key for the error message
        /// shown to the user in the validation section.
        /// </summary>
        public string ErrorResourceKey;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var cache = filterContext.HttpContext.Cache;

            //Request origin
            var originationInfo = request.HostAddress() + request.UserAgent;

            //Request target
            var targetInfo = filterContext.RouteData.Values["controller"].ToString() 
                + filterContext.RouteData.Values["action"] 
                + request.QueryString;

            //Hash request info
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            //Check if request is in the cache
            if (cache[hashValue] != null)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, SharedStrings.ResourceManager.GetString(ErrorResourceKey));
            }
            else
            {
                //Add request to the cache
                cache.Add(hashValue, true, null, DateTime.Now.AddSeconds(RequestDelay), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}