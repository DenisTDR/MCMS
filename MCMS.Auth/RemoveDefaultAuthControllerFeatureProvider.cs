using System.Collections.Generic;
using System.Reflection;
using MCMS.Auth.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MCMS.Auth
{
    // https://stackoverflow.com/a/46588830
    public class RemoveDefaultAuthControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // https://stackoverflow.com/a/46588830

            feature.Controllers.Remove(typeof(AuthController).GetTypeInfo());
        }
    }
}