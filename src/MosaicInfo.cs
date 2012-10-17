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
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    public class MosaicInfo
    {
        private TileReader reader;

        private bool isCorrelated = false;
        private bool isCompositeRGB = false;
        private double currentZoom = 1.0;

        public MosaicInfo(TileReader reader)
        {
            this.reader = reader;

            this.ScaleMinIntensity = reader.LoadInfo.TotalMinIntensity;
            this.ScaleMaxIntensity = reader.LoadInfo.TotalMaxIntensity;
            this.IsCorrelated = reader.LoadInfo.IsCorrelated;
            this.IsCompositeRGB = reader.LoadInfo.IsCompositeRGB;
        }

        public TileReader TileReader
        {
            get
            {
                return reader;
            }
        }

        public bool IsCorrelated
        {
            get
            {
                return this.isCorrelated;
            }
            set
            {
                this.isCorrelated = value;
            }
        }

        public bool IsCompositeRGB
        {
            get
            {
                return this.isCompositeRGB;
            }
            set
            {
                this.isCompositeRGB = value;
            }
        }

        internal string Prefix
        {
            get
            {
                return this.reader.LoadInfo.Prefix;
            }
        }

        internal string DirectoryPath
        {
            get
            {
                return this.reader.DirectoryPath;
            }
        }

        public Dictionary<string, string> MetaData
        {
            get
            {
                return this.reader.LoadInfo.MetaData;
            }
        }

        internal List<Tile> Items
        {
            get
            {
                return this.reader.LoadInfo.Items;
            }
        }

        public FREE_IMAGE_TYPE FreeImageType
        {
            get
            {
                return this.reader.LoadInfo.FreeImageType;
            }
        }

        public int WidthInTiles
        {
            get
            {
                return this.reader.LoadInfo.WidthInTiles;
            }
        }

        public int HeightInTiles
        {
            get
            {
                return this.reader.LoadInfo.HeightInTiles;
            }
        }

        public int NumberOfTiles
        {
            get
            {
                return this.WidthInTiles * this.HeightInTiles;
            }
        }

        public double ThumbnailScaleMinIntensity
        {
            get
            {
                return this.reader.LoadInfo.ThumbnailScaleMinIntensity;
            }
        }

        public double ThumbnailScaleMaxIntensity
        {
            get
            {
                return this.reader.LoadInfo.ThumbnailScaleMaxIntensity;
            }
        }

        public double TotalMinIntensity
        {
            get
            {
                return this.reader.LoadInfo.TotalMinIntensity;
            }
        }

        public double TotalMaxIntensity
        {
            get
            {
                return this.reader.LoadInfo.TotalMaxIntensity;
            }
        }

        public double ScaleMinIntensity
        {
            get
            {
                return this.reader.LoadInfo.ScaleMinIntensity;
            }
            set
            {
                this.reader.LoadInfo.ScaleMinIntensity = value;
            }
        }

        public double ScaleMaxIntensity
        {
            get
            {
                return this.reader.LoadInfo.ScaleMaxIntensity;
            }
            set
            {
                this.reader.LoadInfo.ScaleMaxIntensity = value;
            }
        }

        public int ColorDepth
        {
            get
            {
                return this.reader.LoadInfo.ColorDepth;
            }
        }

        public double OverLapPercentageX
        {
            get
            {
                return this.reader.LoadInfo.OverLapPercentageX;
            }
        }

        public double OverLapPercentageY
        {
            get
            {
                return this.reader.LoadInfo.OverLapPercentageY;
            }
        }

        public bool HasConstantOverlap
        {
            get
            {
                return  (MosaicWindow.MosaicInfo.OverLapPercentageX != 0.0 && MosaicWindow.MosaicInfo.OverLapPercentageY != 0.0);
            }
        }

        public double Zoom
        {
            get
            {
                return this.currentZoom;
            }
            set
            {
                this.currentZoom = value;
            }
        }

        public double OriginalPixelsPerMicron
        {
            get
            {
                return this.reader.LoadInfo.OriginalPixelsPerMicron;
            }
        }

        public double OriginalMicronsPerPixel
        {
            get
            {
                return this.reader.LoadInfo.OriginalMicronsPerPixel;
            }
        }

        public double MicronsPerPixel
        {
            get
            {
                return 1 / (this.OriginalPixelsPerMicron * this.Zoom);
            }
        }
 
        public bool IsColour
        {
            get
            {
                return this.reader.LoadInfo.IsColour;
            }
        }

        public int TotalWidth
        {
            get
            {
                return this.reader.LoadInfo.TotalWidth;
            }
        }

        public int TotalHeight
        {
            get
            {
                return this.reader.LoadInfo.TotalHeight;
            }
        }

        public bool IsGreyScale
        {
            get
            {
                return this.reader.LoadInfo.IsGreyScale;
            }
        } 
    }
}
