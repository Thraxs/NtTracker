using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NtTracker.Extensions;
using NtTracker.Filters;
using NtTracker.Models;
using NtTracker.Resources.UserAccount;
using NtTracker.Services;
using NtTracker.ViewModels;

namespace NtTracker.Controllers
{
    public class UserAccountController : Controller
    {
        const int PageSize = 10;

        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(IAuthenticationManager authenticationManager, IUserAccountService userAccountService)
        {
            _authenticationManager = authenticationManager;
            _userAccountService = userAccountService;
        }

        // GET: UserAccount/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: UserAccount/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ThrottleRequest(RequestDelay = 60, ErrorResourceKey = "RegistrationThrottleError")]
        public ActionResult Register(UserAccountViewModel user)
        {
            if (!ModelState.IsValid) return View(user);

            //If user is already registered show message   
            if (_userAccountService.IsRegistered(user.UserName))
            {
                ModelState.AddModelError(string.Empty, Strings.UserAlreadyExists);
            }
            else
            {
                var validCredentials = _userAccountService.ValidateCredentials(user.UserName, user.Password);

                //Check if the authentication failed
                if (validCredentials == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, Strings.AuthenticationError);

                //Check credentials
                if (validCredentials == true)
                {
                    var newUser = _userAccountService.Create(user);

                    //Log the correct registration
                    _userAccountService.Log(OperationType.Register, newUser.Id, data: "UserName: " + newUser.UserName + ", Addr: " + Request.HostAddress());

                    //Redirect to login
                    return RedirectToAction("Login", new { registered = 1 });
                }

                //If incorrect credentials, add the error
                ModelState.AddModelError(string.Empty, Strings.LoginError);
            }

            return View(user);
        }

        // GET: UserAccount/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int? registered)
        {
            if (Request.IsAuthenticated) return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // POST: UserAccount/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserAccountViewModel user, string returnUrl)
        {
            if (!ModelState.IsValid) return View(user);

            //Check if registered
            if (_userAccountService.IsRegistered(user.UserName))
            {
                var userAccount = _userAccountService.FindByUserName(user.UserName);

                //Check if account is locked
                if (!_userAccountService.CheckLock(userAccount))
                {
                    var validCredentials = _userAccountService.ValidateCredentials(user.UserName, user.Password);

                    //Check if the authentication failed
                    if (validCredentials == null)
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, Strings.AuthenticationError);

                    //Check credentials, if correct, redirect to home page
                    if (validCredentials == true)
                    {
                        //Send the user and request info to the service to store login info
                        var requestInfo = Request.HostAddress() + " - " + Request.UserAgent;
                        userAccount = _userAccountService.LoginSuccessful(userAccount, requestInfo);

                        //Create identity for the logged account
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                            new Claim(ClaimTypes.Name, userAccount.UserName)
                        }, DefaultAuthenticationTypes.ApplicationCookie);

                        //Set role depending on account permissions
                        identity.AddClaim(userAccount.IsAdmin
                            ? new Claim(ClaimTypes.Role, "Admin")
                            : new Claim(ClaimTypes.Role, "User"));

                        //Sign in and log the operation
                        _authenticationManager.SignIn(identity);
                        _userAccountService.Log(OperationType.Login, userAccount.Id, data: "Addr: " + Request.HostAddress());

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, Strings.LoginError);
                }

                var wasLocked = userAccount.IsLocked;

                //Account is locked or incorrect credentials provided,
                //register a failed login attempt
                userAccount = _userAccountService.LoginFailed(userAccount);

                //Log if the account has been locked
                if (!wasLocked && userAccount.IsLocked)
                    _userAccountService.Log(OperationType.AccountLocked, userAccount.Id);

                //If account is not yet locked, exit
                if (!userAccount.IsLocked) return View(user);

                if (userAccount.UnlockDate == null)
                {
                    //Locked indefinitely
                    ModelState.AddModelError(string.Empty, Strings.LoginLockedPerm);
                }
                else
                {
                    //Locked temporarily
                    var lockMessage = Strings.LoginLocked + userAccount.UnlockDate;

                    //After initial lock, alert the user of further failed attempts
                    if (userAccount.FailedLoginAttempts > 3)
                    {
                        lockMessage += Strings.LoginLocked2;
                    }

                    ModelState.AddModelError(string.Empty, lockMessage);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Strings.LoginError);
            }

            return View(user);
        }

        // POST: UserAccount/LogOff
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationManager.SignOut();

            return RedirectToAction("Login");
        }

        // GET: UserAccount/List
        [Authorize(Roles = "Admin")]
        public ActionResult List(string id, string userName, UserAccountType? type, UserAccountStatus? status,
            string registeredFrom, string registeredTo, string sorting, int? page)
        {
            //Sorting
            ViewBag.IdSort = sorting == "id_a" ? "id_d" : "id_a";
            ViewBag.UserSort = sorting == "userName_a" ? "userName_d" : "userName_a";
            ViewBag.DateSort = sorting == "registration_a" ? "registration_d" : "registration_a";
            ViewBag.TypeSort = sorting == "isAdmin_a" ? "isAdmin_d" : "isAdmin_a";
            ViewBag.StatusSort = sorting == "isLocked_a" ? "isLocked_d" : "isLocked_a";

            //Pagination
            var pageNumber = (page ?? 1);

            var users = _userAccountService.Search(id, userName, type, status, registeredFrom, registeredTo, 
                sorting, pageNumber, PageSize);

            return View(users);
        }

        // GET: Account/View/5
        [Authorize(Roles = "Admin")]
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = _userAccountService.FindById((int)id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new UserAccountViewModel(user));
        }

        // POST: Account/Lock/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Lock(int id)
        {
            //Dont allow a user to lock his own user
            if (id == User.Identity.GetUserId<int>())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You cant edit your own account");
            }

            var account = _userAccountService.FindById(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            _userAccountService.ChangeLock(account, true);
            _userAccountService.Log(OperationType.AccountManualLock, User.Identity.GetUserId<int>(), data: "LockedUserID: " + id);

            return RedirectToAction("View", new { id });
        }

        // POST: Account/Unlock/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unlock(int id)
        {
            //Dont allow a user to lock his own user
            if (id == User.Identity.GetUserId<int>())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You cant edit your own account");
            }

            var account = _userAccountService.FindById(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            _userAccountService.ChangeLock(account, false);
            _userAccountService.Log(OperationType.AccountManualUnlock, User.Identity.GetUserId<int>(), data: "UnlockedUserID: " + id);

            return RedirectToAction("View", new { id });
        }

        // POST: Account/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            //Dont allow a user to delete is own user
            if (id == User.Identity.GetUserId<int>())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You can't delete your own account");
            }

            var deleted = _userAccountService.Delete(id);

            //Delete failed because of a wrong id
            if (!deleted) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _userAccountService.Log(OperationType.AccountDelete, User.Identity.GetUserId<int>(), data: "DeletedUserID: " + id);

            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userAccountService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}