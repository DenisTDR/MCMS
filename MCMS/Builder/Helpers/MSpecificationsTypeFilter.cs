using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MCMS.Builder.Helpers
{
    public class MSpecificationsTypeFilter
    {
        public IEnumerable<Type> FilterMapped(MApp app)
        {
            return app.Specifications
                .Select(spec => spec.GetType().Assembly).Distinct()
                .SelectMany(ass => ass.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .Where(type => !type.GetCustomAttributes(typeof(NotMappedAttribute), true).Any());
        }
    }
}