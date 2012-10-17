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
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    public delegate void CorrelatorTextFeedbackDelegate(string text, Color color);
    public delegate void CorrelatorTileCorrelationBeginDelegate(CorrelationTile tile, FreeImageAlgorithmsBitmap fib);
    public delegate void CorrelatorTileCorrelationPausedOrResumedDelegate(bool paused);
    public delegate void CorrelatorTileCorrelatedDelegate(CorrelationTile tile, Rectangle correlatedBounds, FreeImageAlgorithmsBitmap fib, bool success);
    public delegate void CorrelatorTileCorrelatedCompletedDelegate(string updateText, bool aborted);
    public delegate void CorrelatorTileCorrelationProgressDelegate(string updateText, int percentage);

    public class CorrelatorException : MosaicException
    {
        public CorrelatorException(string message)
            : base(message)
        {
        }

    }

    public abstract class Correlator
    {
        private List<CorrelationTile> items;
        private DateTime startTime;
        private DateTime stopTime;
        private ThreadController threadController;
        private CorrelatorTextFeedbackDelegate textFeedbackHandler;
        private CorrelatorTileCorrelatedDelegate tileCorrelatedHandler;
        private CorrelatorTileCorrelationBeginDelegate tileCorrelatedBeginHandler;
        private CorrelatorTileCorrelationPausedOrResumedDelegate correlationPausedOrResumedHandler;
        private CorrelatorTileCorrelatedCompletedDelegate tileCorrelatedCompletedHandler;
        private CorrelatorTileCorrelationProgressDelegate tileCorrelatedProgressHandler;
        private FreeImageAlgorithmsBitmap backgroundImage;
        private MosaicInfo mosaicInfo;   
        private int numberOfCorrelatedTiles = 0;
        private int numberOfFailedCorrelations = 0;
        private TiledImageView correlationDisplayViewer;

        private Correlator(TiledImageView correlationDisplayViewer)
        {
            this.correlationDisplayViewer = correlationDisplayViewer;
            this.items = new List<CorrelationTile>();
        }

        protected TiledImageView CorrelationDisplayViewer
        {
            get
            {
                return this.correlationDisplayViewer;
            }
        }

        private void CreateCorrelationTileList()
        {
            // Make a new sorted list of tiles where each tile is the CorrelationTile object.
            foreach (Tile t in this.MosaicInfo.Items)
            {
                this.items.Add(new CorrelationTile(t));
            }
        }

        public Correlator(MosaicInfo mosaicInfo, TiledImageView correlationDisplayViewer)
            : this(correlationDisplayViewer)
        {
            this.mosaicInfo = mosaicInfo;

            CreateCorrelationTileList();         
        }

        public List<CorrelationTile> CorrelationTiles
        {
            get
            {
                return this.items;
            }
        }

        private void Correlate()
        {
            this.items = new List<CorrelationTile>();

            // Make a new sorted list of tiles where each tile is the CorrelationTile object.
            foreach (Tile t in this.MosaicInfo.Items)
            {
                this.items.Add(new CorrelationTile(t));
            }

            Correlate(this.CorrelationTiles);
        }

        protected abstract void Correlate(List<CorrelationTile> items);

        private void CreateBackgroundImage()
        {
            // All tiles are assumed to be the same size.
            Tile tile = this.MosaicInfo.Items[0];

            int width = tile.Width * 3;
            int height = tile.Height * 3;

            // Create a full res scratch image
            // This should be 3x3 images in size and should never need to be larger as
            // it is only used for correlation a small region of the tiles at a time.     
 //           if (this.MosaicInfo.IsGreyScale)
                this.backgroundImage = new FreeImageAlgorithmsBitmap(width, height, 8);
 //           else
 //               this.backgroundImage = new FreeImageAlgorithmsBitmap(width, height, 24);

        }

        public virtual MosaicInfo MosaicInfo
        {
            set
            {
                this.mosaicInfo = value;
                CreateCorrelationTileList();         
                this.CreateBackgroundImage();
            }
            get
            {
                return this.mosaicInfo;
            }
        }

        public virtual CorrelatorOptionPanel OptionPanel
        {
            get
            {
                return null;
            }
        }

        public int NumberOfCorrelatedTiles
        {
            get
            {
                return this.numberOfCorrelatedTiles;
            }
            set
            {
                this.numberOfCorrelatedTiles = value;
            }
        }

        public int NumberOfFailedCorrelations
        {
            get
            {
                return this.numberOfFailedCorrelations;
            }
            set
            {
                this.numberOfFailedCorrelations = value;
            }
        }

        public ThreadController ThreadController
        {
            get
            {
                return this.threadController;
            }
            set
            {
                this.threadController = value;
                this.threadController.SetThreadCompletedCallback(TileCorrelationThreadCompleted);
                this.threadController.SetThreadProgressCallback(TileCorrelationThreadProgressIndicator);
            }
        }

        public void CorrelationPause()
        {     
            this.threadController.PauseThread();

            Object[] objects = { true };

            this.threadController.InvokeObject.Invoke(this.CorrelationPausedOrResumedHandler, objects);
        }

        public void CorrelationResume()
        {
            this.threadController.ResumeThread();

            Object[] objects = { false };

            this.threadController.InvokeObject.Invoke(this.CorrelationPausedOrResumedHandler, objects);
        }

        public void CorrelationAbort()
        {
            this.stopTime = DateTime.Now;
            this.threadController.AbortThread();
        }

        public CorrelatorTextFeedbackDelegate TextFeedbackDelegate
        {
            set
            {
                this.textFeedbackHandler = value;
            }
        }

        public CorrelatorTileCorrelationBeginDelegate TileCorrelationBeginHandler
        {
            get
            {
                return this.tileCorrelatedBeginHandler;
            }
            set
            {
                this.tileCorrelatedBeginHandler = value;
            }
        }

        public CorrelatorTileCorrelationPausedOrResumedDelegate CorrelationPausedOrResumedHandler
        {
            get
            {
                return this.correlationPausedOrResumedHandler;
            }
            set
            {
                this.correlationPausedOrResumedHandler = value;
            }
        }

        public CorrelatorTileCorrelatedCompletedDelegate TileCorrelatedCompletedHandler
        {
            get
            {
                return this.tileCorrelatedCompletedHandler;
            }
            set
            {
                this.tileCorrelatedCompletedHandler = value;
            }
        }

        public CorrelatorTileCorrelationProgressDelegate TileCorrelatedProgressHandler
        {
            get
            {
                return this.tileCorrelatedProgressHandler;
            }
            set
            {
                this.tileCorrelatedProgressHandler = value;
            }
        }     

        public CorrelatorTileCorrelatedDelegate TileCorrelatedHandler
        {
            get
            {
                return this.tileCorrelatedHandler;
            }
            set
            {
                this.tileCorrelatedHandler = value;
            }
        }

        public FreeImageAlgorithmsBitmap BackgroundImage
        {
            get
            {
                return this.backgroundImage;
            }
        }

        public void StopCorrelation()
        {
            if(this.ThreadController != null)
                this.ThreadController.AbortThread();

            // Give subclass a thread free oportunity to do stuff.
            this.CorrelationStopped();
        }

        protected virtual void CorrelationStarted() { }

        protected virtual void CorrelationStopped() {}

        public void StartCorrelation()
        {
            // Abort any previous runs
            this.ThreadController.AbortThread();

            // Give subclass a thread free oportunity to do stuff.
            this.CorrelationStarted();
            Tile.SetUseAjustedPositionForAllTiles(this.MosaicInfo.Items, false);

             // We are starting a correlation lets remove any previous correleations
            Tile.ResetAjustedPositionForAllTiles(this.MosaicInfo.Items);

            this.startTime = DateTime.Now;
            this.threadController.ThreadStart("TileCorrelationThread", new ThreadStart(this.Correlate));
        }

        public void Reset()
        {
            this.backgroundImage.Clear();
        }

        protected void SendFeedback(string text, Color color)
        {
            if (this.ThreadController.ThreadAborted)
                return;

            Object[] objects = { text, color };

            this.threadController.InvokeObject.BeginInvoke(this.textFeedbackHandler, objects);
        }

        protected void SendFeedback(string text)
        {
            SendFeedback(text, Color.Black);
        }

        protected void SendTileCorrelatationBegin(CorrelationTile tile, FreeImageAlgorithmsBitmap fib)
        {
            if (this.ThreadController.ThreadAborted)
                return;

            Object[] objects = { tile, fib };

            this.threadController.InvokeObject.BeginInvoke(this.TileCorrelationBeginHandler, objects);
        }

        protected void SendTileCorrelated(CorrelationTile tile, Rectangle correlatedBounds, FreeImageAlgorithmsBitmap fib, bool success)
        {
            if (this.ThreadController.ThreadAborted)
                return;

            Object[] objects = { tile, correlatedBounds, fib, success};

            this.threadController.InvokeObject.BeginInvoke(this.TileCorrelatedHandler, objects);
        }

        public TimeSpan TimeTaken()
        {
            TimeSpan duration = stopTime - startTime;
            return duration;
        }

        private void TileCorrelationThreadCompleted(object sender, string updateText, bool aborted)
        {
            this.stopTime = DateTime.Now;
            
            if(this.tileCorrelatedCompletedHandler != null)
                this.tileCorrelatedCompletedHandler(updateText, aborted);
        }

        public void TileCorrelationThreadProgressIndicator(object sender, string updateText, int percentage)
        {
            if (this.TileCorrelatedProgressHandler != null)
                TileCorrelatedProgressHandler(updateText, percentage);
        }

        public virtual void DebugBackgroundPaint(CorrelationDebugForm debugForm, PaintEventArgs e)
        {
        }

        public virtual void DebugCurrentImagePaint(CorrelationDebugForm debugForm, PaintEventArgs e)
        {
        }

        protected void ProcessPause()
        {
            // Sit in a tight while loop if the thread is paused.
            while (this.ThreadController.ThreadPaused)
            {
                if (this.ThreadController.ThreadAborted)
                    return;
            }
        }
    }
}
