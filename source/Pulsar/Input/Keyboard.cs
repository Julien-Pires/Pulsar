using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Pulsar.Extension;

using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Pulsar.Input
{
    public static class Keyboard
    {
        #region Fields

        private static Keys[] AllDigital;
        internal static List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private static KeyboardState previousState;
        private static KeyboardState currentState;

        #endregion

        #region Constructors

        static Keyboard()
        {
            Keyboard.Initialize();
        }

        #endregion

        #region Methods

        internal static void Initialize()
        {
#if !XBOX
            Keyboard.AllDigital = (Keys[])Enum.GetValues(typeof(Keys));
#else
            Keyboard.AllDigital = EnumExtension.GetValues<Keys>();
#endif
        }

        internal static void Update()
        {
            Keyboard.previousState = Keyboard.currentState;
            Keyboard.currentState = XnaKeyboard.GetState();

            Keyboard.ButtonPressed.Clear();
            for (int i = 0; i < Keyboard.AllDigital.Length; i++)
            {
                if(Keyboard.IsPressed(Keyboard.AllDigital[i]))
                {
                    AbstractButton btn = new AbstractButton(Keyboard.AllDigital[i]);
                    Keyboard.ButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed));
                }
            }
        }

        public static bool AnyKeyPressed()
        {
            return Keyboard.ButtonPressed.Count > 0;
        }

        public static bool IsPressed(Keys key)
        {
            return Keyboard.previousState.IsKeyUp(key) && Keyboard.currentState.IsKeyDown(key);
        }

        public static bool IsReleased(Keys key)
        {
            return Keyboard.previousState.IsKeyDown(key) && Keyboard.currentState.IsKeyUp(key);
        }

        public static bool IsDown(Keys key)
        {
            return Keyboard.currentState.IsKeyDown(key);
        }

        public static bool IsUp(Keys key)
        {
            return Keyboard.currentState.IsKeyUp(key);
        }

        #endregion
    }
}
