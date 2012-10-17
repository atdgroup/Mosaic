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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;

using SingletonApp;

namespace ImageStitching
{
    public class MosaicException : ApplicationException
    {
        public MosaicException(string message)
            : base(message)
        {
        }

    }

    public class MosaicReaderException : MosaicException
    {
        public MosaicReaderException(string message)
            : base(message)
        {
        }

    }

	/// <summary>
	/// Summary description for ImageStitcher.
	/// </summary>
    public sealed class MosaicApp : MarshalByRefObject
	{
        static private MosaicWindow window = null;

		private MosaicApp()
		{
		}

        private static void GetPassedArgs(string[] args)
        {
            // If no arguments have been passed just bring the old window
            // to the front
            if (args.Length > 0)
            {
                // Display the new image
                MosaicApp.window.Open(args);
            }
        }

		[STAThread]
		public static void Main(string[] args)
		{
            MosaicApp.window = new MosaicWindow();

            SingletonController.Receiver += new SingletonController.ReceiveDelegate(MosaicApp.GetPassedArgs);
/*  disabled the singleton control
            // test if this is the first instance and register receiver, if so.
            if (SingletonController.IamFirst())
            {
*/
                if (args.Length > 0)
                    MosaicApp.window.SetStartupFiles(args);

                Application.Run(window);
/*
            }
            else
            {
                // send command line args to running app, then terminate
                SingletonController.Send(args);
            }
*/
         
            SingletonController.Cleanup();
		}
	}
}
