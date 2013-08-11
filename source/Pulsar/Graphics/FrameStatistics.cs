using System;
using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains statistics about rendered frames and framerate.
    /// Frame statistics are stored in logs when a frame is rendered. The logs size can be adjusted.
    /// By default the size is 1 which means we only keep information about the last frame.
    /// The most recent frame stats corresponds to the last frame drawn, not the current one.
    /// </summary>
    public sealed class FrameStatistics
    {
        #region Fields

        private double _elapsedTime;
        private int _historicLength = 1;
        private FrameDetail[] _frameDetails;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of FrameStatistics class
        /// </summary>
        internal FrameStatistics()
        {
            CurrentFrame = new FrameDetail();
            _frameDetails = new FrameDetail[_historicLength];
            for (int i = 0; i < _historicLength; i++)
            {
                _frameDetails[i] = new FrameDetail();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resize logs of frames statistics 
        /// </summary>
        private void Resize()
        {
            FrameDetail[] newDetails = new FrameDetail[_historicLength];
            int previousLength = _frameDetails.Length;
            for (int i = 0; i < _historicLength; i++)
            {
                if(i < previousLength) newDetails[i] = _frameDetails[i];
                else newDetails[i] = new FrameDetail();
            }
            _frameDetails = newDetails;
        }

        /// <summary>
        /// Update the framerate
        /// </summary>
        /// <param name="time">Time since the last update</param>
        internal void ComputeFramerate(GameTime time)
        {
            _elapsedTime += time.ElapsedGameTime.TotalMilliseconds;
            if (_elapsedTime <= 1000.0) return;

            _elapsedTime -= 1000.0;
            Framerate = Framecount;
            Framecount = 0;
        }

        /// <summary>
        /// Push the current frame at the top of the logs
        /// </summary>
        internal void SaveCurrentFrame()
        {
            int lastIdx = _historicLength - 1;
            FrameDetail newCurrent = _frameDetails[lastIdx];
            if (_historicLength > 1)
            {
                for (int i = lastIdx; i > 0; i--)
                {
                    _frameDetails[i] = _frameDetails[i - 1];
                }
            }
            newCurrent.Reset();
            _frameDetails[0] = CurrentFrame;
            CurrentFrame = newCurrent;
        }

        /// <summary>
        /// Get a specific frame
        /// </summary>
        /// <param name="index">Index of the frame</param>
        /// <returns>Return the frame statistics at the specified index</returns>
        public FrameDetail GetFrame(int index)
        {
            return _frameDetails[index];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the current frame statistics
        /// </summary>
        internal FrameDetail CurrentFrame { get; private set; }

        /// <summary>
        /// Get or set the number of frame drawn since the last reset
        /// </summary>
        internal int Framecount { get; set; }

        /// <summary>
        /// Get the number of frame drawn by seconds
        /// </summary>
        public int Framerate { get; private set; }

        /// <summary>
        /// Get the most recent frame statistics
        /// </summary>
        public FrameDetail LastFrame
        {
            get { return _frameDetails[0]; }
        }

        /// <summary>
        /// Get or set the length of frame statistics logs
        /// </summary>
        public int HistoricLength
        {
            get { return _historicLength; }
            set
            {
                if (value <= 0) throw new Exception("HistoricLength cannot be inferior or equal to zero");
                _historicLength = value;
                Resize();
            }
        }

        #endregion
    }
}