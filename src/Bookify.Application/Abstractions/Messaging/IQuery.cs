using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;
/// <summary>
/// Represents a query in CQRS.
/// 
/// Queries:
/// - Do NOT modify system state
/// - Only retrieve data
/// - Always return a Result<TResponse>
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}
