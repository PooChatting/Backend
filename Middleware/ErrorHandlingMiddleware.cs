using Microsoft.AspNetCore.Http;
using Poochatting.Exceptions;

namespace Poochatting.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (UnauthorizedException exception)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(exception.Message);
            }
            catch (BadRequestException exception)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(exception.Message);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went REALLY wrong");
            }
        }
    }
}
