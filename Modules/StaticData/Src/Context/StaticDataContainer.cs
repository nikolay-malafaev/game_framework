using GameFramework.DI;

namespace GameFramework.StaticData
{
    public class StaticDataContainer : ContainerBase<StaticDataContainer, IStaticDataContext>
    {
        protected override IStaticDataContext CreateContext()
        {
            return new DefaultStaticDataContext();
        }
    }
}