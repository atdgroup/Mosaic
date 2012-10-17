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
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SingletonApp
{
    [Serializable]
    class SingletonController : MarshalByRefObject
    {
        private static TcpChannel m_TCPChannel = null;
        private static Mutex m_Mutex = null;

        public delegate void ReceiveDelegate(string[] args);

        static private ReceiveDelegate m_Receive = null;
        static public ReceiveDelegate Receiver
        {
            get
            {
                return m_Receive;
            }
            set
            {
                m_Receive = value;
            }
        }

        public static bool IamFirst(ReceiveDelegate r)
        {
            if (IamFirst())
            {
                Receiver += r;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IamFirst()
        {
            string m_UniqueIdentifier;
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName(false).CodeBase;
            m_UniqueIdentifier = assemblyName.Replace("\\", "_");

            m_Mutex = new Mutex(false, m_UniqueIdentifier);

            if (m_Mutex.WaitOne(1, true))
            {
                //We locked it! We are the first instance!!!    
                CreateInstanceChannel();
                return true;
            }
            else
            {
                //Not the first instance!!!
                m_Mutex.Close();
                m_Mutex = null;
                return false;
            }
        }

        private static void CreateInstanceChannel()
        {
            m_TCPChannel = new TcpChannel(1234);
            ChannelServices.RegisterChannel(m_TCPChannel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                Type.GetType("SingletonApp.SingletonController"),
                "SingletonController",
                WellKnownObjectMode.SingleCall);
        }

        public static void Cleanup()
        {
            if (m_Mutex != null)
            {
                m_Mutex.Close();
            }

            if (m_TCPChannel != null)
            {
                m_TCPChannel.StopListening(null);
            }

            m_Mutex = null;
            m_TCPChannel = null;
        }

        public static void Send(string[] s)
        {
            SingletonController ctrl;
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            try
            {
                ctrl = (SingletonController)Activator.GetObject(typeof(SingletonController), "tcp://localhost:1234/SingletonController");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                throw;
            }
            ctrl.Receive(s);
        }

        public void Receive(string[] s)
        {
            if (m_Receive != null)
            {
                m_Receive(s);
            }
        }
    }
}
