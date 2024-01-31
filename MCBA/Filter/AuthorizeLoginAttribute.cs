using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCBA.Filter;

//Adapted from Day 6 lab, Project MCBAExampleWithLogin
public class AuthorizeLoginAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {

        if (context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute))
            return;

        var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if (!customerID.HasValue)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}

