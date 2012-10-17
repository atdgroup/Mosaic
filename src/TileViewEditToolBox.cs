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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageStitching
{
    public delegate void TileViewEditToolBoxHandler<S, A>(S sender, A args);

    public partial class TileViewEditToolBox : UserControl
    {
        public event TileViewEditToolBoxHandler<TileViewEditToolBox, EventArgs> TickButtonPressed;
        public event TileViewEditToolBoxHandler<TileViewEditToolBox, EventArgs> CrossButtonPressed;

        public TileViewEditToolBox()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BringToFront();
        }

        /*
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        public void InvalidateEx()
        {
            if (Parent == null)
                return;

            Rectangle rc = new Rectangle(this.Location, this.Size);
            Parent.Invalidate(rc, true);
        }
        */

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
            //do not allow the background to be painted 
        //}

        protected void OnTickButtonPressed(EventArgs args)
        {
            if (TickButtonPressed != null)
                TickButtonPressed(this, args);
        }

        protected void OnCrossButtonPressed(EventArgs args)
        {
            if (CrossButtonPressed != null)
                CrossButtonPressed(this, args);
        }

        private void OnTickPressed(object sender, EventArgs e)
        {
            OnTickButtonPressed(e);
        }

        private void OnCrossClicked(object sender, EventArgs e)
        {
            OnCrossButtonPressed(e);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
        //    this.InvalidateEx();
        }

        public void StartInvalidateTimer()
        {
            this.timer.Enabled = true;
        }

        public void StopInvalidateTimer()
        {
            this.timer.Enabled = false;
        }
    }
}
