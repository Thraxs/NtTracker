using System.Configuration;
using System.Web.Mvc;
using NtTracker.Filters;

namespace NtTracker
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CultureFilter(defaultCulture: ConfigurationManager.AppSettings["DefaultCulture"]));
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TimeoutFilter());
        }
    }
}
