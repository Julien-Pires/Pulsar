using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Models;

namespace Pulsar.Graphics.Graph
{
    /// <summary>
    /// Factory to create Entity
    /// </summary>
    internal sealed class EntityFactory : Factory<Entity>
    {
        #region Methods

        /// <summary>
        /// Create an instance of the Entity class
        /// </summary>
        /// <param name="name">Name of the model used to create the Entity</param>
        /// <returns>Return an instance of the Entity class</returns>
        public override Entity Create(params string[] data)
        {
            string modelName = data[0];
            Mesh m = MeshManager.Instance.Load(modelName, "Default", modelName);
            Entity ent = new Entity(m);

            return ent;
        }

        #endregion
    }
}
