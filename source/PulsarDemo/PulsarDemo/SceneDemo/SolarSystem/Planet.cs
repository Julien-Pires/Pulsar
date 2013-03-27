using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Graphics;
using Pulsar.Graphics.Graph;

namespace PulsarDemo.SceneDemo.SolarSystem
{
    public class Planet
    {
        #region Fields

        private const float sizeRatio = 1.0f / 1000.0f;
        private const float distanceRatio = 1.0f / 1.5f;

        private string name;
        private float realSize = 0.0f;
        private float parentGap = 0.0f;
        private float normalizeScale = 1.0f;
        private float finalScale = 1.0f;
        private float finalPosition = 0.0f;
        private SceneNode node = null;
        private SceneNode parent = null;
        private Entity model = null;

        #endregion

        #region Constructors

        public Planet(string name, float realSize, float parentGap)
        {
            this.name = name;
            this.realSize = realSize;
            this.parentGap = parentGap;
        }

        #endregion

        #region Methods

        internal void Update()
        {
            this.node.Yaw(MathHelper.ToRadians(1.0f));
        }

        private void InitializeNode()
        {
            if ((this.node == null) || (this.model == null))
            {
                return;
            }
            
            this.parent = (SceneNode)this.node.Parent;
            this.normalizeScale = 1.0f / (model.Mesh.BoundingSphere.Radius * 2.0f);
            this.finalScale = this.realSize * Planet.sizeRatio;
            this.finalPosition = Planet.distanceRatio * this.parentGap;

            this.node.DoScale(this.normalizeScale);
            this.node.DoScale(this.finalScale);
            this.node.SetPosition(new Vector3(0.0f, 0.0f, -this.finalPosition));
        }

        #endregion

        #region Properties

        internal SceneNode Node
        {
            get { return this.node; }
            set
            {
                this.node = value;
                this.InitializeNode();
            }
        }

        internal Entity Model
        {
            get { return this.model; }
            set
            {
                this.model = value;
                this.InitializeNode();
            }
        }

        #endregion
    }
}
