using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public class WaitLoadingOperation : ILoadingOperation
    {
        private readonly float _durationInSeconds;

        public WaitLoadingOperation(float durationInSeconds)
        {
            _durationInSeconds = durationInSeconds;
        }
        
        public async UniTask<LoadingResult> Run()
        {
            await UniTask.WaitForSeconds(_durationInSeconds);
            return LoadingResult.Success();
        }
    }
    
    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle Wait(this LoadingBundle loadingBundle, float durationInSeconds)
        {
            loadingBundle.AddOperation(new WaitLoadingOperation(durationInSeconds));
            return loadingBundle;
        }
    }
}