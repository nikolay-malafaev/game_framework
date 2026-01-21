using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameFramework.Loading
{
    public abstract class MultithreadingLoadingOperationBase : ILoadingOperation
    {
        private int _weight;

        public MultithreadingLoadingOperationBase(int weight = 1)
        {
            _weight = weight;
        }

        public async UniTask<LoadingResult> Run()
        {
            try
            {
                return await UniTask.RunOnThreadPool(RunOnThreadPool);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return LoadingResult.Error(exception.Message);
            }
        }
        
        public virtual float GetWeight() => _weight;
        
        public virtual float GetProgress() => 1;

        protected abstract UniTask<LoadingResult> RunOnThreadPool();
    }
}