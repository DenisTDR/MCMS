using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCMS.Base.Data.Entities
{
    public abstract class Entity : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}