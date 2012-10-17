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
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;

namespace GrayLabs
{
    namespace Windows
    {
        namespace Forms
        {
            partial class AboutBox : Form
            {
                public AboutBox()
                {
                    InitializeComponent();

                    //  Initialize the AboutBox to display the product information from the assembly information.
                    //  Change assembly information settings for your application through either:
                    //  - Project->Properties->Application->Assembly Information
                    //  - AssemblyInfo.cs
                    this.Text = String.Format(CultureInfo.CurrentCulture, "About {0}", AssemblyTitle);
                    this.labelProductName.Text = AssemblyProduct;
                    this.labelVersion.Text = String.Format(CultureInfo.CurrentCulture, "Version {0}", AssemblyVersion);
                    this.labelCopyright.Text = AssemblyCopyright;
                    this.labelCompanyName.Text = AssemblyCompany;
                    this.labelSvnVersion.Text = String.Format(CultureInfo.CurrentCulture, "Svn Respository Version: {0}", SvnVersion);
                }

                #region Assembly Attribute Accessors

                public static  string AssemblyTitle
                {
                    get
                    {
                        // Get all Title attributes on this assembly
                        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                        // If there is at least one Title attribute
                        if (attributes.Length > 0)
                        {
                            // Select the first one
                            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                            // If it is not an empty string, return it
                            if (!String.IsNullOrEmpty(titleAttribute.Title))
                                return titleAttribute.Title;
                        }
                        // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                    }
                }

                public static string AssemblyVersion
                {
                    get
                    {
                        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    }
                }

                public static string SvnVersion
                {
                    get
                    {
                        object obj = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(SubVersionAttribute), false)[0];

                        SubVersionAttribute attribute = (SubVersionAttribute)obj;

                        return attribute.Version;           
                    }
                }

                public static string AssemblyDescription
                {
                    get
                    {
                        // Get all Description attributes on this assembly
                        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                        // If there aren't any Description attributes, return an empty string
                        if (attributes.Length == 0)
                            return "";
                        // If there is a Description attribute, return its value
                        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                    }
                }

                public static string AssemblyProduct
                {
                    get
                    {
                        // Get all Product attributes on this assembly
                        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                        // If there aren't any Product attributes, return an empty string
                        if (attributes.Length == 0)
                            return "";
                        // If there is a Product attribute, return its value
                        return ((AssemblyProductAttribute)attributes[0]).Product;
                    }
                }

                public static string AssemblyCopyright
                {
                    get
                    {
                        // Get all Copyright attributes on this assembly
                        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                        // If there aren't any Copyright attributes, return an empty string
                        if (attributes.Length == 0)
                            return "";
                        // If there is a Copyright attribute, return its value
                        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                    }
                }

                public static string AssemblyCompany
                {
                    get
                    {
                        // Get all Company attributes on this assembly
                        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                        // If there aren't any Company attributes, return an empty string
                        if (attributes.Length == 0)
                            return "";
                        // If there is a Company attribute, return its value
                        return ((AssemblyCompanyAttribute)attributes[0]).Company;
                    }
                }
                #endregion
            }
        }
    }
}
