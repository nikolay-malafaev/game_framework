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
        private IObjectResolver _objectResolver;
        private SceneFactory _sceneFactory;
        private WindowSettings _windowSettings;
        private string _windowId;

        public WindowViewFactory(IObjectResolver objectResolver, SceneFactory sceneFactory, WindowSettings windowSettings, string windowId)
        {
            _objectResolver = objectResolver;
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
                TView view = (TView) _sceneFactory.Instantiate(_prefab);
                view.Initialize(_objectResolver, _windowId);
                return view;
            }
            return null;
        }
    }
}