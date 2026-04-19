using System;
using Cysharp.Threading.Tasks;
using GameFramework.Infrastructure;
using GameFramework.Verification;
using VContainer;
using VContainer.Unity;

namespace GameFramework.UI.Window
{
    [Verifiable]
    public partial class WindowViewFactory
    {
        private WindowViewBehaviour _prefab;
        private SceneFactory _sceneFactory;
        private WindowSettings _windowSettings;
        private string _windowId;

        public WindowViewFactory(SceneFactory sceneFactory, WindowSettings windowSettings, string windowId)
        {
            _sceneFactory = sceneFactory;
            _windowSettings = windowSettings;
            _windowId = windowId;
        }

        public async UniTask Initialize()
        { 
            var gameObject = await _windowSettings.ViewPrefab.LoadAssetAsync();
            _prefab = gameObject.GetComponent<WindowViewBehaviour>();
        }
        
        public TView Create<TView>() where TView : WindowViewBehaviour
        {
            if (Verify(_prefab, "WindowViewBehaviour not found in root node!"))
            {
                return (TView) _sceneFactory.Instantiate(_prefab);
            }
            return null;
        }
    }
}