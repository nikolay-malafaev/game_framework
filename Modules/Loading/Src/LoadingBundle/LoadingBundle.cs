using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public sealed class LoadingBundle
    {
        private SequenceLoadingOperation _sequenceLoadingOperation = new();

        public LoadingBundle AddOperation(ILoadingOperation loadingOperation)
        {
            _sequenceLoadingOperation.Push(loadingOperation);
            return this;
        }

        public UniTask<LoadingResult> Run()
        {
            return _sequenceLoadingOperation.Run();
        }
    }
}