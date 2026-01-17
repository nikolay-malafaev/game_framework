using GameFramework.DI;

namespace GameFramework.PersistentData
{
    public class PersistentDataContainer : ContainerBase<PersistentDataContainer, IPersistentDataContext>
    {
        protected override IPersistentDataContext CreateContext()
        {
            return new DefaultPersistentDataContext();
        }
    }
}