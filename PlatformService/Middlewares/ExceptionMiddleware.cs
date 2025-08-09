namespace PlatformService.Middlewares
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
      this.next = next;
      this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await next(context);
      }
      // catch (AuthException ex)
      // {
      //   logger.LogError(ex.Message, "Auth Error Occurred");
      //   context.Response.StatusCode = ex.StatusCode;
      //   await context.Response.WriteAsJsonAsync(new
      //   {
      //     Success = false,
      //     Message = ex.Message
      //   });
      // }
      catch (Exception ex)
      {
        logger.LogError(ex, $"Inter Server error while process request {context.Request.Path}");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
          Success = false,
          Error = "An error occurred while processing your request"
        });
      }
    }
  }
}