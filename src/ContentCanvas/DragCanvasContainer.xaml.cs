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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;

using DiagramDesigner;

namespace ImageStitching
{
    /// <summary>
    /// Interaction logic for DragCanvasContainer.xaml
    /// </summary>
    public partial class DragCanvasContainer : UserControl
    {
        public DragCanvasContainer()
        {
            InitializeComponent(); 
        }

        public DesignerCanvas Canvas
        {
            get
            {
                return this.MyDesignerCanvas;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            /*
            if (this.inkCanvas.Children.Count != 0)
            {
                Rect rect = this.inkCanvas.GetSelectionBounds();

               // this.inkCanvas.Select(null, null);

                // Add the simple selection strokes adorner to the InkCanvas.
                if (adornerLayer == null)
                {
                    // create adorner layer if it doesn't exist yet
                    adornerLayer = AdornerLayer.GetAdornerLayer(this.inkCanvas);
                }
                else
                {
                    // remove previous adorner
                    adornerLayer.Remove(adorner);
                }

                
                adorner = new SimpleSelectionAdorner(this.inkCanvas, rect);
                adornerLayer.Add(adorner);
            }
             * */
        }
    }
}
