using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace NtTracker.Extensions
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Builds URL by finding the best matching route that corresponds to the current URL,
        /// with given parameters added or replaced.
        /// </summary>
        /// <param name="helper">URL helper</param>
        /// <param name="substitutes">Properties to be updated</param>
        /// <returns>Given URL with updated parameters</returns>
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            var routeData = new RouteValueDictionary(helper.RequestContext.RouteData.Values);
            var queryString = helper.RequestContext.HttpContext.Request.QueryString;

            //Add query string parameters to route data
            foreach (string param in queryString)
            {
                if (!string.IsNullOrEmpty(queryString[param]))
                {
                    routeData[param] = queryString[param];
                }
            }

            //Update the given properties
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = (string) property.GetValue(substitutes);
                if (string.IsNullOrEmpty(value))
                {
                    routeData.Remove(property.Name);
                }
                else
                {
                    routeData[property.Name] = value;
                }
            }

            //Generate new URL
            var url = helper.RouteUrl(routeData);
            return new MvcHtmlString(url);
        }

        /// <summary>
        /// Sets the class of a navigation element to active if the 
        /// current controller is one of the given ones.
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="controllers">Controllers that make this element active.</param>
        /// <returns>The active class or empty if is not active.</returns>
        public static string IsActive(this HtmlHelper html, string controllers = "")
        {
            var viewContext = html.ViewContext;
            var isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
            {
                viewContext = html.ViewContext.ParentActionViewContext;
            }

            var routeValues = viewContext.RouteData.Values;
            var currentController = routeValues["controller"].ToString();

            if (string.IsNullOrEmpty(controllers))
            {
                controllers = currentController;
            }

            var acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedControllers.Contains(currentController) ? "active" : string.Empty;
        }
    }
}