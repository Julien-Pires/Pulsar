using System;

namespace Pulsar.Input
{
    internal delegate bool CheckCommandState();

    internal interface IInputCommand
    {
        #region Properties

        bool IsTriggered { get; }

        #endregion
    }
}
