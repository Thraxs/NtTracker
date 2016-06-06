using NtTracker.Resources.Shared;

namespace NtTracker.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the localized display name of a bool.
        /// </summary>
        /// <param name="value">bool value.</param>
        /// <returns>Localized bool value.</returns>
        public static string ToLocalizedString(this bool value)
        {
            return value ? SharedStrings.Yes : SharedStrings.No;
        }
    }
}