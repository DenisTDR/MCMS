using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MCMS.Auth.Identity
{
    internal class IdentityPageModelConvention<TUser> : IPageApplicationModelConvention where TUser : class
    {
        public void Apply(PageApplicationModel model)
        {
            var defaultUIAttribute = model.ModelType.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "IdentityDefaultUIAttribute");
            if (defaultUIAttribute == null)
            {
                return;
            }

            var template =
                defaultUIAttribute.GetType().GetProperty("Template").GetValue(defaultUIAttribute) as Type;

            ValidateTemplate(template);
            var templateInstance = template.MakeGenericType(typeof(TUser));
            model.ModelType = templateInstance.GetTypeInfo();
        }

        private void ValidateTemplate(Type template)
        {
            if (template.IsAbstract || !template.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException("Implementation type can't be abstract or non generic.");
            }

            var genericArguments = template.GetGenericArguments();
            if (genericArguments.Length != 1)
            {
                throw new InvalidOperationException("Implementation type contains wrong generic arity.");
            }
        }
    }
}