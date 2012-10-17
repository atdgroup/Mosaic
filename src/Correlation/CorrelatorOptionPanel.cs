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
using System.Drawing;
using System.Windows.Forms;

namespace ImageStitching
{
    public partial class CorrelatorOptionPanel : UserControl
    {
        private MosaicInfo info;

        public CorrelatorOptionPanel()
        {
            InitializeComponent();
        }

        public virtual void Reset(MosaicInfo info) { }

        public MosaicInfo MosaicInfo
        {
            get
            {
                return this.info;
            }
            set
            {
                this.info = value;
            }
        }
    }
}
