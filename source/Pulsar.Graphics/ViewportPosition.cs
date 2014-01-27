using System;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Specifies the position of a viewport, can be combined
    /// </summary>
    [Flags]
    public enum ViewportPosition
    {
        /// <summary>
        /// Occupies the half top
        /// </summary>
        Top = 0,

        /// <summary>
        /// Occupies the half bottom
        /// </summary>
        Bottom = 1,

        /// <summary>
        /// Occupies the half left
        /// </summary>
        Left = 2,

        /// <summary>
        /// Occupies the half right
        /// </summary>
        Right = 4
    }
}