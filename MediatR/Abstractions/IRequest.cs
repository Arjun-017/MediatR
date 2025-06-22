namespace MediatR.Abstractions;

/// <summary>
/// Event to be sent. Use this if the event is to be handled by a single handler.
/// </summary>
public interface IRequest { }

/// <summary>
/// Event to be sent. Use this if the event is to be handled by a single handler.
/// </summary>
public interface IRequest<in T> { }

/// <summary>
/// Event to be sent. Use this if the event is to be handled by a multiple handlers.
/// </summary>
public interface INotification { }
