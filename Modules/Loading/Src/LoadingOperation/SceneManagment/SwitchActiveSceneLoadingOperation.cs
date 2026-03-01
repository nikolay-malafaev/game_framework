using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GameFramework.Loading
{
    public class SwitchActiveSceneLoadingOperation : ILoadingOperation
    {
        private readonly string _sceneName;

        public SwitchActiveSceneLoadingOperation(string sceneName)
        {
            _sceneName = sceneName;
        }
        
        public async UniTask<LoadingResult> Run()
        {
            var newActiveScene = await LoadSceneAdditive(_sceneName);
            var oldActiveScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(newActiveScene);
            await UnloadScene(oldActiveScene);
            return LoadingResult.Success(string.Format("Loaded scene: {0}", newActiveScene.name));
        }

        private async UniTask<Scene> LoadSceneAdditive(string sceneName)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await asyncOperation.ToUniTask();
            return SceneManager.GetSceneByName(sceneName);
        }

        private UniTask UnloadScene(Scene scene)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(scene.buildIndex);
            return asyncOperation.ToUniTask();
        }
    }
    
    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle SwitchActiveScene(this LoadingBundle loadingBundle, string sceneName)
        {
            loadingBundle.AddOperation(new SwitchActiveSceneLoadingOperation(sceneName));
            return loadingBundle;
        }
    }
}