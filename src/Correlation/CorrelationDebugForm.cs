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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageStitching
{
    public partial class CorrelationDebugForm : Form
    {
        // Temp variable to store a previously focused control
        private Control focused = null;

        public CorrelationDebugForm()
        {
            InitializeComponent();
        }

        public ImageView BackgroundImageView
        {
            set
            {
                this.imageView = value;
            }
            get
            {
                return this.imageView;
            }
        }

        public ImageView TileImageView
        {
            set
            {
                this.imageView2 = value;
            }
            get
            {
                return this.imageView2;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // Get the focused control before the splitter is focused
            focused = getFocused(this.Controls);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            // If a previous control had focus
            if (focused != null)
            {
                // Return focus and clear the temp variable for
                // garbage collection
                focused.Focus();
                focused = null;
            }
        }

        private Control getFocused(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.Focused)
                {
                    // Return the focused control
                    return c;
                }
                else if (c.ContainsFocus)
                {
                    // If the focus is contained inside a control's children
                    // return the child
                    return getFocused(c.Controls);
                }
            }
            // No control on the form has focus
            return null;
        }
    }
}
