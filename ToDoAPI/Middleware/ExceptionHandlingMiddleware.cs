using System.Net;

namespace ToDoAPI.Middleware;

/// <summary>
///     Middleware for handling exceptions globally in the application, intercepting unhandled exceptions 
///     and formatting the response with an appropriate HTTP status code and error message.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    ///     Invokes the middleware, catching any unhandled exceptions and passing them to the 
    ///     <see cref="HandleExceptionAsync"/> method for handling.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    ///     Processes an exception by setting the HTTP response status code and formatting the response message 
    ///     based on the type of exception encountered.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>A task that completes when the response has been written.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message;

        // Determine the appropriate status code and message based on the exception type
        switch (exception)
        {
            case KeyNotFoundException:
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case ArgumentException:
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        // Set the response status code and write the error message as JSON
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}
