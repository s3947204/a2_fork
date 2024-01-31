using Autofac;
using Castle.Core.Resource;
using Data;
using Data.Models;
using MCBA.Controllers;
using MCBA.Tests.Base;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MCBA.Tests;
public class WithdrawControllerTests : BackendTest
{

    private readonly MCBAContext _context;
    private WithdrawController _controller;
    private readonly Mock<ISession> _mockSession;


    public WithdrawControllerTests()
    {
        _context = Container.Resolve<MCBAContext>();
        _controller = Container.Resolve<WithdrawController>();
        _mockSession = new Mock<ISession>();

    }

    [Fact]
    public async Task Withdraw_ReturnsView()
    {
        // Arrange
        // Mocking logged in user by setting a customer
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), 2100, _mockSession);
        var context = TestUtil.GetControllerContextWithMockSession(session);
        _controller.ControllerContext = context;

        // Act
        var result = await _controller.Withdraw();
        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Theory]
    [InlineData(2100, 4100, 23.23, "Valid withdrawal")]
    [InlineData(2200, 4200, 10.01, "Valid withdrawal")]
    [InlineData(2300, 4300, 99, "Valid withdrawal")]
    public async Task Withdraw_ReturnsRedirect_ValidWithdraw(int customerID, int accountNumber, decimal amount, string comment)
    {
        // Arrange
        // Mocking logged in user by setting a customer
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        var context = TestUtil.GetControllerContextWithMockSession(session);

        var withdrawViewModel = new WithdrawViewModel()
        {
            ChosenAccount = accountNumber,
            Amount = amount,
            Comment = comment
        };

        _controller.ControllerContext = context;

        // Act
        var result = await _controller.Withdraw(withdrawViewModel);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Theory]
    [InlineData(2100, 4100, 23.0001, "Invalid", 1)] // 1 model error, invalid amount
    [InlineData(2100, 4200, 0.0099, "Invalid", 3)] // 2 model errors, account does not belong to customer, invalid amount(less than once cent and more than two decimal places hence two)
    [InlineData(2300, 4300, 23, "The comment is far too long and should yield error", 1)]
    [InlineData(2200, 4200, 500.96, "Invalid", 1)] // Insufficient funds, savings account with balance 500.95
    [InlineData(2100, 4101, 600.01, "Invalid", 1)] // Insufficient funds, checking account with balance 600
    [InlineData(2100, 4300, 10.013, "The comment is far too long and should yield error", 3)] // account does not belong to custoemr, invalid amount and long comment
    public async Task Withdraw_ModelErrors_WhenInvalid(int customerID, int accountNumber, decimal amount, string comment, int numberOfErrors)
    {
        // Arrange
        // Mocking logged in user by setting a customer
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        var context = TestUtil.GetControllerContextWithMockSession(session);

        var withdrawViewModel = new WithdrawViewModel()
        {
            ChosenAccount = accountNumber,
            Amount = amount,
            Comment = comment
        };

        _controller.ControllerContext = context;
        _controller = (WithdrawController)TestUtil.AddValidationToModels(_controller, withdrawViewModel);

        // Act
        var result = await _controller.Withdraw(withdrawViewModel);
        var error = TestUtil.GetModelError(_controller);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal(numberOfErrors, error.Count());
    }

    [Theory]
    [InlineData(2100, 4100, 30.12, "Valid withdraw")]
    [InlineData(2200, 4101, 10.12, "Valid withdraw")]
    [InlineData(2300, 4300, 99.23, "Valid withdraw")]
    public async Task WithdrawConfirmPost_AddsTransactionToDb(int customerID, int accountNumber, decimal amount, string comment)
    {
        //  Arrange
        var sessionWithCustomerID = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        var sessionWithChosenAccount = TestUtil.GetMockSessionWithStringValue($"{nameof(WithdrawController)}_ChosenAccount", accountNumber.ToString(), sessionWithCustomerID);
        var sessionWithAmount = TestUtil.GetMockSessionWithStringValue($"{nameof(WithdrawController)}_Amount", amount.ToString(), sessionWithChosenAccount);
        var sessionWithComment = TestUtil.GetMockSessionWithStringValue($"{nameof(WithdrawController)}_Comment", comment.ToString(), sessionWithAmount);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(sessionWithComment);
        var transactionCountBefore = _context.Transaction.Count();

        // Act
        var result = await _controller.WithdrawConfirmPost();
        var transactionCountAfter = _context.Transaction.Count();
        var transaction = _context.Transaction
                       .OrderByDescending(p => p.TransactionTimeUtc)
                       .FirstOrDefault();
        // Assert
        Assert.Equal(transactionCountAfter, transactionCountBefore + 1);
        Assert.Equal(accountNumber, transaction.AccountNumber);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal("W", transaction.TransactionType);
        Assert.Equal(comment, transaction.Comment);
    }

    

}




