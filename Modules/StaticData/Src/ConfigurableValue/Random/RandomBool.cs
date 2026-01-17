namespace GameFramework.StaticData
{
    public class RandomBool : ConfigurableValue<bool>
    {
        public override bool Value => UnityEngine.Random.value > 0.5f;
    }
}