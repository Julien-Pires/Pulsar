using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Components
{
    /// <summary>
    /// Defines a game world
    /// </summary>
    public sealed class World
    {
        #region Fields

        private readonly IndexPool _indexPool = new IndexPool();
        private readonly GameObjectManager _gameObjectManager = new GameObjectManager();
        private readonly ComponentManager _componentManager = new ComponentManager();
        private readonly SystemManager _systemManager = new SystemManager();
        private readonly List<IGameObjectManager> _customsGoManagers = new List<IGameObjectManager>();
        private readonly List<GameObject> _addedGameObjects = new List<GameObject>();
        private readonly List<GameObject> _removedGameObjects = new List<GameObject>();

        #endregion

        #region Methods

        /// <summary>
        /// Updates the world
        /// </summary>
        /// <param name="time">Elapsed time</param>
        public void Update(GameTime time)
        {
            _systemManager.Update(time);
        }

        /// <summary>
        /// Processes changes in the world
        /// </summary>
        public void Process()
        {
            DispatchAdded();
            DispatchRemoved();
        }

        /// <summary>
        /// Dispatches added game object
        /// </summary>
        private void DispatchAdded()
        {
            for (int i = 0; i < _addedGameObjects.Count; i++)
            {
                GameObject gameObject = _addedGameObjects[i];
                if (gameObject.Owner != null)
                    throw new Exception(string.Format("Failed to add, game object {0} already attached to another world",
                        gameObject.Id));

                gameObject.Owner = this;
                gameObject.Index = _indexPool.Get();
                _gameObjectManager.Add(gameObject);

                for (int j = 0; j < gameObject.Count; j++)
                {
                    Component component = gameObject[j];
                    _componentManager.Add(component);
                    _systemManager.ComponentAdded(component);
                }

                for (int j = 0; j < _customsGoManagers.Count; j++)
                    _customsGoManagers[j].Added(gameObject);
            }

            _addedGameObjects.Clear();
        }

        /// <summary>
        /// Dispatches removed game object
        /// </summary>
        private void DispatchRemoved()
        {
            for (int i = 0; i < _removedGameObjects.Count; i++)
            {
                GameObject gameObject = _removedGameObjects[i];
                for (int j = 0; j < _customsGoManagers.Count; j++)
                    _customsGoManagers[j].Removed(gameObject);

                gameObject.Owner = null;
                _gameObjectManager.Remove(gameObject.Id);
                for (int j = 0; j < gameObject.Count; j++)
                {
                    Component component = gameObject[j];
                    _systemManager.ComponentRemoved(component);
                    _componentManager.Remove(component);
                }

                _indexPool.Release(gameObject.Index);
                gameObject.Index = -1;
            }

            _removedGameObjects.Clear();
        }

        /// <summary>
        /// Adds a game object in the world
        /// </summary>
        /// <param name="gameObject">Game object</param>
        public void Add(GameObject gameObject)
        {
            if(gameObject == null)
                throw new ArgumentNullException("gameObject");

            _addedGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the world
        /// </summary>
        /// <param name="gameObject">Game object</param>
        /// <returns>Returns true if removed otherwise false</returns>
        public bool Remove(GameObject gameObject)
        {
            if(gameObject == null)
                throw new ArgumentNullException("gameObject");

            if ((gameObject.Owner == null) || (gameObject.Owner != this))
                return false;

            _removedGameObjects.Add(gameObject);

            return true;
        }

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="component">Component</param>
        internal void AddComponent(Component component)
        {
            _componentManager.Add(component);
            _systemManager.ComponentAdded(component);
        }

        /// <summary>
        /// Removes a component
        /// </summary>
        /// <param name="component">Component</param>
        internal void RemoveComponent(Component component)
        {
            _systemManager.ComponentRemoved(component);
            _componentManager.Remove(component);
        }

        /// <summary>
        /// Adds a custom game object manager
        /// </summary>
        /// <param name="gameObjectManager">Game object manager</param>
        public void AddManager(IGameObjectManager gameObjectManager)
        {
            if(gameObjectManager == null)
                throw new ArgumentNullException("gameObjectManager");

            if(IndexOfManager(gameObjectManager.GetType()) > -1)
                throw new Exception(string.Format("Failed to add, a GameObject Manager with type {0} already exists", gameObjectManager.GetType()));

            _customsGoManagers.Add(gameObjectManager);
        }

        /// <summary>
        /// Removes a custom game object manager
        /// </summary>
        /// <param name="gameObjectManager">Game object manager</param>
        /// <returns>Return true if the game object manager is removed successfully otherwise false</returns>
        public bool RemoveManager(IGameObjectManager gameObjectManager)
        {
            if(gameObjectManager == null)
                throw new ArgumentNullException("gameObjectManager");

            return RemoveManager(gameObjectManager.GetType());
        }

        /// <summary>
        /// Removes a custom game object manager with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the game object manager</typeparam>
        /// <returns>Return true if the game object manager is removed successfully otherwise false</returns>
        public bool RemoveManager<T>() where T : IGameObjectManager
        {
            return RemoveManager(typeof (T));
        }

        /// <summary>
        /// Removes a custom game object manager with a specified type
        /// </summary>
        /// <param name="type">Type of the game object manager</param>
        /// <returns>Return true if the game object manager is removed successfully otherwise false</returns>
        public bool RemoveManager(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            int index = IndexOfManager(type);
            if (index == -1)
                return false;

            _customsGoManagers.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Gets a custom game object manager with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the game object manager</typeparam>
        /// <returns>Returns a game object manager instance if found otherwise default T value</returns>
        public T GetManager<T>() where T : IGameObjectManager
        {
            int index = IndexOfManager(typeof (T));
            if (index == -1)
                return default(T);

            return (T)_customsGoManagers[index];
        }

        /// <summary>
        /// Gets the index of a specific game object manager
        /// </summary>
        /// <param name="type">Type of the game object manager</param>
        /// <returns>Returns the zero-based index of the game object manager if found, otherwise -1</returns>
        private int IndexOfManager(Type type)
        {
            for (int i = 0; i < _customsGoManagers.Count; i++)
            {
                if (_customsGoManagers[i].GetType() == type)
                    return i;
            }

            return -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the game object manager
        /// </summary>
        public GameObjectManager GameObjectManager
        {
            get { return _gameObjectManager; }
        }

        /// <summary>
        /// Gets the component manager
        /// </summary>
        public ComponentManager ComponentManager
        {
            get { return _componentManager; }
        }

        /// <summary>
        /// Gets the system manager
        /// </summary>
        public SystemManager SystemManager
        {
            get { return _systemManager; }
        }

        #endregion
    }
}
