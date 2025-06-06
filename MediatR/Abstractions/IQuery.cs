namespace MediatR.Abstractions
{
    /// <summary>
    /// Event to be sent. Use this if the event is to be handled by a single handler.
    /// </summary>
    public interface IQuery { }

    /// <summary>
    /// Event to be sent. Use this if the event is to be handled by a single handler.
    /// </summary>
    public interface IQuery<out T> { }

    /// <summary>
    /// Event to be sent. Use this if the event is to be handled by a multiple handlers.
    /// </summary>
    public interface INotification { }

    /// <summary>
    /// Event to be sent. Use this if the event is to be handled by a multiple handlers.
    /// </summary>
    public interface INotification<out T> { }

}
