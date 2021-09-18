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
        public Guid CreaterId { get; set; }
        public Guid? LastUpdaterId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
