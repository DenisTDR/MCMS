using System.Collections.Generic;

namespace MCMS.Base.Builder
{
    public interface IMApp
    {
        public IEnumerable<MSpecifications> Specifications { get; }
    }
}