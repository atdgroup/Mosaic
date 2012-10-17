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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ImageStitching
{
    public delegate void MosaicPluginEventHandler<S, A>(S sender, A args);

    abstract public class MosaicPlugin
    {
        private static Dictionary<string, MosaicPlugin> plugins = new Dictionary<string, MosaicPlugin>(10);
        private MosaicWindow window;
        private string name;
        private bool active;
//        private ToolStripMenuItem[] menuItems = null;

        public event MosaicPluginEventHandler<MosaicPlugin, EventArgs> PluginActiveStatusChanged;

        public MosaicPlugin(string name, MosaicWindow window)
        {
            MosaicPlugin.plugins[name] = this;
            this.window = window;
            this.name = name;

            window.MosaicLoaded += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnMosaicLoaded);

            window.ZoomChanged += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnZoomChanged);

            window.SavingScreenShot += new MosaicWindowGraphicsHandler<MosaicWindow, Graphics, MosaicWindowEventArgs>(OnSavingScreenShot);

            window.TileView.MouseDown +=
                new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);

            window.TileView.MouseUp +=
                new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);

            window.TileView.MouseMove +=
                new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);

            window.TileView.Paint += new PaintEventHandler(this.OnTileViewPainted);
        }
      
        public static Dictionary<string, MosaicPlugin> AllPlugins
        {
            get
            {
                return MosaicPlugin.plugins;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public MosaicWindow MosaicWindow
        {
            get
            {
                return this.window;
            }
        }

        public virtual bool Active
        {
            set
            {
                this.active = value;
            }
            get
            {
                return this.active;
            }
        }

        public abstract bool Enabled
        {
            get;
            set;
        }

        protected ToolStripMenuItem AddMenuItem(string menuText, string text, string icon, EventHandler handler)
        {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem menu = menuStrip.Items.Add(menuText) as ToolStripMenuItem;

            ToolStripItem item = menu.DropDownItems.Add(text);

            ToolStripMenuItem menuItem = item as ToolStripMenuItem;
            menuItem.CheckOnClick = true;
            menuItem.CheckedChanged += new EventHandler(handler);

            menu.MergeAction = MergeAction.MatchOnly;

            ToolStripManager.Merge(menuStrip, this.MosaicWindow.MenuStrip);

            return menuItem;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if(this.Active)
                this.OnViewMouseDown(e);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (this.Active)
                this.OnViewMouseUp(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.Active)
                this.OnViewMouseMove(e);
        }

        private void OnMosaicLoaded(MosaicWindow sender, MosaicWindowEventArgs e)
        {
            this.OnMosaicLoaded(e);
        }

        private void OnZoomChanged(MosaicWindow sender, MosaicWindowEventArgs e)
        {
            this.OnZoomChanged(e);
        }

        private void OnSavingScreenShot(MosaicWindow sender, Graphics g, MosaicWindowEventArgs e)
        {
            this.OnSavingScreenShot(g, e);
        }

        protected virtual void OnMosaicLoaded(MosaicWindowEventArgs args)
        {
        }

        protected virtual void OnZoomChanged(MosaicWindowEventArgs args)
        {
        }

        protected virtual void OnSavingScreenShot(Graphics g, MosaicWindowEventArgs args)
        {
        }
        
        protected virtual void OnTileViewPainted(object sender, PaintEventArgs e)
        {
            if (this.Active)
                this.OnViewPainted(e);
        }

        protected virtual void OnViewPainted(PaintEventArgs e)
        {
        }

        protected virtual void OnViewMouseDown(MouseEventArgs e)
        {
        }

        protected virtual void OnViewMouseUp(MouseEventArgs e)
        {
        }

        protected virtual void OnViewMouseMove(MouseEventArgs e)
        {
        }

        protected void OnPluginActiveStatusChanged(EventArgs args)
        {
            if (PluginActiveStatusChanged != null)
                PluginActiveStatusChanged(this, args);
        }
    }

    abstract public class MosaicToggleButtonTool : MosaicPlugin
    {
        private ToolStripButton button;

        public MosaicToggleButtonTool(string name, MosaicWindow window, string icon) : base(name, window)
        { 
            Image image = (Image)new Bitmap(GetType(), icon);
            this.button = new ToolStripButton(image);
            this.button.Text = this.Name;
            this.button.CheckOnClick = true;
            this.button.CheckedChanged += new EventHandler(OnToggleButtonToolSelected);
            this.MosaicWindow.ToolStrip.Items.Add(this.button);

            this.Active = false;
        }

        protected virtual void OnToggleButtonToolSelected(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            if (item.Checked)
            {
                this.Active = true;
            }
            else
            {
                this.Active = false;
            }

            OnPluginActiveStatusChanged(new EventArgs());
        }

        protected ToolStripButton Button
        {
            get
            {
                return this.button;
            }
        }

        public override bool Enabled
        {
            get
            {
                return this.button.Enabled;
            }
            set
            {
                this.button.Enabled = value;
            }
        }

        public override bool Active
        {
            set
            {
                base.Active = value;
                this.button.Checked = value;
            }
            get
            {
                return base.Active;
            }
        }
    }
}
