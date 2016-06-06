using System.Web;

namespace NtTracker.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Returns source address of the request checking first
        /// for a forwarded address and then the request address. 
        /// </summary>
        /// <param name="request">Request to get the address from.</param>
        /// <returns>Given URL with updated parameters</returns>
        public static string HostAddress(this HttpRequestBase request)
        {
            return request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
        }

        /// <summary>
        /// Returns source address of the request checking first
        /// for a forwarded address and then the request address. 
        /// </summary>
        /// <param name="request">Request to get the address from.</param>
        /// <returns>Given URL with updated parameters</returns>
        public static string HostAddress(this HttpRequest request)
        {
            return request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
        }
    }
}