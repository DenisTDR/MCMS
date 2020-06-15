using System;

namespace MCMS.Base.Data.Entities
{
    public interface IEntity
    {
        string Id { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}