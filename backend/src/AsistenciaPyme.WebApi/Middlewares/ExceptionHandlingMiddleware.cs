using System.Net;

namespace AsistenciaPyme.WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException exception)
        {
            await EscribirRespuestaAsync(
                context,
                HttpStatusCode.NotFound,
                exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            await EscribirRespuestaAsync(
                context,
                HttpStatusCode.Unauthorized,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            await EscribirRespuestaAsync(
                context,
                HttpStatusCode.BadRequest,
                exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            await EscribirRespuestaAsync(
                context,
                HttpStatusCode.BadRequest,
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ocurrió un error no controlado.");

            await EscribirRespuestaAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Ocurrió un error interno en el servidor.");
        }
    }

    private static async Task EscribirRespuestaAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string mensaje)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            Codigo = (int)statusCode,
            Mensaje = mensaje
        });
    }
}