using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Behaviors;
/// <summary>
/// Pipeline behavior that logs command execution lifecycle.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>
    (ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request.GetType().Name; // Get command name for logging

        try
        {
            // Log before command execution
            logger.LogInformation("Executing command {Command}", name);

            // Continue to the next behavior or the actual handler
            var result = await next();

            // Log after successful execution
            logger.LogInformation("Command {Command} processed successfully", name);

            return result;
        }

        catch (Exception ex)
        {
            // Log error if execution fails
            logger.LogError(ex, "Command {Command} processing failed", name);

            throw; // Rethrow exception to preserve pipeline behavior
        }
    }
}
