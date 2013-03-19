using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsar.Core
{
    public interface IEventHandler
    {
        #region Methods

        void HandleEvent(Message msg);

        #endregion
    }
}
