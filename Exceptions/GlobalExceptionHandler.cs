using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OrderNotFoundException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        if (exception is CustomerNotFoundException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        if (exception is ProductNotFoundException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        if (exception is BadRequestException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        if (exception is InsufficientStockException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        if (exception is ConcurrencyConflictException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }
        if (exception is ConflictException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }
        if (exception is AddressNotFoundException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }
        if (exception is BasketItemNotFoundException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(
                new { message = exception.Message },
                cancellationToken);

            return true;
        }

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            new { message = "An unexpected error occurred." },
            cancellationToken);

        return true;
    }
}