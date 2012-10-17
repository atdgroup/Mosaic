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

namespace ExtensionMethods
{
    public static class RectangleExtensions
    {
        public static PointF CentreF(this Rectangle rect)
        {
            return new PointF(rect.Left + rect.Width / 2.0f, rect.Top + rect.Height / 2.0f);
        }

        public static Point Centre(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        public static int Area(this Rectangle rect)
        {
            return (rect.Width * rect.Height);
        }

        public static Rectangle RectanglePositionAroundCentre(this Rectangle rect, Point centre)
        {
            Rectangle rt = rect;
            rt.Location = new Point(centre.X - rect.Width / 2, centre.Y - rect.Height / 2);

            return rt;
        }

        public static Rectangle RectangleChangeWidthAroundCentre(this Rectangle rect, int width)
        {
            Rectangle rt = new Rectangle();
            Point centre = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

            rt.Width = width;
            rt.Height = rect.Height;

            rt.Location = new Point(centre.X - rt.Width / 2, rect.Y);

            return rt;
        }

        public static Rectangle RectangleChangeLargestDimensionToSize(this Rectangle rect, int size)
        {
            Rectangle rt = rect;
            Point centre = rect.Centre();

            if(rect.Width > rect.Height)
            {
                rt.Width = size;
            }
            else if (rect.Height > rect.Width)
            {
                rt.Height = size;
            }
            else
            {
                return rect;
            }

            rt = rt.RectanglePositionAroundCentre(centre);

            return rt;
        }

        public static Rectangle RectangleChangeHeightAroundCentre(this Rectangle rect, int height)
        {
            Rectangle rt = new Rectangle();
            Point centre = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

            rt.Width = rect.Width;
            rt.Height = height;

            rt.Location = new Point(rect.X, (centre.Y - rt.Height / 2));

            return rt;
        }

        public static Rectangle RectangleChangeSizeAroundCentre(this Rectangle rect, int width, int height)
        {
            Rectangle rt = new Rectangle();
            Point centre = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

            rt.Width = width;
            rt.Height = height;

            rt.Location = new Point(centre.X - rt.Width / 2, centre.Y - rt.Height / 2);

            return rt;
        }

        public static Rectangle RectangleIncreaseBySizeAroundCentre(this Rectangle rect, int width, int height)
        {
            return RectangleChangeSizeAroundCentre(rect, rect.Width + width, rect.Height + height);
        }

        public static Rectangle RectangleDecreaseBySizeAroundCentre(this Rectangle rect, int width, int height)
        {
            return RectangleChangeSizeAroundCentre(rect, rect.Width - width, rect.Height - height);
        }

        public static Rectangle RectangleExtractSizeFromCentre(this Rectangle rect, Size size)
        {
            return new Rectangle(rect.Centre().X - size.Width / 2, rect.Centre().Y - size.Height / 2, size.Width, size.Height);
        }  

        public static Rectangle RectangleExtractRandomSize(this Rectangle rect, Size size)
        {
            Random random = new Random();
            int left = random.Next(rect.Left, rect.Right - Math.Min(rect.Width, size.Width));
            int top = random.Next(rect.Top, rect.Bottom - Math.Min(rect.Height, size.Height));

            return new Rectangle(left, top, size.Width, size.Height);
        }

        public static Rectangle PlaceAtRandomVerticalWithBounds(this Rectangle rect, Rectangle bounds)
        {
            Point location = new Point();

            Random random = new Random();

            location.X = rect.Left;
            location.Y = random.Next(bounds.Top, bounds.Bottom - bounds.Height);

            return new Rectangle(location, rect.Size);
        }

        public static Rectangle PlaceAtRandomHorizontalWithBounds(this Rectangle rect, Rectangle bounds)
        {
            Point location = new Point();

            Random random = new Random();

            location.Y = rect.Top;
            location.X = random.Next(bounds.Left, bounds.Right - bounds.Width);

            return new Rectangle(location, rect.Size);
        }
    }
}