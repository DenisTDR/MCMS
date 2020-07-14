using System;
using System.Collections.Generic;

namespace MCMS.Data.Seeder
{
    public sealed class EntitySeeders : List<ISeeder>
    {
        public EntitySeeders Add<T>() where T : ISeeder, new()
        {
            base.Add(new T());
            return this;
        }
    }
}