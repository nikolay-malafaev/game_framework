using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameFramework.Infrastructure
{
    public class SceneFactory
    {
        [Inject]
        private IObjectResolver _objectResolver;

        public T Instantiate<T>(T prefab, string parentPath = null) where T : Component
        {
            return _objectResolver.Instantiate(prefab, GetOrCreateParent(parentPath));
        }
        
        public T Instantiate<T>(T prefab, Transform parent, bool worldPositionStays = false) where T : Component
        {
            return _objectResolver.Instantiate(prefab, parent, worldPositionStays);
        }
        
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, string parentPath = null) where T : Component
        {
            return _objectResolver.Instantiate(prefab, position, rotation, GetOrCreateParent(parentPath));
        }

        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return _objectResolver.Instantiate(prefab, position, rotation, parent);
        }

        public GameObject Instantiate(GameObject prefab, string parentPath = null)
        {
            return _objectResolver.Instantiate(prefab, GetOrCreateParent(parentPath));
        }
        
        public GameObject Instantiate(GameObject prefab, Transform parent, bool worldPositionStays = false)
        {
            return _objectResolver.Instantiate(prefab, parent, worldPositionStays);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, string parentPath = null)
        { 
            return _objectResolver.Instantiate(prefab, position, rotation, GetOrCreateParent(parentPath));
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _objectResolver.Instantiate(prefab, position, rotation, parent);
        }

        private Transform GetOrCreateParent(string parentPath)
        {
            if (string.IsNullOrEmpty(parentPath))
            {
                parentPath = "RUNTIME";
            }

            string[] parts = parentPath.Split('/');
            Transform currentParent = null;

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) continue;
                
                string name = $"[{part.ToUpper()}]";
                
                Transform nextParent = null;
                if (currentParent == null)
                {
                    var go = GameObject.Find(name);
                    if (go != null)
                    {
                        nextParent = go.transform;
                    }
                }
                else
                {
                    nextParent = currentParent.Find(name);
                }

                if (nextParent == null)
                {
                    nextParent = new GameObject(name).transform;
                    if (currentParent != null)
                    {
                        nextParent.SetParent(currentParent);
                    }
                }

                currentParent = nextParent;
            }

            return currentParent;
        }
    }
}