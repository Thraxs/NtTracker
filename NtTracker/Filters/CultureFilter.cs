using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace NtTracker.Filters
{
    public class CultureFilter : IAuthorizationFilter
    {
        private readonly string _defaultCulture;

        /// <summary>
        /// Change the culture of the current thread depending 
        /// on the route values.
        /// </summary>
        /// <param name="defaultCulture">Culture to use if one is not specified.</param>
        public CultureFilter(string defaultCulture)
        {
            _defaultCulture = defaultCulture;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var routeValues = filterContext.RouteData.Values;
            var culture = (string)routeValues["culture"] ?? _defaultCulture;
            CultureInfo cultureInfo;

            try {
                cultureInfo = new CultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
                //Culture doesn't exist, use default
                cultureInfo = new CultureInfo(_defaultCulture);
                filterContext.RouteData.Values["culture"] = _defaultCulture;
            }

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }
    }
}