using System;

namespace MCMS.Base.Data.Entities
{
    public abstract class Entity : IEntity
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}