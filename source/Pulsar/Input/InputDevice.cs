using System;

namespace Pulsar.Input
{
    /// <summary>
    /// Enumerate all input devices
    /// </summary>
    [Flags]
    public enum InputDevice : byte
    { 
        None = 0,
        Mouse = 1,
        Keyboard = 2,
        GamePad = 4,
        AllGamePad = 8
    }
}