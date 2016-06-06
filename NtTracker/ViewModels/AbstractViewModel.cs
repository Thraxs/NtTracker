using System.Web.Mvc;

namespace NtTracker.ViewModels
{
    public abstract class AbstractViewModel
    {
        /// <summary>
        /// Property to store the model state when doing service layer validation.
        /// </summary>
        public ModelStateDictionary ModelState { get; set; }
    }
}