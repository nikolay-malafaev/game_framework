using System.Collections.Generic;

namespace GameFramework.StaticData
{
    public interface IStaticDataService
    {
        TAsset Get<TAsset>() where TAsset : StaticDataAsset;
        IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : StaticDataAsset;
        bool Contains<TAsset>() where TAsset : StaticDataAsset;
        void Add<TAsset>(TAsset asset) where TAsset : StaticDataAsset;
        void Add<TAsset>(IReadOnlyList<TAsset> assets) where TAsset : StaticDataAsset;
    }
}
