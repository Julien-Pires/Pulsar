using System;

namespace Pulsar.Input
{
    internal delegate bool CommandCheckState();

    internal interface IInputCommand
    {
        #region Properties

        bool IsTriggered { get; }

        #endregion
    }
}
