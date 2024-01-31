using Autofac;

namespace MCBA.Tests.Base;

// Adapted from week 10 lectorial from project MagicInventory.Tests
public abstract class BaseTest : IDisposable
{
    private static readonly object _containerLock = new();

    private IContainer _container;
    protected ContainerBuilder Builder { get; }

    protected BaseTest()
    {
        Builder = new ContainerBuilder();
    }

    protected IContainer Container
    {
        get
        {
            // If the container is null it is built.
            if (_container == null)
                BuildContainer();

            return _container;
        }
    }

    private void BuildContainer()
    {
        // Perform locking to ensure the container is only built once.
        // NOTE: The container is only built if it is null.
        lock (_containerLock)
        {
            _container ??= Builder.Build();
        }
    }

    public virtual void Dispose()
    {
        Container.Dispose();
        GC.SuppressFinalize(this);
    }
}
