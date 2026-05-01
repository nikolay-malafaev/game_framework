using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameFramework.Infrastructure
{
    public class SceneFactory
    {
        [Inject]
        private IObjectResolver _objectResolver;

        // todo hardcode
        [Inject]
        [Key("RuntimeNode")]
        private Transform _runtimeNode;
        
        public T Instantiate<T>(T prefab) where T : Component
        {
            return _objectResolver.Instantiate(prefab, _runtimeNode);
        }
        
        public T Instantiate<T>(T prefab, Transform parent, bool worldPositionStays = false) where T : Component
        {
            return _objectResolver.Instantiate(prefab, parent, worldPositionStays);
        }
        
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return _objectResolver.Instantiate(prefab, position, rotation, _runtimeNode);
        }

        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return _objectResolver.Instantiate(prefab, position, rotation, parent);
        }

        public GameObject Instantiate(GameObject prefab)
        {
            return _objectResolver.Instantiate(prefab, _runtimeNode);
        }
        
        public GameObject Instantiate(GameObject prefab, Transform parent, bool worldPositionStays = false)
        {
            return _objectResolver.Instantiate(prefab, parent, worldPositionStays);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        { 
            return _objectResolver.Instantiate(prefab, position, rotation, _runtimeNode);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _objectResolver.Instantiate(prefab, position, rotation, parent);
        }
    }
}