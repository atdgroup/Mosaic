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
using System.Collections.Generic;

using FreeImageAPI;
using ExtensionMethods;

namespace ImageStitching
{
    public class CorrelationTile
    {
        private Tile tile;
        private static Point origin;  
        private static int maxNumberOfCorrelationAttempts = 5;
        private Point startPosition;
        private bool correlationFailed;
        private bool dontCorrelate = false;
        private bool performedCorrelation = false;  // flags any attempt at correlation, a successful correlation is flagged in "Tile"
        private int numberOfAtemptedCorrelations = 0;

        public CorrelationTile(Tile tile)
        {
            this.tile = tile;
            this.CorrelationPosition = tile.OriginalPosition;
        }

        public Tile Tile
        {
            get
            {
                return this.tile;
            }
        }

        public Point CorrelationPosition
        {
            get
            {
                return this.startPosition;
            }
            set
            {
                this.startPosition = value;
            }
        }

        public static int MaxNumberOfCorrelationAttempts
        {
            set
            {
                CorrelationTile.maxNumberOfCorrelationAttempts = value;
            }
            get
            {
                return CorrelationTile.maxNumberOfCorrelationAttempts;
            }
        }

        public int NumberOfAtemptedCorrelations
        {
            get
            {
                return this.numberOfAtemptedCorrelations;
            }
            set
            {
                this.numberOfAtemptedCorrelations = value;
            }
        }

        public bool CorrelationFailed
        {
            get
            {
                return this.correlationFailed;
            }
            set
            {
                this.correlationFailed = value;
            }
        } 

        public bool DontCorrelate
        {
            get
            {
                return this.dontCorrelate;
            }
            set
            {
                this.dontCorrelate = value;
            }
        }

        // Sepcifies whther a correlation has been attempted
        // regardless of success or not.
        public bool PerformedCorrelation
        {
            set
            {
                this.performedCorrelation = value;
            }
            get
            {
                return this.performedCorrelation;
            }
        }

        public static Point Origin
        {
            set
            {
                CorrelationTile.origin = value;
            }
            get
            {
                return CorrelationTile.origin;
            }
        }

        public Point OriginalPositionRelativeToOrigin
        {
            get
            {
                return new Point(this.Tile.OriginalPosition.X - CorrelationTile.origin.X, this.Tile.OriginalPosition.Y - CorrelationTile.origin.Y);
            }
        }

        public Point CorrelatedPositionRelativeToOrigin
        {
            get
            {
                return new Point(this.CorrelationPosition.X - CorrelationTile.origin.X, this.CorrelationPosition.Y - CorrelationTile.origin.Y);
            }
        }

        public Rectangle CorrelatedBoundsRelativeToOrigin
        {
            get
            {
                return new Rectangle(this.CorrelatedPositionRelativeToOrigin, this.Tile.Size);
            }
        }

        public Rectangle CorrelatedBoundsRelativeToMosaic
        {
            get
            {
                return new Rectangle(this.CorrelationPosition, this.Tile.Size);
            }
        }

        public Rectangle OriginalBoundsRelativeToOrigin
        {
            get
            {
                return new Rectangle(this.OriginalPositionRelativeToOrigin, this.Tile.Size);
            }
        }

        public Rectangle BoundsRelativeToMosaic
        {
            get
            {
                Rectangle rect = new Rectangle(this.Tile.Position, this.Tile.Size);

                return rect;
            }
        }

        public Rectangle BackgroundBoundsWithinMosaic
        {   // Assumes a 3x3 section of mosaic
            get
            {
                Point topLeft = new Point(this.Tile.OriginalPosition.X - this.Tile.Width, this.Tile.OriginalPosition.Y - this.Tile.Height);
                Size size = new Size(this.Tile.Size.Width * 3, this.Tile.Size.Height * 3);
                return new Rectangle(topLeft, size);
            }
        }

        public static Point TranslateBackgroundPointToMosaicPoint(Point pt)
        {
            return new Point(pt.X + CorrelationTile.Origin.X, pt.Y + CorrelationTile.Origin.Y);
        }

        public static Rectangle TranslateBackgroundRectToMosaicRectangle(Rectangle rect)
        {
            return new Rectangle(TranslateBackgroundPointToMosaicPoint(rect.Location), rect.Size);
        }

        public static CorrelationTile GetMostLeftTile(List<CorrelationTile> tiles)
        {
            CorrelationTile leftMostTile = tiles[0];

            foreach (CorrelationTile tile in tiles)
            {
                if (tile.Tile.Position.X < leftMostTile.Tile.Position.X)
                    leftMostTile = tile;
            }

            return leftMostTile;
        }

        public static CorrelationTile GetMostRightTile(List<CorrelationTile> tiles)
        {
            CorrelationTile rightMostTile = tiles[0];

            foreach (CorrelationTile tile in tiles)
            {
                if (tile.Tile.Position.X > rightMostTile.Tile.Position.X)
                    rightMostTile = tile;
            }

            return rightMostTile;
        }

        public static CorrelationTile GetMostTopTile(List<CorrelationTile> tiles)
        {
            CorrelationTile topMostTile = tiles[0];

            foreach (CorrelationTile tile in tiles)
            {
                if (tile.Tile.Position.Y < topMostTile.Tile.Position.Y)
                    topMostTile = tile;
            }

            return topMostTile;
        }

        public static CorrelationTile GetMostBottomTile(List<CorrelationTile> tiles)
        {
            CorrelationTile bottomMostTile = tiles[0];

            foreach (CorrelationTile tile in tiles)
            {
                if (tile.Tile.Position.Y > bottomMostTile.Tile.Position.Y)
                    bottomMostTile = tile;
            }

            return bottomMostTile;
        }

        public static int GetMostLeftPosition(List<CorrelationTile> tiles)
        {
            return CorrelationTile.GetMostLeftTile(tiles).Tile.Position.X;
        }

        public static int GetMostRightPosition(List<CorrelationTile> tiles)
        {
            return CorrelationTile.GetMostRightTile(tiles).Tile.Bounds.Right;
        }

        public static int GetMostTopPosition(List<CorrelationTile> tiles)
        {
            return CorrelationTile.GetMostTopTile(tiles).Tile.Position.Y;
        }

        public static int GetMostBottomPosition(List<CorrelationTile> tiles)
        {
            return CorrelationTile.GetMostBottomTile(tiles).Tile.Bounds.Bottom;
        }

        public static int GetHorizontalRangeOfTiles(List<CorrelationTile> tiles)
        {
            return (CorrelationTile.GetMostRightPosition(tiles) - CorrelationTile.GetMostLeftPosition(tiles));
        }

        public static int GetVerticalRangeOfTiles(List<CorrelationTile> tiles)
        {
            return (CorrelationTile.GetMostBottomPosition(tiles) - CorrelationTile.GetMostTopPosition(tiles));
        }

        public static PointF GetCentreOfTiles(List<CorrelationTile> tiles)
        {
            int left = CorrelationTile.GetMostLeftPosition(tiles);
            int top = CorrelationTile.GetMostTopPosition(tiles);
            int width = CorrelationTile.GetMostRightPosition(tiles) - left;
            int height = CorrelationTile.GetMostBottomPosition(tiles) - top;

            return new PointF(left + width / 2.0f, top + height / 2.0f);
        }
    }

    /// <summary>
    /// CircularComparer orders tile according to which ones are closest to
    /// some centraol value.
    /// </summary>
    internal class CorrelationTileCircularComparer : IComparer<CorrelationTile>
    {
        private PointF centre;

        public CorrelationTileCircularComparer(PointF centre)
        {
            this.centre = centre;
        }

        public CorrelationTileCircularComparer(CorrelationTile tile)
        {
            this.centre = tile.Tile.Centre;
        }

        private float DistanceBetweenCentresMeasure(PointF centre1, PointF centre2)
        {
            return (((centre1.X - centre2.X) * (centre1.X - centre2.X)) + ((centre1.Y - centre2.Y) * (centre1.Y - centre2.Y)));
        }

        public int Compare(CorrelationTile lhs, CorrelationTile rhs)
        {
            float lhsSpacing = DistanceBetweenCentresMeasure(lhs.Tile.Centre, this.centre);
            float rhsSpacing = DistanceBetweenCentresMeasure(rhs.Tile.Centre, this.centre);

            return lhsSpacing.CompareTo(rhsSpacing);
        }
    }

    /// <summary>
    /// TileIntersectionComparer orders tile according to which ones have the largest area
    /// </summary>
    internal class TileIntersectionComparer : IComparer<CorrelationTile>
    {
        private CorrelationTile tile;

        public TileIntersectionComparer(CorrelationTile tile)
        {
            this.tile = tile;
        }

        public int Compare(CorrelationTile lhs, CorrelationTile rhs)
        {
            Rectangle rect1 = lhs.Tile.Bounds;
            Rectangle rect2 = rhs.Tile.Bounds;

            rect1.Intersect(tile.Tile.Bounds);
            rect2.Intersect(tile.Tile.Bounds);

            int area1 = rect1.Area();
            int area2 = rect2.Area();

            if(area1 == area2)
                return -(lhs.Tile.IsAdjusted.CompareTo(rhs.Tile.IsAdjusted));

            // - means we want the items sorted by descending value
            return -(area1.CompareTo(area2));
        }
    }
}
