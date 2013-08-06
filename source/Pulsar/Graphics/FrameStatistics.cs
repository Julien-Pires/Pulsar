using System;
using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains information about one frame
    /// </summary>
    public sealed class FrameStatistics
    {
        #region Fields

        private double _elapsedTime;
        private int _historicLength = 1;
        private FrameDetail[] _frameDetails;

        #endregion

        #region Constructor

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
        /// ComputeFramerate FrameStatistics counter
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

        public FrameDetail GetFrame(int index)
        {
            return _frameDetails[index];
        }

        #endregion

        #region Properties

        internal FrameDetail CurrentFrame { get; private set; }

        /// <summary>
        /// Get or set the number of frame drawn since the last reset
        /// </summary>
        internal int Framecount { get; set; }

        /// <summary>
        /// Get the number of frame drawn by seconds
        /// </summary>
        public int Framerate { get; private set; }

        public FrameDetail LastFrame
        {
            get { return _frameDetails[0]; }
        }

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