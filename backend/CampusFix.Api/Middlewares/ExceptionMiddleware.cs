using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CampusFix.Api.Middlewares;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var status = ex switch { ArgumentException => 400, KeyNotFoundException => 404,
                InvalidOperationException => 409, DbUpdateException => 409, _ => 500 };
            if (status == 500) logger.LogError(ex, "Error procesando la solicitud");
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails {
                Status = status, Title = status == 500 ? "No se pudo completar la operación. Revise la conexión del servidor." :
                ex is DbUpdateException ? "Conflicto al guardar. Actualice los datos e intente nuevamente." : ex.Message });
        }
    }
}
