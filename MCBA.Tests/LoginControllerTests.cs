
using Autofac;
using Data;
using Data.Models;
using MCBA.Controllers;
using MCBA.Tests.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MCBA.Tests;
public class LoginControllerTests : BackendTest
{
    private readonly MCBAContext _context;
    private readonly LoginController _controller;
    private readonly Mock<ISession> _mockSession;

    public LoginControllerTests()
    {

        _context = Container.Resolve<MCBAContext>();
        _controller = Container.Resolve<LoginController>();
        _mockSession = new Mock<ISession>();
    }

    [Fact]
    public void Login_ReturnsAView()
    {
        // Arrange

        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(_mockSession);

        //// Act
        var result = _controller.Login();
            

        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Fact]
    public void Login_Redirect_IfCustomerIDSetInSession()
    {
        // Arrange
        // Adapted from: https://stackoverflow.com/questions/54217245/unit-test-controller-mocking-isession



        var key = nameof(Customer.CustomerID);
        int fy = 2100;

        var sessionWithValues = TestUtil.GetMockSessionWithIntValue(key, fy, _mockSession);

        var controllerContext = TestUtil.GetControllerContextWithMockSession(sessionWithValues);

        _controller.ControllerContext = controllerContext;

        //// Act
        var result = _controller.Login();


        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Theory]
    [InlineData("12345678", "abc123" )]
    [InlineData("38074569", "ilovermit2020" )]
    [InlineData("17963428", "youWill_n0tGuess-This!")]
    public async Task Login_ReturnsARedirect_UponSuccessfullLogin(string loginID, string password)
    {
        // Arrange
 
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(_mockSession);
        // Act
        var result = await _controller.Login(loginID, password);


        // Assert
        Assert.True(_controller.ModelState.IsValid);
        Assert.IsType<RedirectToActionResult>(result);
    }


    [Theory]
    [InlineData("", "")]
    [InlineData("31231", "ajsdklfj")]
    [InlineData("12345678", "abkdls")]
    [InlineData("12345678", "abc124")]
    public async Task Login_ReturnsAView_WhenLoginFails(string loginID, string password)
    {
        // Act
        var result = await _controller.Login(loginID, password);
        // Assert
        Assert.False(_controller.ModelState.IsValid);
        Assert.IsType<ViewResult>(result);
    }



    [Theory]
    [InlineData(2100, "12345678", "abc123")]
    [InlineData(2200, "38074569", "ilovermit2020")]
    [InlineData(2300, "17963428", "youWill_n0tGuess-This!")]
    public async Task Login_ModelError_WhenCustomerLockedOut(int customerID, string loginID,  string password)
    {
        // Arrange
        var customer = await _context.Customer.FindAsync(customerID);
        customer.Locked = true;
        await _context.SaveChangesAsync();

        // Act
        await _controller.Login(loginID, password);
        var modelErrors = TestUtil.GetModelError(_controller);

        // Assert
        Assert.Equal("You have been locked out of your account", modelErrors[0]);
        Assert.Single(modelErrors);
    }

}
