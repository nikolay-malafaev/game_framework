using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GameFramework.Loading
{
    public class HideLoadingViewLoadingOperation : ILoadingOperation
    {
        public async UniTask<LoadingResult> Run()
        {
            // play hide animation
            var loadingScene = SceneManager.GetActiveScene();
            var asyncOperation = SceneManager.UnloadSceneAsync(loadingScene.buildIndex);
            await asyncOperation.ToUniTask();
            return LoadingResult.Success("Hide loading view");   
        }
    }
    
    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle HideLoadingView(this LoadingBundle loadingBundle)
        {
            loadingBundle.AddOperation(new HideLoadingViewLoadingOperation());
            return loadingBundle;
        }
    }
}