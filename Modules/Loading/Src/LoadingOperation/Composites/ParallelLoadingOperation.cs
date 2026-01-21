using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework.Logging;
using UnityEngine;

namespace GameFramework.Loading
{
    [Loggable]
    public sealed partial class ParallelLoadingOperation : ILoadingOperation
    {
        private readonly List<ILoadingOperation> _operations;
        private readonly float _weight;
        private float _maxChildWeight;
        private bool _failOnAnyError;
        private bool _isRunning;

        public ParallelLoadingOperation(int weight = 1, bool failOnAnyError = false, params ILoadingOperation[] operations)
        {
            _weight = weight < 0 ? 0 : weight;
            _failOnAnyError = failOnAnyError;
            _operations = operations.ToList();
            
            foreach (var operation in operations) 
                _maxChildWeight += operation.GetWeight();
        }
        
        public ParallelLoadingOperation Add(ILoadingOperation operation)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (_isRunning) throw new InvalidOperationException("Cannot Push while Run is in progress.");
            _operations.Add(operation);
            _maxChildWeight += operation.GetWeight();
            return this;
        }

        public async UniTask<LoadingResult> Run()
        {
            if(_isRunning) throw new InvalidOperationException("Cannot Push while Run is in progress.");
            _isRunning = true;
            
            int count = _operations.Count;
            if (count == 0) return LoadingResult.Success();
            
            UniTask<LoadingResult>[] tasks = new UniTask<LoadingResult>[count];
            
            for (int i = 0; i < count; i++)
            {
                ILoadingOperation operation = _operations[i];
                tasks[i] = RunSafe(operation);
            }
            
            LoadingResult[] results = await UniTask.WhenAll(tasks);

            _isRunning = false;
            
            bool hasError = false;
            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    LogError($"Operation failed. Message: {result.Message}");
                    hasError = true;
                }
            }
            
            if (_failOnAnyError && hasError)
            {
                return LoadingResult.Error("Parallel loading has error.");
            }
            
            return LoadingResult.Success();
        }

        public float GetWeight() => _weight;

        public float GetProgress()
        {
            if (_maxChildWeight <= 0f)
                return 1f;

            float currentWeight = 0f;
            foreach (ILoadingOperation operation in _operations)
                currentWeight += operation.GetWeight() * operation.GetProgress();

            return currentWeight / _maxChildWeight;
        }
        
        private async UniTask<LoadingResult> RunSafe(ILoadingOperation operation)
        {
            try
            {
                return await operation.Run();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return LoadingResult.Error(exception.Message);
            }
        }
    }
}
