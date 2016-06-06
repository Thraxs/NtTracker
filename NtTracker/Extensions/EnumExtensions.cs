using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NtTracker.Resources.Shared;

namespace NtTracker.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the localized display name of an enum value.
        /// </summary>
        /// <param name="value">Enum value.</param>
        /// <returns>Localized display name property.</returns>
        public static string ToLocalizedString(this Enum value)
        {
            if (value == null) return SharedStrings.NA;

            var res = value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>().GetName();
            return res;
        }

        /// <summary>
        /// Returns the localized display description of an enum value.
        /// </summary>
        /// <param name="value">Enum value.</param>
        /// <returns>Localized display description property.</returns>
        public static string ToLocalizedDescription(this Enum value)
        {
            var res = value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>().GetDescription();
            return res;
        }

        /// <summary>
        /// Custom implementation of GetSelectList to include the enum int value in the display attribute.
        /// Gets a list of <see cref="SelectListItem"/> objects corresponding to enum constants defined in the given.
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to evaluate.</param>
        /// <returns> An <see cref="IList{T}"/> for the given <paramref name="type"/>.</returns>
        /// <remarks>
        /// Implementation based on the source code of System.Web.Mvc.Html.
        /// </remarks>
        public static IList<SelectListItem> GetSelectListWithValue(Type type)
        {
            IList<SelectListItem> selectList = new List<SelectListItem>();

            Type checkedType = Nullable.GetUnderlyingType(type) ?? type;
            if (checkedType != type)
            {
                // Underlying type was non-null so handle Nullable<T>; ensure returned list has a spot for null
                selectList.Add(new SelectListItem { Text = String.Empty, Value = String.Empty, });
            }

            // Populate the list
            const BindingFlags BindingFlags =
                BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static;
            foreach (FieldInfo field in checkedType.GetFields(BindingFlags))
            {
                // fieldValue will be an numeric type (byte, ...)
                object fieldValue = field.GetRawConstantValue();

                var displayText = "[" + fieldValue + "] " + field.GetCustomAttribute<DisplayAttribute>(false).GetName();
                selectList.Add(new SelectListItem { Text = displayText, Value = fieldValue.ToString(), });
            }

            return selectList;
        }
    }
}