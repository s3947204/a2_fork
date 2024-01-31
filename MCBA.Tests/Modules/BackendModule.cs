using Autofac;
using Data;
using MCBA.Controllers;
using MCBA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MCBA.Tests.Modules;

// Adapted from week 10 lectorial from project MagicInventory.Tests
public class BackendModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        var context = new MCBAContext(new DbContextOptionsBuilder<MCBAContext>().
            UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
        context.Database.EnsureCreated();
        SeedData.Initialize(context);

        // Substitute all ILogger types.
        builder.RegisterInstance(new LoggerFactory()).As<ILoggerFactory>();
        builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));

        builder.RegisterInstance(context);
        builder.RegisterType<HomeController>();
        builder.RegisterType<LoginController>();
        builder.RegisterType<DepositController>();
        builder.RegisterType<WithdrawController>();
        builder.RegisterType<AccountServices>();
    }



}
