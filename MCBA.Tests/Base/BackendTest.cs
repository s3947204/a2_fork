using Autofac;
using MCBA.Tests.Modules;

namespace MCBA.Tests.Base;

// Adapted from week 10 lectorial from project MagicInventory.Tests
public abstract class BackendTest : BaseTest
{
    protected BackendTest()
    {
        Builder.RegisterModule<BackendModule>();
    }
}
