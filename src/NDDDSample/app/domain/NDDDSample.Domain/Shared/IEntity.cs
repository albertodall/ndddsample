namespace NDDDSample.Domain.Shared
{
    /// <summary>
    /// An entity, as explained in the DDD book.
    /// </summary>  
    public interface IEntity<out TId>
    {
        TId ID { get; }
        bool IsTransient();
    }
}