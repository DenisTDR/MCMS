using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCMS.Base.Data.Entities
{
    public abstract class Entity : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual string Id { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual DateTime Updated { get; set; }
    }
}