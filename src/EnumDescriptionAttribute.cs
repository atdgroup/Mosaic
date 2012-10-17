/*
*   Copyright 2007-2010 Glenn Pierce, Paul Barber,
*   Oxford University (Gray Institute for Radiation Oncology and Biology) 
*
*   This file is part of MosaicStitcher.
*
*   MosaicStitcher is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*   
*   MosaicStitcher is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with MosaicStitcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ImageStitching
{
    /// <summary>
    /// Provides a description for an enumerated type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        private string description;

        /// <summary>
        /// Gets the description stored in this attribute.
        /// </summary>
        /// <value>The description stored in the attribute.</value>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EnumDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description to store in this attribute.
        /// </param>
        public EnumDescriptionAttribute(string description)
            : base()
        {
            this.description = description;
        }
    }

    /// <summary>
    /// Provides a static utility object of methods and properties to interact
    /// with enumerated types.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the <see cref="DescriptionAttribute" /> of an <see cref="Enum" /> 
        /// type value.
        /// </summary>
        /// <param name="value">The <see cref="Enum" /> type value.</param>
        /// <returns>A string containing the text of the
        /// <see cref="DescriptionAttribute"/>.</returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            EnumDescriptionAttribute[] attributes =
               (EnumDescriptionAttribute[])
             fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }

        /// <summary>
        /// Converts the <see cref="Enum" /> type to an <see cref="IList" /> 
        /// compatible object.
        /// </summary>
        /// <param name="type">The <see cref="Enum"/> type.</param>
        /// <returns>An <see cref="IList"/> containing the enumerated
        /// type value and description.</returns>
        public static IList ToList(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            ArrayList list = new ArrayList();
            Array enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                list.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
            }

            return list;
        }
    }
}
