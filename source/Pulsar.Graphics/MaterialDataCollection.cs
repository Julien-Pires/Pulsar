using System.Collections.Generic;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics
{
    internal sealed class MaterialDataCollection<T> : Dictionary<string, T>, IMaterialDataCollection<T>
    {
        #region Methods

        public void BindToTechnique(TechniqueBinding technique)
        {
            foreach (KeyValuePair<string, T> pair in this)
                technique.SetConstantValue(pair.Key, pair.Value);
        }

        #endregion
    }
}
