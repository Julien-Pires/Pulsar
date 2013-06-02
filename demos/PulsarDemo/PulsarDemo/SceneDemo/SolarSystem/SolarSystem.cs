using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Pulsar.Game;
using Pulsar.Input;
using Pulsar.Assets;
using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;

namespace PulsarDemo.SceneDemo.SolarSystem
{
    public sealed class SolarSystem : ISceneDemo
    {
        #region Fields

        private const string contentFolder = @"SolarSystem/";
        private const string modelsFolder = @"Models/";

        private readonly AssetStorage storage;
        private readonly string sunModel = "Sun/Sun";
        private readonly string sunName = "Sun";
        private readonly float sunOrbit = 0.0f;
        private readonly string[] planetsModel = { "Mercury/Mercury", "Venus/Venus", "Earth/Earth", "Mars/Mars", "Jupiter/Jupiter",
                                                   "Saturn/Saturn", "Uranus/Uranus", "Neptune/Neptune" };
        private readonly string[] planetsName = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };
        private readonly float[] planetsOrbit = { 150.0f, 290.0f, 420.0f, 600.0f, 1100.0f, 1600.0f, 2000.0f, 2410.0f };
        private CameraController camCtrl;
        private SceneTree graph;
        private List<Planet> planets = new List<Planet>();

        #endregion

        #region Constructors

        public SolarSystem(GraphicsEngine r, CameraController camCtrl)
        {
            this.storage = AssetStorageManager.Instance.CreateStorage("Default", "Content/SolarSystem");
            this.graph = r.CreateSceneGraph("SolarSystem");
            this.camCtrl = camCtrl;
        }

        #endregion

        #region Methods

        public void Load()
        {
            this.CreateCamera();
            this.CreateSystem();
        }

        public void Activate()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }

            this.camCtrl.Camera = this.graph.CameraManager.Current;
        }

        public void Render()
        {
            this.graph.RenderScene();
        }

        public void Update(GameTime time)
        {
            Camera c = camCtrl.Camera;
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
            mainCam.Translate(new Vector3(600.0f, 0.0f, 0.0f));
            mainCam.Yaw(MathHelper.ToRadians(90.0f));
            
            CameraManager camMngr = this.graph.CameraManager;
            camMngr.AddCamera(mainCam);
            camMngr.UseCamera(mainCam);
        }

        private void CreateSystem()
        {
            SceneNode rootNode = this.graph.Root;

            this.CreateSun(rootNode);
            for (int i = 0; i < this.planetsModel.Length; i++)
            {
                this.AddPlanet(i, rootNode);
            }
        }

        private void CreateSun(SceneNode parent)
        {
            string mesh = SolarSystem.modelsFolder + this.sunModel;
            Entity modelSun = this.graph.CreateEntity(mesh, this.sunName);
            modelSun.RenderAABB = true;
            SceneNode node = parent.CreateChildSceneNode(this.sunName);
            node.AttachObject(modelSun);

            Planet plt = new Planet(this.sunName, modelSun, this.sunOrbit);
            this.planets.Add(plt);
        }

        private void AddPlanet(int planetIdx, SceneNode parent)
        {
            string mesh = SolarSystem.modelsFolder + this.planetsModel[planetIdx];
            Entity modelPlanet = this.graph.CreateEntity(mesh, this.planetsName[planetIdx]);
            modelPlanet.RenderAABB = true;
            SceneNode node = parent.CreateChildSceneNode(this.planetsName[planetIdx]);
            node.AttachObject(modelPlanet);

            Planet plt = new Planet(this.planetsName[planetIdx], modelPlanet, this.planetsOrbit[planetIdx]);
            this.planets.Add(plt);
        }

        #endregion

        #region Properties

        public bool IsLoaded { get; internal set; }

        #endregion
    }
}
