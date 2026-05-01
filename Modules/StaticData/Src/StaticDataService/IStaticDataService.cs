using System.Collections.Generic;

namespace GameFramework.StaticData
{
    public interface IStaticDataService
    {
        TAsset Get<TAsset>() where TAsset : UniqueStaticDataAsset;
        TAsset Get<TAsset>(string id) where TAsset : KeyedStaticDataAsset;
        IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : KeyedStaticDataAsset;
        bool Contains<TAsset>() where TAsset : UniqueStaticDataAsset;
        bool Contains<TAsset>(string id) where TAsset : KeyedStaticDataAsset;
        void Add(UniqueStaticDataAsset asset);
        void Add(IEnumerable<UniqueStaticDataAsset> assets);
        void Add(KeyedStaticDataAsset asset);
        void Add(IEnumerable<KeyedStaticDataAsset> assets);
    }
}
