using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToDoAPI.Middleware.Filters;

/// <summary>
///     Custom action filter that validates the model state before an action method is executed.
///     If the model state is invalid, it short-circuits the request pipeline by returning a 
///     <see cref="BadRequestObjectResult"/> with the model state errors.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    ///     Called before the action method is executed. Checks if the model state is valid.
    ///     If not, it sets the result to a <see cref="BadRequestObjectResult"/>, which prevents the action from executing.
    /// </summary>
    /// <param name="context">The context for the action being executed, providing access to the model state.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
