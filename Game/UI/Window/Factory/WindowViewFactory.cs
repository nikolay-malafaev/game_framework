using System;
using Cysharp.Threading.Tasks;
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
        private WindowSettings _windowSettings;
        private string _windowId;

        public WindowViewFactory(IObjectResolver objectResolver, WindowSettings windowSettings, string windowId)
        {
            _objectResolver = objectResolver;
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
                return (TView) _objectResolver.Instantiate(_prefab);
            }
            return null;
        }
    }
}