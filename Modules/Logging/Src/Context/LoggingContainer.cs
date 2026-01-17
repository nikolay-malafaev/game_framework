using GameFramework.DI;

namespace GameFramework.Logging
{
    public class LoggingContainer : ContainerBase<LoggingContainer, ILoggingContext>
    {
        protected override ILoggingContext CreateContext()
        {
            return new DefaultLoggingContext();
        }
    }
}