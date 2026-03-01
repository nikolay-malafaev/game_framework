using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GameFramework.Loading
{
    public class ShowLoadingViewLoadingOperation : ILoadingOperation
    {
        public async UniTask<LoadingResult> Run()
        {
            var asyncOperation = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            await asyncOperation.ToUniTask();
            // play hide animation
            return LoadingResult.Success("Show loading view");   
        }
    }

    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle ShowLoadingView(this LoadingBundle loadingBundle)
        {
            loadingBundle.AddOperation(new ShowLoadingViewLoadingOperation());
            return loadingBundle;
        }
    }
}