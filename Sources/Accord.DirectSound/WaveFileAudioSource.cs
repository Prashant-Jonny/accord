﻿// Accord (Experimental) Audio Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.DirectSound
{
    using System;
    using System.Threading;
    using Accord.Audio;
    using Accord.Audio.Formats;
    using SlimDX.Multimedia;
    using System.IO;

    /// <summary>
    ///   Wave file audio source.
    /// </summary>
    /// 
    /// <remarks><para>The audio source reads Wave files using a WaveFileReader.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create Wave file audio source
    /// WaveFileAudioSource source = new WaveFileAudioSource( "some file" );
    /// // set event handlers
    /// source.NewFrame += new NewFrameEventHandler( audio_NewFrame );
    /// // start the audio source
    /// source.Start( );
    /// // ...
    /// // signal to stop
    /// source.SignalToStop( );
    /// 
    /// // New frame event handler, which is invoked on each new available audio frame
    /// private void audio_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     float[][] frame = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class WaveFileAudioSource : IAudioSource, IDisposable
    {
        // audio source stream
        private Stream stream;

        // user data associated with the audio source
        private object userData = null;

        // received frames count
        private int framesReceived;

        // get frame interval from source or use manually specified
        private int bytesReceived;

        // desired size for each frame
        private int frameSize = 8192;

        private WaveDecoder decoder;
        private string fileName;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;


        /// <summary>
        ///   Event raised when a new frame has arrived.
        /// </summary>
        /// 
        public event EventHandler<NewFrameEventArgs> NewFrame;

        /// <summary>
        ///   Event raised when an error occurs in the audio source.
        /// </summary>
        /// 
        public event EventHandler<AudioSourceErrorEventArgs> AudioSourceError;


        /// <summary>
        ///   Gets or sets the file source path.
        /// </summary>
        /// 
        public string Source
        {
            get { return fileName; }
            set
            {
                fileName = value;
                stream = null;
            }
        }

        /// <summary>
        ///   Gets or sets the desired frame size to use when reading this source.
        /// </summary>
        /// 
        public int DesiredFrameSize
        {
            get { return frameSize; }
            set { frameSize = value; }
        }

        /// <summary>
        ///   Gets the quantity of frames received.
        /// </summary>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        ///   Gets the quantity of bytes received.
        /// </summary>
        /// 
        public int BytesReceived
        {
            get
            {
                int bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
        }

        /// <summary>
        ///   Gets or sets a user-defined tag associated with this object.
        /// </summary>
        /// 
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        /// <summary>
        ///   Gets whether this source is active or not.
        /// </summary>
        /// 
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        /// <summary>
        ///   Starts reading from the source.
        /// </summary>
        /// 
        public void Start()
        {
            if (thread == null)
            {
                // check source
                if (String.IsNullOrEmpty(fileName))
                    throw new ArgumentException("Audio source is not specified");

                if (!System.IO.File.Exists(fileName))
                    throw new ArgumentException("Source file does not exists");

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent(true);

                // create and start new thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Name = fileName; // mainly for debugging
                thread.Start();
            }
        }

        /// <summary>
        ///   Signals the source to stop.
        /// </summary>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        ///   Blocks the calling thread until the source has stopped.
        /// </summary>
        /// 
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        /// <summary>
        ///   Stops the source.
        /// </summary>
        /// 
        public void Stop()
        {
            if (this.IsRunning)
            {
                thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        ///   Constructs a new Wave file audio source.
        /// </summary>
        /// 
        /// <param name="fileName">The path for the underlying source.</param>
        /// 
        public WaveFileAudioSource(string fileName)
        {
            this.fileName = fileName;
            this.decoder = new WaveDecoder();
        }

        /// <summary>
        ///   Constructs a new Wave file audio source.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing a Wave file.</param>
        /// 
        public WaveFileAudioSource(Stream stream)
        {
            this.stream = stream;
            this.decoder = new WaveDecoder();
        }


        /// <summary>
        ///   Free resource.
        /// </summary>
        ///
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }

        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private void WorkerThread()
        {
            WaveStream waveStream = null;

            try
            {
                waveStream = (stream != null) ?
                    new WaveStream(stream) : new WaveStream(fileName);

                // Open the Wave stream
                decoder.Open(waveStream);

                while (stopEvent.WaitOne(0, false))
                {
                    // get next frame
                    Signal s = decoder.Decode(frameSize);
                    framesReceived += s.Length;
                    bytesReceived += decoder.Bytes;

                    if (NewFrame != null)
                        NewFrame(this, new NewFrameEventArgs(s));

                    // check current position
                    if (waveStream.Position >= waveStream.Length)
                        break;

                    // sleeping ...
                    Thread.Sleep(100);
                }
            }
            catch (Exception exception)
            {
                // provide information to clients
                if (AudioSourceError != null)
                {
                    AudioSourceError(this, new AudioSourceErrorEventArgs(exception.Message));
                }
                else
                {
                    throw;
                }
            }

            if (waveStream != null)
            {
                waveStream.Close();
                waveStream.Dispose();
                waveStream = null;
            }
        }

        /// <summary>
        ///   Gets whether the current source supports seeking.
        /// </summary>
        /// 
        public bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        ///   Navigates to a given position within the source.
        /// </summary>
        /// 
        /// <param name="frameIndex">The frame position to navigate to.</param>
        /// 
        public void Seek(int frameIndex)
        {
            this.decoder.Seek(frameIndex);
        }

        /// <summary>
        ///   Gets the sampling rate for this source.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return decoder.SampleRate; }
            set { throw new NotSupportedException(); }
        }



        #region IDisposable members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (stopEvent != null)
                {
                    stopEvent.Close();
                    stopEvent = null;
                }
            }
        }
        #endregion


    }
}
