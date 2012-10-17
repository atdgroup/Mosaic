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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Ink;
using System.Windows.Controls.Primitives;

using DiagramDesigner;

using FreeImageAPI;
using FreeImageIcs;

namespace ImageStitching
{
    public partial class ImageCollectionArrangerForm : Form
    {
        private Bitmap[] images;
        private int largestImageWidth;
        private int largestImageHeight;
        private int virtualWidth;
        private int virtualHeight;
        private DragCanvasContainer canvasContainer;
        private float startx = 0.0f;
        private float starty = 0.0f;

        public ImageCollectionArrangerForm()
        {
            InitializeComponent();

            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = System.Windows.Forms.DockStyle.Fill;
            host.BackColor = System.Drawing.Color.Blue;

            // Create the WPF UserControl.
            this.canvasContainer = new DragCanvasContainer();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = this.canvasContainer;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);
        }

        public void Initialise(Bitmap[] images)
        {
            this.images = images;

            foreach (Bitmap bm in images)
            {
                if (bm.Width > largestImageWidth)
                    largestImageWidth = bm.Width;

                if (bm.Height > largestImageHeight)
                    largestImageHeight = bm.Height;
            }

            // Lets assume the worst case that all the images are arranged in
            // one row or coloumn.
            virtualWidth = largestImageWidth * images.Length;
            virtualHeight = largestImageHeight * images.Length;
        }

        private DesignerCanvas Canvas
        {
            get
            {
                return this.canvasContainer.MyDesignerCanvas;
            }
        }

        public void AddThumbail(string filePath)
        {
            DesignerItem newItem = new DesignerItem();
            FreeImageAlgorithmsWPFImage uiElement = new FreeImageAlgorithmsWPFImage(filePath);
            uiElement.Stretch = Stretch.Fill;
            newItem.Selected += new System.Windows.RoutedEventHandler(OnDesignerItemSelected); 

            uiElement.IsHitTestVisible = false;
            newItem.Content = uiElement;

            newItem.Width = uiElement.ThumbnailFib.Width;
            newItem.Height = uiElement.ThumbnailFib.Height;

            DesignerCanvas.SetLeft(newItem, startx);
            DesignerCanvas.SetTop(newItem, starty);

            startx += 10.0f;
            starty += 10.0f;

            this.Canvas.Children.Add(newItem);

            this.Canvas.DeselectAll();
            newItem.IsSelected = true;
        }

        void OnDesignerItemSelected(object sender, System.Windows.RoutedEventArgs e)
        {
            DesignerItem item = sender as DesignerItem;

            FreeImageAlgorithmsWPFImage imObject = item.Content as FreeImageAlgorithmsWPFImage;

            if (imObject != null)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(imObject.FilePath);
                this.Text = "Manual Import " + fileName;
                item.BringToFront();
            }
        }

        public List<DesignerItem> GetItemPositionsSortedFromMostTopLeft()
        {
            List<DesignerItem> items = new List<DesignerItem>();

            foreach (DesignerItem item in this.Canvas.Children)
            {
                items.Add(item);
            }

            items.Sort(new TopLeftClosestToPointComparer(new PointF(0.0f, 0.0f)));

            return items;
        }
    }

    public class FreeImageAlgorithmsWPFImage : System.Windows.Controls.Image
    {
        public FreeImageAlgorithmsBitmap Fib = null;
        public FreeImageAlgorithmsBitmap ThumbnailFib = null;
        public string FilePath;

        public FreeImageAlgorithmsWPFImage(string filepath) : base()
        {    
            this.FilePath = filepath;

            try
            {
                this.Fib = Tile.LoadFreeImageBitmapFromFile(filepath);
            }
            catch (Exception e)
            {
                throw;
            }

            ThumbnailFib = new FreeImageAlgorithmsBitmap(this.Fib);
            Tile.ResizeToThumbnail(ThumbnailFib);
            
            if (ThumbnailFib.IsGreyScale)
                ThumbnailFib.LinearScaleToStandardType(0, 0);
            else
                ThumbnailFib.ConvertToStandardType(true);

            Bitmap bitmap = ThumbnailFib.ToBitmap();

            System.Windows.Int32Rect rect = new System.Windows.Int32Rect();

            rect.X = 0;
            rect.Y = 0;
            rect.Width = bitmap.Width;
            rect.Height = bitmap.Height;

            this.Source = ConvertGdiToWPF(bitmap, rect);
        }

        private BitmapSource ConvertGdiToWPF(System.Drawing.Bitmap bm, System.Windows.Int32Rect rect)
        {
            BitmapSource bms = null;

            if (bm != null)
            {
                IntPtr h_bm = bm.GetHbitmap();
                bms = Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, rect, BitmapSizeOptions.FromEmptyOptions());
            }

            return bms;
        }
    }

    /// <summary>
    /// CircularComparer orders tile according to which ones are closest to
    /// some centraol value.
    /// </summary>
    internal class TopLeftClosestToPointComparer : IComparer<DesignerItem>
    {
        private PointF point;

        public TopLeftClosestToPointComparer(PointF point)
        {
            this.point = point;
        }

        private float DistanceBetweenPointsMeasure(PointF pt1, PointF pt2)
        {
            return (((pt1.X - pt2.X) * (pt1.X - pt2.X)) + ((pt1.Y - pt2.Y) * (pt1.Y - pt2.Y)));
        }

        private PointF Location(DesignerItem element)
        {
            float x = (float) DesignerCanvas.GetLeft(element);
            float y = (float) DesignerCanvas.GetTop(element);

            return new PointF(x, y);
        }

        public int Compare(DesignerItem lhs, DesignerItem rhs)
        {
            float lhsSpacing = DistanceBetweenPointsMeasure(Location(lhs), this.point);
            float rhsSpacing = DistanceBetweenPointsMeasure(Location(rhs), this.point);

            return lhsSpacing.CompareTo(rhsSpacing);
        }
    }
}
