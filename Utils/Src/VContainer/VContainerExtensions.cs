using VContainer;
using VContainer.Unity;

public static class VContainerExtensions
{
    public static void Install<TInstaller>(this IContainerBuilder builder)
        where TInstaller : IInstaller, new()
    {
        builder.Install(new TInstaller());
    }

    public static void Install(this IContainerBuilder builder, IInstaller installer)
    {
        installer.Install(builder);
    }
}