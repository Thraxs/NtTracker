using System;
using System.Linq;
using System.Threading;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;
using PagedList;

namespace NtTracker.Services
{
    public class UserAccountService : Service, IUserAccountService
    {
        public UserAccountService(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Find a user account by the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User with the given id or null if not found.</returns>
        public UserAccount FindById(int id)
        {
            var account = Repository.UserAccounts
                .SingleOrDefault(u => u.Id == id);

            return account;
        }

        /// <summary>
        /// Find a user account by the user name.
        /// </summary>
        /// <param name="userName">User name of the account.</param>
        /// <returns>User with the given user name or null if not found.</returns>
        public UserAccount FindByUserName(string userName)
        {
            var searchString = userName.ToLower(Thread.CurrentThread.CurrentCulture);
            var user = Repository.UserAccounts
                .SingleOrDefault(u => u.UserName == searchString);

            return user;
        }

        /// <summary>
        /// Perform a search of user accounts with the given parameters and ordering.
        /// All the parameters are optional.
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
        public IPagedList<UserAccount> Search(string id, string userName, UserAccountType? type, 
            UserAccountStatus? status, string registeredFrom, string registeredTo, 
            string sorting, int pageNumber, int pageSize)
        {
            var accounts = Repository.UserAccounts.AsQueryable();

            //Filtering
            if (!string.IsNullOrEmpty(id))
            {
                int idComp;
                if (!int.TryParse(id, out idComp))
                {
                    idComp = -1;
                }
                accounts = accounts.Where(a => a.Id == idComp);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                accounts = accounts.Where(a => a.UserName.Contains(userName));
            }
            if (type != null)
            {
                var isAdmin = type == UserAccountType.Admin;
                accounts = accounts.Where(a => a.IsAdmin == isAdmin);
            }
            if (status != null)
            {
                var isLocked = status == UserAccountStatus.Locked;
                accounts = accounts.Where(a => a.IsLocked == isLocked);
            }
            if (!string.IsNullOrEmpty(registeredFrom))
            {
                DateTime dateComp;
                if (!DateTime.TryParse(registeredFrom, out dateComp))
                {
                    dateComp = DateTime.MinValue;
                }
                accounts = accounts.Where(a => a.RegistrationDate >= dateComp);
            }
            if (!string.IsNullOrEmpty(registeredTo))
            {
                DateTime dateComp;
                if (!DateTime.TryParse(registeredTo, out dateComp))
                {
                    dateComp = DateTime.MaxValue;
                }
                accounts = accounts.Where(a => a.RegistrationDate <= dateComp);
            }

            //Sorting
            switch (sorting)
            {
                case "id_a":
                    accounts = accounts.OrderBy(a => a.Id);
                    break;
                case "id_d":
                    accounts = accounts.OrderByDescending(a => a.Id);
                    break;
                case "userName_a":
                    accounts = accounts.OrderBy(a => a.UserName);
                    break;
                case "userName_d":
                    accounts = accounts.OrderByDescending(a => a.UserName);
                    break;
                case "registration_a":
                    accounts = accounts.OrderBy(a => a.RegistrationDate);
                    break;
                case "registration_d":
                    accounts = accounts.OrderByDescending(a => a.RegistrationDate);
                    break;
                case "isAdmin_a":
                    accounts = accounts.OrderBy(a => a.IsAdmin);
                    break;
                case "isAdmin_d":
                    accounts = accounts.OrderByDescending(a => a.IsAdmin);
                    break;
                case "isLocked_a":
                    accounts = accounts.OrderBy(a => a.IsLocked);
                    break;
                case "isLocked_d":
                    accounts = accounts.OrderByDescending(a => a.IsLocked);
                    break;
                default:
                    accounts = accounts.OrderByDescending(a => a.Id);
                    break;
            }

            return accounts.ToPagedList(pageNumber, pageSize);
        }

        /// <summary>
        /// Check if a user is registered in the system.
        /// </summary>
        /// <param name="userName">User to be checked.</param>
        /// <returns>bool indicating if the user is registered.</returns>
        public bool IsRegistered(string userName)
        {
            var exists = Repository.UserAccounts.Any(u => u.UserName == userName);
            return exists;
        }

        /// <summary>
        /// Check user credentials using the external authentication service.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="password">User password.</param>
        /// <returns>bool value indicating if the credentials are correct. A null 
        /// value will be returned if there was an exception during authentication.</returns>
        public bool? ValidateCredentials(string user, string password)
        {
            return Security.AuthService.ValidateCredentials(user, password);
        }

        /// <summary>
        /// Create a new user, save it to the database and
        /// return it with the new database id.
        /// </summary>
        /// <param name="userAccount">User to create.</param>
        /// <returns>Newly created user account.</returns>
        public UserAccount Create(UserAccountViewModel userAccount)
        {
            //Create new user
            UserAccount newUser = new UserAccount
            {
                UserName = userAccount.UserName,
                IsAdmin = false,
                RegistrationDate = DateTime.Now,
                FailedLoginAttempts = 0
            };

            //Save the new user
            Repository.UserAccounts.Add(newUser);
            Save();

            return newUser;
        }

        /// <summary>
        /// Update a user account latest login info after a correct login.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <param name="requestInfo">Info of the request.</param>
        /// <returns>Updated user account.</returns>
        public UserAccount LoginSuccessful(UserAccount userAccount, string requestInfo)
        {
            //Update login information and reset failed attempts
            userAccount.LastLogin = DateTime.Now;
            userAccount.LastLoginInfo = requestInfo;
            userAccount.FailedLoginAttempts = 0;
            Save();

            return userAccount;
        }

        /// <summary>
        /// Update a user account after a failed login. Depending on the
        /// number of failed attempts, it may be locked.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <returns>Updated user account</returns>
        public UserAccount LoginFailed(UserAccount userAccount)
        {
            //Add a failed login
            userAccount.FailedLoginAttempts++;

            //If more than 2 failed log in attempts, lock account
            if (userAccount.FailedLoginAttempts > 2)
            {
                userAccount.IsLocked = true;

                switch (userAccount.FailedLoginAttempts)
                {
                    case 3:
                        //After 3 tries lock account for 1 hour
                        userAccount.UnlockDate = DateTime.Now.AddHours(1);
                        break;
                    case 5:
                        //After 5 tries lock account for 24 hours
                        userAccount.UnlockDate = DateTime.Now.AddHours(24);
                        break;
                    case 7:
                        //After 7 tries lock account for 1 week
                        userAccount.UnlockDate = DateTime.Now.AddDays(7);
                        break;
                    default:
                        if (userAccount.FailedLoginAttempts >= 10)
                        {
                            //After 10 tries lock account forever (needs manual unlock from admin)
                            userAccount.UnlockDate = null;
                        }
                        break;
                }
            }

            Save();

            return userAccount;
        }

        /// <summary>
        /// Checks if the given account is locked and, if it is locked,
        /// checks if it's time to unlock it. If so, it activates the account.
        /// </summary>
        /// <param name="userAccount">User to update.</param>
        /// <returns>bool indicating if the account is locked or not.</returns>
        public bool CheckLock(UserAccount userAccount)
        {
            //Account is not locked
            if (!userAccount.IsLocked) return false;
            //Account is locked indefinitely 
            if (userAccount.UnlockDate == null) return true;
            //Account unlock date is in the future
            if (!(userAccount.UnlockDate < DateTime.Now)) return true;

            //Unlock account and save
            userAccount.IsLocked = false;
            userAccount.UnlockDate = null;
            Save();

            return false;
        }

        /// <summary>
        /// As an administrator, force a manual lock/unlock of the given
        /// user account. Unlock date and failed login attempts will not
        /// be taken into account and will be reset.
        /// </summary>
        /// <param name="userAccount">User account to lock/unlock.</param>
        /// <param name="lockStatus">Lock status to set to the account.</param>
        public void ChangeLock(UserAccount userAccount, bool lockStatus)
        {
            userAccount.IsLocked = lockStatus;
            userAccount.UnlockDate = null;
            userAccount.FailedLoginAttempts = 0;

            Save();
        }

        /// <summary>
        /// Delete the user account with the given id.
        /// </summary>
        /// <param name="id">Id of the user account to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        public bool Delete(int id)
        {
            var account = Repository.UserAccounts.SingleOrDefault(u => u.Id == id);
            if (account == null) return false;

            //Load related entities to remove references
            Repository.Entry(account).Collection(u => u.PatientsRegistered).Load();
            Repository.Entry(account).Collection(u => u.Operations).Load();

            Repository.UserAccounts.Remove(account);

            Save();
            return true;
        }
    }
}