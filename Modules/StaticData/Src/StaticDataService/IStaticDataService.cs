using System.Collections.Generic;
using GameFramework.Types;

namespace GameFramework.StaticData
{
    public interface IStaticDataService
    {
        Optional<TAsset> Get<TAsset>() where TAsset : UniqueStaticDataAsset;
        Optional<TAsset> Get<TAsset>(string id) where TAsset : KeyedStaticDataAsset;
        IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : KeyedStaticDataAsset;
        bool Contains<TAsset>() where TAsset : UniqueStaticDataAsset;
        bool Contains<TAsset>(string id) where TAsset : KeyedStaticDataAsset;
        void Add(UniqueStaticDataAsset asset);
        void Add(IEnumerable<UniqueStaticDataAsset> assets);
        void Add(KeyedStaticDataAsset asset);
        void Add(IEnumerable<KeyedStaticDataAsset> assets);
    }
}
