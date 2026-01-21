using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GameFramework.Loading
{
    public class ShowCustomLoadingViewLoadingOperation : ILoadingOperation
    {
        private readonly string _prefabName;

        public ShowCustomLoadingViewLoadingOperation(string prefabName)
        {
            _prefabName = prefabName;
        }

        public async UniTask<LoadingResult> Run()
        {
            var asyncOperation = SceneManager.LoadSceneAsync("loadingScene.buildIndex", LoadSceneMode.Additive);
            await asyncOperation.ToUniTask();
            // play hide animation
            // _prefabName
            return LoadingResult.Success(string.Format("Show custom loading view for prefab \"{0}\"", _prefabName));   
        }
    }
    
    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle ShowCustomLoadingView(this LoadingBundle loadingBundle, string prefabName)
        {
            loadingBundle.AddOperation(new ShowCustomLoadingViewLoadingOperation(prefabName));
            return loadingBundle;
        }
    }
}