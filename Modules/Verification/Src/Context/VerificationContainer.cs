using GameFramework.DI;

namespace GameFramework.Verification
{
    public class VerificationContainer : ContainerBase<VerificationContainer, IVerificationContext>
    {
        protected override IVerificationContext CreateContext()
        {
#if BUILD_PRODUCTION
            return new ProductionVerificationContext();
#endif
            return new DebugVerificationContext();
        }
    }
}