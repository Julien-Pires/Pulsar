using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Pulsar.Assets;
using Pulsar.Graphics;

namespace Pulsar.SceneDemo.SolarSystem
{
    public sealed class SolarSystem : ISceneDemo
    {
        #region Fields

        private readonly int planetCount;
        private const string modelsFolder = "models/Planets/";
        private readonly string[] planetsModel = { "Mercure/mercure", "Venus/venus", "Earth/earth", "Mars/mars", 
                                                     "Jupiter/jupiter", "Saturne/saturne", "Neptune/neptune" };
        private readonly string[] planetsName = { "Mercure", "Venus", "Earth", "Mars", "Jupiter", "Saturne", "Neptune" };
        private readonly float[] planetsGap = { 58.0f, 108.0f, 149.6f, 228.0f, 740.0f, 1429.0f, 4540.0f };
        private readonly float[] planetSize = { 4880.0f, 12100.0f, 12756.0f, 6792.0f, 142984.0f, 120536.0f, 49532.0f };

        private CameraController camCtrl = null;
        private SceneGraph graph = null;
        private List<Planet> planets = new List<Planet>();

        #endregion

        #region Constructors

        public SolarSystem(Root r, ContentManager cntMngr)
        {
            this.graph = r.CreateSceneGraph("Solar System");
            this.planetCount = this.planetsModel.Length;
        }

        #endregion

        #region Methods

        public void Load()
        {
            this.CreateCamera();
            this.CreateSystem();
        }

        public void Render()
        {
            this.graph.RenderScene();
        }

        public void Update(GameTime time)
        {
            this.camCtrl.Tick(time);

            for (int i = 0; i < this.planets.Count; i++)
            {
                this.planets[i].Update();
            }
        }

        private void CreateCamera()
        {
            Camera mainCam = new Camera("MainCam");
            mainCam.FarPlane = 5000.0f;

            CameraManager camMngr = this.graph.CameraManager;
            camMngr.AddCamera(mainCam);
            camMngr.UseCamera(mainCam);

            this.camCtrl = new CameraController(mainCam);
        }

        private void CreateSystem()
        {
            SceneNode rootNode = this.graph.Root;

            for (int i = 0; i < this.planetCount; i++)
            {
                this.AddPlanet(i, rootNode);
            }
        }

        private void AddPlanet(int planetIdx, SceneNode parent)
        {
            string mesh = SolarSystem.modelsFolder + this.planetsModel[planetIdx];
            Entity modelPlanet = this.graph.CreateEntity(mesh, this.planetsName[planetIdx]);
            SceneNode node = parent.CreateChildSceneNode(this.planetsName[planetIdx]);
            node.AttachObject(modelPlanet);
            modelPlanet.RenderAABB = true;
            Planet plt = new Planet(this.planetsName[planetIdx], this.planetSize[planetIdx], this.planetsGap[planetIdx])
                {
                    Model = modelPlanet,
                    Node = node
                };
            this.planets.Add(plt);
        }

        #endregion
    }
}
