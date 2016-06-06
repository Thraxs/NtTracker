using System.Collections.Generic;
using NtTracker.Models;
using NtTracker.ViewModels;
using PagedList;

namespace NtTracker.Services
{
    public interface IUserAccountService : IService
    {
        /// <summary>
        /// Find a user account by the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User with the given id or null if not found..</returns>
        UserAccount FindById(int id);

        /// <summary>
        /// Find a user account by the user name.
        /// </summary>
        /// <param name="userName">User name of the account.</param>
        /// <returns>User with the given user name or null if not found.</returns>
        UserAccount FindByUserName(string userName);

        /// <summary>
        /// Perform a search of user accounts with the given parameters and ordering.
        /// All the parameters are optional
        /// </summary>
        /// <param name="id">User account id.</param>
        /// <param name="userName">User account username.</param>
        /// <param name="type">User account type.</param>
        /// <param name="status">User account status.</param>
        /// <param name="registeredFrom">User accounts registered after this date.</param>
        /// <param name="registeredTo">User accounts registered before this date.</param>
        /// <param name="sorting">Sorting criteria.</param>
        /// <param name="pageNumber">Page number for listing pagination.</param>
        /// <param name="pageSize">Page size for listing pagination.</param>
        /// <returns>A paged list with the result of the search sorted as indicated.</returns>
        IPagedList<UserAccount> Search(string id, string userName, UserAccountType? type,
            UserAccountStatus? status, string registeredFrom, string registeredTo, 
            string sorting, int pageNumber, int pageSize);

        /// <summary>
        /// Check if a user is registered in the system.
        /// </summary>
        /// <param name="userName">User to be checked.</param>
        /// <returns>bool indicating if the user is registered.</returns>
        bool IsRegistered(string userName);

        /// <summary>
        /// Check user credentials using the external authentication service.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="password">User password.</param>
        /// <returns>bool value indicating if the credentials are correct. A null 
        /// value will be returned if there was an exception during authentication.</returns>
        bool? ValidateCredentials(string user, string password);

        /// <summary>
        /// Create a new user, save it to the database and
        /// return it with the new database id.
        /// </summary>
        /// <param name="userAccount">User to create.</param>
        /// <returns>Newly created user account.</returns>
        UserAccount Create(UserAccountViewModel userAccount);

        /// <summary>
        /// Update a user account latest login info after a correct login.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <param name="requestInfo">Info of the request.</param>
        /// <returns>Updated user account.</returns>
        UserAccount LoginSuccessful(UserAccount userAccount, string requestInfo);

        /// <summary>
        /// Update a user account after a failed login. Depending on the
        /// number of failed attempts, it may be locked.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <returns>Updated user account</returns>
        UserAccount LoginFailed(UserAccount userAccount);

        /// <summary>
        /// Checks if the given account is locked and, if it is locked,
        /// checks if it's time to unlock it. If so, it activates the account.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <returns>bool indicating if the account is locked or not.</returns>
        bool CheckLock(UserAccount userAccount);

        /// <summary>
        /// As an administrator, force a manual lock/unlock of the given
        /// user account. Unlock date and failed login attempts will not
        /// be taken into account and will be reset.
        /// </summary>
        /// <param name="userAccount">User account to lock/unlock.</param>
        /// <param name="lockStatus">Lock status to set to the account.</param>
        void ChangeLock(UserAccount userAccount, bool lockStatus);

        /// <summary>
        /// Delete the user account with the given id.
        /// </summary>
        /// <param name="id">Id of the user account to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        bool Delete(int id);
    }
}
