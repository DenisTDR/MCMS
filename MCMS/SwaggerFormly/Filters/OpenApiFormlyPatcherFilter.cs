using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace MCMS.SwaggerFormly.Filters
{
    public class OpenApiFormlyPatcherSchemaFilter : ISchemaFilter
    {
        private readonly LinkGenerator _linkGenerator;

        public OpenApiFormlyPatcherSchemaFilter(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!ShouldPatchSchema(context.Type))
            {
                return;
            }

            schema.Extensions.Add("x-formly-patched", new OpenApiBoolean(true));
            foreach (var propKvp in schema.Properties)
            {
                var pInfo = context.Type.GetProperties().First(p =>
                    p.Name.Equals(propKvp.Key, StringComparison.InvariantCultureIgnoreCase));
                if (IsFormlyIgnored(pInfo))
                {
                    propKvp.Value.Extensions["x-formlyIgnore"] = new OpenApiBoolean(true);
                    continue;
                }

                var propSchema = propKvp.Value;
                // if (!string.IsNullOrWhiteSpace(propSchema?.Reference?.Id))
                // {
                //     propSchema.AllOf = new List<OpenApiSchema> {new OpenApiSchema {Reference = propSchema.Reference}};
                //     propSchema.Reference = null;
                //     if (typeof(IFormModel).IsAssignableFrom(pInfo.PropertyType))
                //     {
                //         propSchema.Extensions["type"] = Oas("object");
                //     }
                // }

                PatchProperty(propKvp.Key, propSchema, pInfo, context.Type);
            }
        }

        private void PatchProperty(string propertyName, OpenApiSchema schema, PropertyInfo propertyInfo,
            Type declaringType)
        {
            var templateOptions = new OpenApiObject();
            schema.Extensions.Add("x-templateOptions", templateOptions);
            var validators = new List<ValidatorModel>();
            PatchFieldTexts(templateOptions, propertyInfo, declaringType);
            ProcessFieldAttributes(schema, templateOptions, propertyInfo, declaringType, validators);
            PatchDataTypeAttribute(propertyInfo, schema, templateOptions, validators);
            PatchEnumProperties(propertyInfo, templateOptions, schema);
            PatchValidators(propertyInfo, schema, validators);
        }

        private void ProcessFieldAttributes(OpenApiSchema schema,
            OpenApiObject templateOptions, PropertyInfo propertyInfo, Type declaringType, List<ValidatorModel> validators)
        {
            var xProps = new OpenApiObject();
            foreach (var fieldPropertyAttribute in GetAttributes<FormlyFieldPropAttribute>(propertyInfo, declaringType))
            {
                xProps[fieldPropertyAttribute.FullPath] = OpenApiExtensions.ToOpenApi(fieldPropertyAttribute.Value);
            }

            foreach (var formlyFieldAttribute in GetAttributes<FormlyFieldAttribute>(propertyInfo, declaringType))
            {
                formlyFieldAttribute.Attach(schema, xProps, templateOptions, _linkGenerator);
                if (formlyFieldAttribute.HasCustomValidators)
                {
                    validators.AddRange(formlyFieldAttribute.GetCustomValidators());
                }
            }

            if (xProps.Count > 0)
            {
                schema.Extensions["x-props"] = xProps;
            }
        }

        private void PatchFieldTexts(OpenApiObject templateOptions, PropertyInfo propertyInfo, Type declaringType)
        {
            if (!templateOptions.ContainsKey("label"))
            {
                templateOptions["label"] = Oas(EntityHelper.GetPropertyName(propertyInfo) ?? propertyInfo.Name);
            }
        }

        private void PatchDataTypeAttribute(PropertyInfo propertyInfo, OpenApiSchema schema,
            OpenApiObject templateOptions, List<ValidatorModel> validators)
        {
            var dataTypeAttributes = propertyInfo.GetCustomAttributes<DataTypeAttribute>();
            // var vOa = schema.Extensions.GetOrSetDefault<OpenApiObject, IOpenApiExtension>("validators");
            // var validation = vOa.GetOrSetDefault<OpenApiArray, IOpenApiAny>("validation");

            foreach (var dataTypeAttribute in dataTypeAttributes)
            {
                switch (dataTypeAttribute.DataType)
                {
                    case DataType.EmailAddress:
                        validators.Add(new ValidatorModel("email", message: "invalid_email"));
                        break;
                    case DataType.PhoneNumber:
                        validators.Add(
                            new ValidatorModel("pattern", "^\\+?(?:[0-9]\\s*){8,}$", "invalid_phone_number"));
                        break;
                    case DataType.Html:
//                        schema.Extensions["customFormat"] = "textarea";
                        break;
                }
            }
        }


        private void PatchValidators(PropertyInfo propertyInfo, OpenApiSchema schema, List<ValidatorModel> validators)
        {
            foreach (var regExpAttr in propertyInfo.GetCustomAttributes<RegularExpressionAttribute>())
            {
                validators.Add(
                    new ValidatorModel("pattern", regExpAttr.Pattern, regExpAttr.ErrorMessage));
            }

            foreach (var reqAttr in propertyInfo.GetCustomAttributes<RequiredAttribute>())
            {
                validators.Add(
                    new ValidatorModel("required", message: reqAttr.ErrorMessage ?? "field-required"));
            }

            if (validators.Any())
            {
                var xValidators = schema.Extensions.GetOrSetDefault<OpenApiArray, IOpenApiExtension>("x-validators");
                xValidators.AddRange(validators.Select(validatorModel => validatorModel.ToOpenApiObject()));
            }
        }

        private void PatchEnumProperties(PropertyInfo propertyInfo, OpenApiObject templateOptions, OpenApiSchema schema)
        {
            if (!propertyInfo.PropertyType.IsEnum)
                return;

            // var requiredNotDefault = propertyInfo.GetCustomAttributes<RequireNotDefaultAttribute>().Any();
            // var defaultValue = Activator.CreateInstance(propertyInfo.PropertyType);

            // var optionsArray = new OpenApiArray();
            // optionsArray.AddRange(Enum.GetValues(propertyInfo.PropertyType).Cast<Enum>()
            //     .Where(enumValue => !requiredNotDefault || !Equals(enumValue, defaultValue))
            //     .Select(
            //         enumValue => new OpenApiObject
            //         {
            //             ["value"] = OpenApiExtensions.ToOpenApi(enumValue.GetCustomValue()),
            //             ["label"] = Oas(enumValue.GetDisplayName()),
            //             ["description"] = Oas(enumValue.GetDisplayDescription())
            //         }));
            // templateOptions["options"] = optionsArray;
            schema.Extensions["format"] = Oas("select");
        }


        #region helpers

        private bool HasAttribute<T>(PropertyInfo propertyInfo, Type declaringType = null) where T : Attribute
        {
            return GetAttributes<T>(propertyInfo, declaringType).Any();
        }

        private List<T> GetAttributes<T>(PropertyInfo propertyInfo, Type declaringType = null) where T : Attribute
        {
            var list = declaringType?.GetCustomAttributes<T>().ToList() ?? new List<T>();
            if (declaringType != null)
            {
                list.AddRange(propertyInfo.GetCustomAttributes<T>());
            }

            return list;
        }

        private OpenApiString Oas(string str)
        {
            return new OpenApiString(str);
        }

        private bool IsFormlyIgnored(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes().Any(attr => attr is FormlyIgnoreAttribute);
        }

        private bool IsReadOnly(PropertyInfo propertyInfo)
        {
            return false;
            // return propertyInfo.GetCustomAttributes().Any(attr => attr is IsReadOnlyAttribute)
            // && propertyInfo.GetCustomAttributes<IsReadOnlyAttribute>().Any(attr => attr.Is);
        }

        private static bool ShouldPatchSchema(Type type)
        {
            return typeof(IFormModel).IsAssignableFrom(type);
        }

        #endregion
    }
}