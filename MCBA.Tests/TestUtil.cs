using MCBA.Tests.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCBA.Tests;
public static class TestUtil
{


    public static List<String> GetModelError(Controller controller)
    {
        var modelErrors = new List<string>();
        foreach (var modelState in controller.ModelState.Values)
        {
            foreach (var modelError in modelState.Errors)
            {
                modelErrors.Add(modelError.ErrorMessage);
            }
        }
        return modelErrors;
    }

    // Adapted from: https://stackoverflow.com/questions/47203333/how-to-mock-session-object-in-asp-net-core
    public static ControllerContext GetControllerContextWithMockSession(Mock<ISession> sessionMock)
    {
        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Session = sessionMock.Object
            }
        };
        return controllerContext;
    }

    //Adapted from: https://stackoverflow.com/questions/47203333/how-to-mock-session-object-in-asp-net-core
    public static Mock<ISession> GetMockSessionWithIntValue(string key, int value, Mock<ISession> sessionMock)
    {
        var val = new byte[]
       {
            (byte)(value >> 24),
            (byte)(0xFF & (value >> 16)),
            (byte)(0xFF & (value >> 8)),
            (byte)(0xFF & value)
       };

        sessionMock.Setup(_ => _.TryGetValue(key, out val)).Returns(true);
        return sessionMock;
    }

    public static Mock<ISession> GetMockSessionWithStringValue(string key, string value, Mock<ISession> sessionMock)
    {
        var val = Encoding.UTF8.GetBytes(value);

        sessionMock.Setup(_ => _.TryGetValue(key, out val)).Returns(true);
        return sessionMock;
    }


    public static Controller AddValidationToModels(Controller controller, object instance)
    {
        var validationContext = new ValidationContext(instance);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(instance, validationContext, validationResults, true);

        // Set ModelState with the validation errors.
        foreach (var validationResult in validationResults)
        {
            controller.ModelState.AddModelError(
                validationResult.MemberNames.Single(), validationResult.ErrorMessage);
        }

        return controller;
    }
}

