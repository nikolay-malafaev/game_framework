using Cysharp.Threading.Tasks;

namespace GameFramework.Loading
{
    public interface ILoadingService
    {
        UniTask RunBundle(LoadingBundle loadingBundle);
    }
}