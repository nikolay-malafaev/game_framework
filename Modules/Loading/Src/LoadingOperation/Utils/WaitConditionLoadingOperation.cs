using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public class WaitConditionLoadingOperation : ILoadingOperation
    {
        private readonly Func<bool> _condition;

        public WaitConditionLoadingOperation(Func<bool> condition)
        {
            _condition = condition;
        }

        public async UniTask<LoadingResult> Run()
        {
            await UniTask.WaitUntil(_condition);
            return LoadingResult.Success();
        }
    }
    
    public static partial class LoadingBundleExtensions
    {
        public static LoadingBundle WaitUntil(this LoadingBundle loadingBundle, Func<bool> condition)
        {
            loadingBundle.AddOperation(new WaitConditionLoadingOperation(condition));
            return loadingBundle;
        }
    }
}