using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public interface ILoadingOperation
    {
        float Weight => 1;

        float Progress => 1;
        
        UniTask<LoadingResult> Run();

        public float GetWeight() => 1;
        
        public float GetProgress() => 1;
    }
}