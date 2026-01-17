using Sirenix.Serialization;

namespace GameFramework.StaticData
{
    public class RandomInt : ConfigurableValue<int>
    {
        [OdinSerialize] private int _minValue;
        [OdinSerialize] private int _maxValue;

        public override int Value => UnityEngine.Random.Range(_minValue, _maxValue);
    }
}