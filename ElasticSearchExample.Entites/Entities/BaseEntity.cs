using System;

namespace ElasticSearchExample.Entites.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
    public class BaseEntity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
    public class BaseGuidEntity : BaseEntity<int>
    {

    }
    public class TrackableEntity : BaseGuidEntity
    {
    }
}
