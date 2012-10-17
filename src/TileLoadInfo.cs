using System;
using System.Drawing;
using System.Collections.Generic;

using FreeImageAPI;

namespace ImageStitching
{
    public abstract partial class TileReader : IDisposable
    {
        #region TileLoadInfo Inner class
        public class TileLoadInfo
        {
            private string directoryPath;
            private string prefix;
            private FREE_IMAGE_TYPE type;
            private double originalPixelsPerMicron;
            private int colorDepth;
            private int totalWidth;
            private int totalHeight;
            private double scaleMinIntensity;
            private double scaleMaxIntensity;
            private double totalMinIntensity;
            private double totalMaxIntensity;
            private int widthInTiles;
            private int heightInTiles;
            private bool isCorrelated = false;
            private bool isCompositeRGB = false;
            private double overLapPercentageX;
            private double overLapPercentageY;
            private double thumbnailScaleMinIntensity;
            private double thumbnailScaleMaxIntensity;
            private List<Tile> tileList;
            private Dictionary<string, string> metaData = new Dictionary<string, string>(50);

            public TileLoadInfo()
            {
                this.tileList = new List<Tile>();
                this.OriginalPixelsPerMicron = 1.0;
                this.overLapPercentageX = 0.0;
                this.overLapPercentageY = 0.0;
                this.ScaleMinIntensity = 0.0;
                this.ScaleMaxIntensity = 0.0;
            }

            public TileLoadInfo(int totalWidth, int totalHeight, int colorDepth,
                FREE_IMAGE_TYPE type, int pixelsPerMicron)
            {
                this.totalWidth = totalWidth;
                this.totalHeight = totalHeight;
                this.colorDepth = colorDepth;
                this.type = type;
                this.originalPixelsPerMicron = pixelsPerMicron;
            }

            public string Prefix
            {
                get
                {
                    return this.prefix;
                }
                set
                {
                    this.prefix = value;
                }
            }

            public string DirectoryPath
            {
                get
                {
                    return this.directoryPath;
                }
                set
                {
                    this.directoryPath = value + System.IO.Path.DirectorySeparatorChar;
                }
            }

 /*           public string FileCachePath
            {
                get
                {
                    return this.DirectoryPath + System.IO.Path.DirectorySeparatorChar +
                        this.Prefix + ".mos";
                }
            }
*/
            public void AddMetaData(string key, string value)
            {
                this.metaData.Add(key, value);
            }

            public Dictionary<string, string> MetaData
            {
                get
                {
                    return this.metaData;
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

            internal List<Tile> Items
            {
                get
                {
                    return this.tileList;
                }
            }

            public FREE_IMAGE_TYPE FreeImageType
            {
                get
                {
                    return this.type;
                }
                set
                {
                    this.type = value;
                }
            }

            public int WidthInTiles
            {
                get
                {
                    return this.widthInTiles;
                }
                set
                {
                    this.widthInTiles = value;
                }
            }

            public int HeightInTiles
            {
                get
                {
                    return this.heightInTiles;
                }
                set
                {
                    this.heightInTiles = value;
                }
            }

            public int NumberOfTiles
            {
                get
                {
                    return this.widthInTiles * this.heightInTiles;
                }
            }

            public double ThumbnailScaleMinIntensity
            {
                get
                {
                    return this.thumbnailScaleMinIntensity;
                }
            }

            public double ThumbnailScaleMaxIntensity
            {
                get
                {
                    return this.thumbnailScaleMaxIntensity;
                }
            }

            public double TotalMinIntensity
            {
                get
                {
                    return this.totalMinIntensity;
                }
                set
                {
                    this.totalMinIntensity = value;
                }
            }

            public double TotalMaxIntensity
            {
                get
                {
                    return this.totalMaxIntensity;
                }
                set
                {
                    this.totalMaxIntensity = value;
                }
            }

            public double ScaleMinIntensity
            {
                get
                {
                    return this.scaleMinIntensity;
                }
                set
                {
                    this.scaleMinIntensity = value;

                    this.thumbnailScaleMinIntensity = Math.Floor(255.0 * (this.scaleMinIntensity - this.TotalMinIntensity) /
                        (this.TotalMaxIntensity - this.TotalMinIntensity));
                }
            }

            public double ScaleMaxIntensity
            {
                get
                {
                    return this.scaleMaxIntensity;
                }
                set
                {
                    this.scaleMaxIntensity = value;

                    this.thumbnailScaleMaxIntensity =
                        Math.Ceiling(255.0 * (this.scaleMaxIntensity - this.TotalMinIntensity) /
                        (this.TotalMaxIntensity - this.TotalMinIntensity));
                }
            }

            public int ColorDepth
            {
                get
                {
                    return this.colorDepth;
                }
                set
                {
                    this.colorDepth = value;
                }
            }

            public double OverLapPercentageX
            {
                get
                {
                    return this.overLapPercentageX;
                }
                set
                {
                    this.overLapPercentageX = value;
                }
            }

            public double OverLapPercentageY
            {
                get
                {
                    return this.overLapPercentageY;
                }
                set
                {
                    this.overLapPercentageY = value;
                }
            }

            public double OriginalPixelsPerMicron
            {
                get
                {
                    return this.originalPixelsPerMicron;
                }
                set
                {
                    this.originalPixelsPerMicron = value;
                }
            }

            public double OriginalMicronsPerPixel
            {
                get
                {
                    return 1 / this.OriginalPixelsPerMicron;
                }
            }

            public bool IsColour
            {
                get
                {
                    if ((this.ColorDepth >= 24)
                        && this.FreeImageType == FREE_IMAGE_TYPE.FIT_BITMAP)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public int TotalWidth
            {
                get
                {
                    this.totalWidth = Tile.GetHorizontalRangeOfTiles(this.tileList);

                    return this.totalWidth;
                }
                set
                {
                    this.totalWidth = value;
                }
            }

            public int TotalHeight
            {
                get
                {
                    this.totalHeight = Tile.GetVerticalRangeOfTiles(this.tileList);

                    return this.totalHeight;
                }
                set
                {
                    this.totalHeight = value;
                }
            }

            public bool IsGreyScale
            {
                get
                {
                    if (this.ColorDepth >= 24 && this.type == FREE_IMAGE_TYPE.FIT_BITMAP)
                        return false;

                    return true;
                }
            }

            public List<Tile> GetTilesIntersectingRectangle(Rectangle rectangle)
            {
                return Tile.GetTilesIntersectingRectangle(this.tileList, rectangle);
            }
        }
        #endregion
    }
}
