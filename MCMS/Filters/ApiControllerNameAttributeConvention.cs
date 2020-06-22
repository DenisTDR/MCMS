using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MCMS.Filters
{
    public class ApiControllerNameAttributeConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsSubclassOf(typeof(ApiController)))
            {
                var typeName = controller.ControllerType.Name;
                
                var toReplace = typeName.Contains("ApiController") ? "ApiController" : "Controller";

                controller.ControllerName = typeName.Replace(toReplace, "");
            }
        }
    }
}