using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public class MultithreadingLoadingOperation : MultithreadingLoadingOperationBase
    {
        private ILoadingOperation _loadingOperation;

        public MultithreadingLoadingOperation(ILoadingOperation loadingOperation, int weight) : base(weight)
        {
            _loadingOperation = loadingOperation ?? throw new ArgumentException();
        }
        
        public override float GetWeight()
        {
            return _loadingOperation?.GetWeight() ?? 1;
        }

        public override float GetProgress()
        {
            return _loadingOperation?.GetProgress() ?? 1;
        }
        
        protected override async UniTask<LoadingResult> RunOnThreadPool()
        {
            return await _loadingOperation.Run();
        }
    }
}