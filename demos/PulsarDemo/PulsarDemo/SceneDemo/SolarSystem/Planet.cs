using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;

namespace PulsarDemo.SceneDemo.SolarSystem
{
    public class Planet
    {
        #region Fields

        private const float distanceRatio = 5000.0f / 5129.0f;

        private string name;
        private float startOrbit = 0.0f;
        private SceneNode node = null;
        private SceneNode parent = null;
        private Entity model = null;

        #endregion

        #region Constructors

        public Planet(string name, Entity planet, float orbit)
        {
            this.name = name;
            this.model = planet;
            this.node = this.model.Parent;
            this.startOrbit = orbit;
            this.InitializeNode();
        }

        #endregion

        #region Methods

        internal void Update()
        {
            this.node.Yaw(MathHelper.ToRadians(1.0f), TransformSpace.Local);
        }

        private void InitializeNode()
        {
            this.parent = (SceneNode)this.node.Parent;
            this.node.SetPosition(new Vector3(0.0f, 0.0f, -this.startOrbit));
        }

        #endregion
    }
}
