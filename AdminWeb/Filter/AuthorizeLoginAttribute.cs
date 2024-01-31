
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminWeb.Filters;

//Adapted from Day 6 lab, Project MCBAExampleWithLogin
public class AuthorizeLoginAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var username = context.HttpContext.Session.GetString(nameof(AdminWeb.Models.Login.Username));
        if (username == null)
            context.Result = new RedirectToActionResult("Login", "Admin", null);
    }
}

