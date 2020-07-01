using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Controllers.Api;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MCMS.Builder.Routes
{
    public class MRouteConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _adminUiRoutePrefix;
        private readonly AttributeRouteModel _adminUiApiRoutePrefix;

        public MRouteConvention(
            string routePrefix
            //IRouteTemplateProvider adminUiRouteTemplateProvider
        )
        {
            _adminUiRoutePrefix = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(routePrefix));
            _adminUiApiRoutePrefix = AttributeRouteModel.CombineAttributeRouteModel(_adminUiRoutePrefix,
                new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute("api")));
        }

        public void Apply(ApplicationModel application)
        {
            ApplyRoutePrefix(_adminUiRoutePrefix, application.Controllers.Where(c =>
                typeof(AdminUiController).IsAssignableFrom(c.ControllerType)).ToList());
            ApplyRoutePrefix(_adminUiApiRoutePrefix, application.Controllers.Where(c =>
                typeof(AdminApiController).IsAssignableFrom(c.ControllerType)).ToList());
        }

        private void ApplyRoutePrefix(AttributeRouteModel routePrefix, List<ControllerModel> controllers)
        {
            foreach (var controller in controllers)
            {
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        var initial = selectorModel.AttributeRouteModel.Template;
                        var forcedPrefix = routePrefix.Template;
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            routePrefix,
                            selectorModel.AttributeRouteModel);
                        var final = selectorModel.AttributeRouteModel.Template;
                        Console.WriteLine(controller.ControllerType.Name + " > " +
                                          $"({forcedPrefix} + {initial} -> {final})");
                    }
                }

                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        Console.WriteLine("aici?");
                        // selectorModel.AttributeRouteModel = _centralPrefix;
                    }
                }
            }
        }
    }
}