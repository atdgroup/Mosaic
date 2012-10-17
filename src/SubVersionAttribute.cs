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
using System.Runtime.InteropServices;

namespace System.Reflection
{
    // Summary:
    //     Specifies the version of the assembly being attributed.
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class SubVersionAttribute : Attribute
    {
        private string version;

        // Summary:
        //     Initializes a new instance of the AssemblyVersionAttribute class with the
        //     version number of the assembly being attributed.
        //
        // Parameters:
        //   version:
        //     The version number of the attributed assembly.
        public SubVersionAttribute(string version)
        {
            this.version = version;
        }

        // Summary:
        //     Gets the version number of the attributed assembly.
        //
        // Returns:
        //     A string containing the assembly version number.
        public string Version
        {
            get
            {
                return this.version;
            }
        }
    }
}
