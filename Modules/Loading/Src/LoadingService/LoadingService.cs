using System;
using Cysharp.Threading.Tasks;
using GameFramework.Logging;

namespace GameFramework.Loading
{
    [Loggable]
    public sealed partial class LoadingService : ILoadingService
    {
        public UniTask RunBundle(LoadingBundle loadingBundle)
        {
            return RunImpl(loadingBundle.Run);
        }
        
        public UniTask RunOperation(ILoadingOperation loadingOperation)
        {
            return RunImpl(loadingOperation.Run);
        }

        private async UniTask RunImpl(Func<UniTask<LoadingResult>> loadingFunc)
        {
            LogInfo("Start loading.");
            LoadingResult loadingResult = await loadingFunc();
            if (loadingResult.IsSuccess)
            {
                LogInfo($"Success finish loading. {loadingResult.Message}");
            }
            else
            {
                LogError($"Error loading. {loadingResult.Message}");
            }
        }
    }
}