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
using System.Threading;
using System.ComponentModel;

namespace ThreadingSystem
{
	/// <summary>
	/// Delegate for the ProgressCompleted event.
	/// </summary>
    public delegate void ThreadProgressDelegate(object sender, string updateText, int percentage);

	/// <summary>
	/// Delegate for the ThreadStarted event.
	/// </summary>
    public delegate void ThreadStartedDelegate(object sender, string updateText);

	/// <summary>
	/// Delegate for the ThreadCompleted event.
	/// </summary>
    public delegate void ThreadCompletedDelegate(object sender, string updateText, bool aborted);

	/// <summary>
	/// Class used to control IThreadObjects and update IThreadIndicators.
	/// </summary>
	public class ThreadController
	{
		private ISynchronizeInvoke invokeObject;

        private ThreadProgressDelegate threadProgressFunction;
        private ThreadStartedDelegate threadStartFunction;
        private ThreadCompletedDelegate threadCompletedFunction;

		private volatile bool threadRunning;
		private volatile bool threadAborted;
        private volatile bool threadPaused;
		private Thread thread;
		
		/// <summary>
		/// ThreadController constructor.
		/// </summary>
		public ThreadController(ISynchronizeInvoke invokeObject)
		{
			this.threadRunning = false;
			this.threadAborted = false;
            this.threadPaused = false;
		
			this.invokeObject = invokeObject;	
		}

        public ISynchronizeInvoke InvokeObject
        {
            get
            {
                return this.invokeObject;
            }
        }

        public void SetThreadProgressCallback(ThreadProgressDelegate threadProgressFunction)
        {
            this.threadProgressFunction = threadProgressFunction;
        }

        public void SetThreadStartedCallback(ThreadStartedDelegate threadStartFunction)
        {
            this.threadStartFunction = threadStartFunction;
        }

        public void SetThreadCompletedCallback(ThreadCompletedDelegate threadCompletedFunction)
        {
            this.threadCompletedFunction = threadCompletedFunction;
        }

		/// <summary>
		/// Method to abort a IThreadObject from executing.
		/// </summary>
		public void AbortThread()
		{
			this.threadAborted = true;	

            if(this.thread != null)
			    this.thread.Join();
		}

        public void PauseThread()
        {
            this.threadPaused = true;
            this.threadRunning = false;
        }

        public void ResumeThread()
        {
            this.threadPaused = false;
            this.threadRunning = true;
        }

		/// <summary>
		/// Property indication the running status of a IThreadObject.
		/// </summary>
		public bool ThreadRunning
		{
			get
			{
				return this.threadRunning;
			}
		}
		
		/// <summary>
		/// Property indication the aborted status of a IThreadObject.
		/// </summary>
		public bool ThreadAborted
		{
			get
			{
				return this.threadAborted;
			}
		}

        public bool ThreadPaused
        {
            get
            {
                return this.threadPaused;
            }
        }

		/// <summary>
		/// Method meant to be called by a IThreadObject to indicated 
		/// how much work the it has completed.
		/// </summary>
		public void ReportThreadPercentage(object sender, string updateText, int percentage)
		{
            if (this.invokeObject == null)
				return;

            if (this.threadAborted)
                return;
	
            Object[] objects = {sender, updateText, percentage };
            this.invokeObject.BeginInvoke(this.threadProgressFunction, objects);
		}

        public void ReportThreadPercentage(object sender, string updateText, int position, int total)
        {
            if (this.threadAborted)
                return;

            if (position > total)
                position = total;

            float percentage = (float)position / total;

            this.ReportThreadPercentage(this, updateText, (int) (percentage * 100.0));
        }
		
		/// <summary>
		/// Method meant to be called by a IThreadObject to indicated 
		/// whether it has completed its work.
		/// </summary>
        public void ReportThreadCompleted(object sender, string updateText, bool aborted)
		{
			this.threadRunning = false;

            if (this.invokeObject == null)
				return;	

			Object[] objects = {sender, updateText, aborted};

            this.invokeObject.BeginInvoke(this.threadCompletedFunction, objects);
		}
		
		/// <summary>
		/// Method meant to be called by a IThreadObject to indicated 
		/// whether it has started its work.
		/// </summary>
        public void ReportThreadStarted(object sender, string updateText)
		{
            if (this.invokeObject == null)
				return;

            if (this.threadAborted)
                return;

			Object[] objects = {sender, updateText};

            this.invokeObject.BeginInvoke(this.threadStartFunction, objects);
		}

		/// <summary>
		/// Method to start an IThreadObject.
		/// </summary>
		public void ThreadStart(string threadName, ThreadStart threadStart)  
		{	
			if(this.ThreadRunning == true)
				return;

            this.thread = new Thread(threadStart);
            this.thread.Name = threadName;

			this.threadAborted = false;
            this.threadPaused = false;
			this.threadRunning = true;
			
			thread.Start();
		}

		/// <summary>
		/// Method to join an IThreadObject.
		/// </summary>
		public void ThreadJoin()  
		{	
			if(this.ThreadRunning == false)
				return;

			thread.Join();
		}
	}
}
