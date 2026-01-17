using System;
using Sirenix.OdinInspector;

namespace GameFramework.StaticData
{
    [Serializable, LabelWidth(250)]
    public abstract class ConfigurableValue<T> : IConfigurableValue
    {
        public abstract T Value { get; }
    }
}