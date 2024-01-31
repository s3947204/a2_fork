using Autofac;
using Data;
using MCBA.Controllers;
using MCBA.Tests.Base;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MCBA.Tests;
public class HomeControllerTests : BackendTest
{
    private readonly MCBAContext _context;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _context = Container.Resolve<MCBAContext>();
        _controller = Container.Resolve<HomeController>();

    }

    [Fact]
    public void Index_ReturnsAView()
    {
        // Act.
        var result =  _controller.Index();

        // Assert.
        Assert.IsType<ViewResult>(result);
    }

}

