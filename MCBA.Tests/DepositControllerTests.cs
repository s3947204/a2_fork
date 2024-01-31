using Autofac;
using Castle.Core.Resource;
using Data;
using Data.Models;
using MCBA.Controllers;
using MCBA.Tests.Base;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MCBA.Tests;
public class DepositControllerTests : BackendTest
{
    private readonly MCBAContext _context;
    private DepositController _controller;
    private readonly Mock<ISession> _mockSession;


    public DepositControllerTests()
    {
        _context = Container.Resolve<MCBAContext>();
        _controller = Container.Resolve<DepositController>();
        _mockSession = new Mock<ISession>();

    }


    [Fact]
    public async Task Deposit_ReturnsView()
    {
        // Arrange
        // Mocking logged in user by setting a customer
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), 2100, _mockSession);
        var context = TestUtil.GetControllerContextWithMockSession(session);
        _controller.ControllerContext = context;

        // Act
        var result = await _controller.Deposit();
        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Theory]
    [InlineData(2100, 4100, 32.10, "Valid deposit")]
    [InlineData(2300, 4300, 10.11, "Valid deposit")]
    [InlineData(2100, 4101, 10.11, "Valid deposit")]
    public async Task Deposit_RedirectToConfirm_WhenValidDataGiven(int customerID, int accountNumber, decimal amount, string comment)
    {
        // Arrange
        var depositViewModel = new DepositViewModel()
        {
            ChosenAccount = accountNumber,
            Amount = amount,
            Comment = comment
        };
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(session);

        // Act
        var result = await _controller.Deposit(depositViewModel);


        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Theory]
    [InlineData(2100, 4100, 32.103, "Invalid deposit")]
    [InlineData(2300, 4300, 0.0, "Invalid deposit")]
    [InlineData(2100, 4101, -12, "Invalid deposit")]
    public async Task Deposit_ModelError_WhenInvalidAmount(int customerID, int accountNumber, decimal amount, string comment)
    {
        // Arrange
        var depositViewModel = new DepositViewModel()
        {
            ChosenAccount = accountNumber,
            Amount = amount,
            Comment = comment
        };


        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(session);
        _controller = (DepositController)TestUtil.AddValidationToModels(_controller, depositViewModel);


        // Act
        var result = await _controller.Deposit(depositViewModel);
        var modelErrors = TestUtil.GetModelError(_controller);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Single(modelErrors);
    }


    [Theory]
    [InlineData(2100, 410032, 32.10, "Invalid deposit")]
    [InlineData(2100, null, 32.10, "Invaild deposit")]
    // Account does not belong to customer
    [InlineData(2300, 4101, 10.11, "Invalid deposit")]
    [InlineData(2200, 4300, 10.11, "Invalid deposit")]
    public async Task Deposit_ModelError_WhenInvalidAccount(int customerID, int? accountNumber, decimal amount, string comment)
    {
        // Arrange
        var depositViewModel = new DepositViewModel()
        {
            Amount = amount,
            Comment = comment
        };

        if(accountNumber != null)
            depositViewModel.ChosenAccount = (int)accountNumber;


        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(session);
        _controller = (DepositController)TestUtil.AddValidationToModels(_controller, depositViewModel);

        // Act
        var result = await _controller.Deposit(depositViewModel);
        var modelErrors = TestUtil.GetModelError(_controller);


        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Single(modelErrors);
    }


    [Theory]
    [InlineData(2100, 4100, 32.10, "The quick brown fox jumped over the lazy dog")]
    // 31 charactesr
    [InlineData(2100, 4100, 32.10, "the quick brown fox jumped over")]
    public async Task Deposit_ModelError_WhenInvalidComment(int customerID, int accountNumber, decimal amount, string comment)
    {
        // Arrange
        var depositViewModel = new DepositViewModel()
        {
            ChosenAccount = accountNumber,
            Amount = amount,
            Comment = comment
        };
        var session = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(session);
        _controller = (DepositController)TestUtil.AddValidationToModels(_controller, depositViewModel);

        // Act
        var result = await _controller.Deposit(depositViewModel);
        var modelErrors = TestUtil.GetModelError(_controller);



        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Single(modelErrors);
    }


    [Fact]
    public void DepositConfirm_ReturnsRedirect_IfUserHasnotGoneThroughDeposit()
    {
        // Arrange
        var sessionWithCustomerID = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), 2100, _mockSession);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(sessionWithCustomerID);

        // Act
        var result =  _controller.DepositConfirm();

        // Assert
        Assert.IsType<RedirectToActionResult>(result);

    }

    [Fact]
    public void DepositConfirm_ReturnsView_IfUserHasGoneThroughDeposit()
    {
        // Arrange
        var sessionWithCustomerID = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), 2100, _mockSession);
        // Mocking session to contain a value with key DepositController_ChosenAccount to mock that the user has gone through deposit
        var sessionWithChosenAccount = TestUtil.GetMockSessionWithStringValue($"{nameof(DepositController)}_ChosenAccount", "4100", sessionWithCustomerID);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(sessionWithChosenAccount);

        // Act
        var result = _controller.DepositConfirm();

        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Theory]
    [InlineData(2100, 4100, 30.12, "Valid deposit")]
    [InlineData(2100, 4101, 10.12, "Valid deposit")]
    [InlineData(2300, 4300, 99.23, "Valid deposit")]
    public async Task DepsoitConfirmPost_AddsTransactionToDb(int customerID, int accountNumber, decimal amount, string comment)
    {
        //  Arrange
        var sessionWithCustomerID = TestUtil.GetMockSessionWithIntValue(nameof(Customer.CustomerID), customerID, _mockSession);
        var sessionWithChosenAccount = TestUtil.GetMockSessionWithStringValue($"{nameof(DepositController)}_ChosenAccount", accountNumber.ToString(), sessionWithCustomerID);
        var sessionWithAmount = TestUtil.GetMockSessionWithStringValue($"{nameof(DepositController)}_Amount", amount.ToString(), sessionWithChosenAccount);
        var sessionWithComment = TestUtil.GetMockSessionWithStringValue($"{nameof(DepositController)}_Comment", comment.ToString(), sessionWithAmount);
        _controller.ControllerContext = TestUtil.GetControllerContextWithMockSession(sessionWithComment);
        var transactionCountBefore = _context.Transaction.Count();

        // Act
        var result = await _controller.DepositConfirmPost();
        var transactionCountAfter = _context.Transaction.Count();
        var transaction = _context.Transaction
                       .OrderByDescending(p => p.TransactionTimeUtc)
                       .FirstOrDefault();
        // Assert
        Assert.Equal(transactionCountAfter, transactionCountBefore + 1);
        Assert.Equal(accountNumber, transaction.AccountNumber);
        Assert.Equal("D", transaction.TransactionType);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(comment, transaction.Comment);
    }

}

