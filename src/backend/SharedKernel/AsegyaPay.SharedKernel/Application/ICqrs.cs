using MediatR;

namespace AsegyaPay.SharedKernel.Application;

/// <summary>
/// Marker interface for CQRS commands that return a result.
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Marker interface for CQRS commands that return no value.
/// </summary>
public interface ICommand : IRequest { }

/// <summary>
/// Marker interface for CQRS queries.
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Handler interface for commands with a response.
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

/// <summary>
/// Handler interface for commands without a response.
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand { }

/// <summary>
/// Handler interface for queries.
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
