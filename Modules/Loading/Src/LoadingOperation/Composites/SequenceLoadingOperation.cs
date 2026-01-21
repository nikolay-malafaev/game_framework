using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameFramework.Loading
{
    public sealed class SequenceLoadingOperation : ILoadingOperation
    {
        private readonly float _weight;
        private readonly Queue<ILoadingOperation> _operations;

        private float _maxChildWeight;
        private float _completedChildWeight;
        private ILoadingOperation _currentOperation;

        private bool _isRunning;
        private bool _breakWhenError;

        public SequenceLoadingOperation(int weight = 1, bool breakWhenError = false, params ILoadingOperation[] operations)
        {
            _weight = weight;
            _breakWhenError = breakWhenError;
            _operations = new Queue<ILoadingOperation>(operations ?? Array.Empty<ILoadingOperation>());
        }

        public SequenceLoadingOperation Push(ILoadingOperation operation)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (_isRunning) throw new InvalidOperationException("Cannot Push while Run is in progress.");
            _operations.Enqueue(operation);
            return this;
        }

        public async UniTask<LoadingResult> Run()
        {
            if(_isRunning) throw new InvalidOperationException("Cannot Push while Run is in progress.");
            _isRunning = true;
            
            _currentOperation = null;
            _completedChildWeight = 0f;

            _maxChildWeight = _operations.Sum(op => op?.GetWeight() ?? 0f);
            
            while (_operations.Count > 0)
            {
                ILoadingOperation operation = _operations.Dequeue();
                _currentOperation = operation;

                try
                {
                    LoadingResult result = await operation.Run();
                    if (!result.IsSuccess && _breakWhenError)
                    {
                        return LoadingResult.Error("Sequence loading has error. " + result.Message);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    if (_breakWhenError)
                    {
                        return LoadingResult.Error(exception.Message);
                    }
                }
                
                _completedChildWeight += operation.GetWeight();
            }
            
            _currentOperation = null;
            _isRunning = false;
            
            return LoadingResult.Success();
        }

        public float GetWeight() => _weight;

        public float GetProgress()
        {
            if (_maxChildWeight <= 0f)
            {
                return _currentOperation != null ? _currentOperation.GetProgress() : 1f;
            }

            float value = _completedChildWeight;

            if (_currentOperation != null)
            {
                value += _currentOperation.GetWeight() * _currentOperation.GetProgress();
            }

            return Math.Clamp(value / _maxChildWeight, 0f, 1f);
        }
    }
}
