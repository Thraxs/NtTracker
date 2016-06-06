using System.Configuration;
using System.DirectoryServices.Protocols;
using System.Net;

namespace NtTracker.Security
{
    public static class AuthService
    {
        private const int ErrorLogonFailure = 0x31;

        /// <summary>
        /// Check user credentials against the LDAP server.
        /// </summary>
        /// <param name="user">LDAP user name.</param>
        /// <param name="password">LDAP user password.</param>
        /// <returns>bool value indicating if the credentials are correct. Null if error.</returns>
        public static bool? ValidateCredentials(string user, string password)
        {
            bool? valid = true;

            var ldapConfig = ConfigurationManager.AppSettings["LdapConnection"].Split(';');
            var connection = new LdapConnection(ldapConfig[0])
            {
                Credential = new NetworkCredential
                {
                    UserName = "cn=" + user + "," + ldapConfig[1],
                    Password = password
                },
                AuthType = AuthType.Basic,
                AutoBind = false
            };

            using (connection)
            {
                connection.SessionOptions.ProtocolVersion = 3;
                try
                {
                    connection.Bind();
                }
                catch (LdapException e)
                {
                    if (e.ErrorCode == ErrorLogonFailure)
                    {
                        valid = false;
                    }
                    else
                    {
                        valid = null;
                    }
                }
            }

            return valid;
        }
    }
}