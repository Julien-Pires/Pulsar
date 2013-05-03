using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    public enum InputDevice { Mouse, Keyboard, GamePad }

    public class Input
    {
        #region Methods

        internal static void Update()
        {
            Mouse.Update();
            Keyboard.Update();
            GamePad.Update();
        }

        #endregion
    }
}
