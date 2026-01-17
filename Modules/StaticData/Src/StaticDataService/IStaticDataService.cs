using System;
using System.Collections.Generic;

namespace GameFramework.StaticData
{
    public interface IStaticDataService : IDisposable
    {
        TAsset Get<TAsset>() where TAsset : StaticDataAsset;

        IReadOnlyList<TAsset> GetAll<TAsset>() where TAsset : StaticDataAsset;

        bool Contains<TAsset>() where TAsset : StaticDataAsset;
    }
}
