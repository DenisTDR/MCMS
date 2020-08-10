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
using MCMS.Base.SwaggerFormly.Models;
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

            var props = new List<Tuple<string, OpenApiSchema, double>>();

            foreach (var propKvp in schema.Properties)
            {
                var pInfo = context.Type.GetProperties().First(p =>
                    p.Name.Equals(propKvp.Key, StringComparison.InvariantCultureIgnoreCase));
                if (IsFormlyIgnored(pInfo, out var orderIndex))
                {
                    propKvp.Value.Extensions["x-formlyIgnore"] = new OpenApiBoolean(true);
                    continue;
                }

                props.Add(new Tuple<string, OpenApiSchema, double>(propKvp.Key, propKvp.Value, orderIndex));

                var propSchema = propKvp.Value;

                PatchProperty(propKvp.Key, propSchema, pInfo, context.Type);
            }

            schema.Properties = props.OrderBy(p => p.Item3)
                .ToDictionary(t => t.Item1, t => t.Item2);
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
            OpenApiObject templateOptions, PropertyInfo propertyInfo, Type declaringType,
            List<ValidatorModel> validators)
        {
            var xProps = new OpenApiObject();
            foreach (var fieldPropertyAttribute in GetAttributes<FormlyFieldPropAttribute>(propertyInfo, declaringType))
            {
                xProps[fieldPropertyAttribute.FullPath] = OpenApiExtensions.ToOpenApi(fieldPropertyAttribute.Value);
            }


            foreach (var patchAttr in GetAttributes<FormlyConfigPatcherAttribute>(propertyInfo, declaringType))
            {
                patchAttr.Patch(schema, xProps, templateOptions, _linkGenerator, validators);
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
                templateOptions["label"] = Oas(TypeHelpers.GetDisplayName(propertyInfo) ?? propertyInfo.Name);
            }

            if (!templateOptions.ContainsKey("description"))
            {
                var desc = TypeHelpers.GetDescription(propertyInfo);
                if (!string.IsNullOrEmpty(desc))
                {
                    templateOptions["description"] = Oas(desc);
                }
            }
        }

        private void PatchDataTypeAttribute(PropertyInfo propertyInfo, OpenApiSchema schema,
            OpenApiObject templateOptions, List<ValidatorModel> validators)
        {
            var dataTypeAttributes = propertyInfo.GetCustomAttributes<DataTypeAttribute>();

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

            var rangeAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                validators.Add(new ValidatorModel("min", message: rangeAttr.ErrorMessage));
                validators.Add(new ValidatorModel("max", message: rangeAttr.ErrorMessage));
            }

            var minLengthAttr = propertyInfo.GetCustomAttribute<MinLengthAttribute>();
            if (minLengthAttr != null)
            {
                validators.Add(new ValidatorModel("minlength", message: minLengthAttr.ErrorMessage));
            }

            var maxLengthAttr = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null)
            {
                validators.Add(new ValidatorModel("maxlength", message: maxLengthAttr.ErrorMessage));
            }

            var stringLengthAttr = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            if (stringLengthAttr != null)
            {
                validators.Add(new ValidatorModel("minlength", message: stringLengthAttr.ErrorMessage));
                validators.Add(new ValidatorModel("maxlength", message: stringLengthAttr.ErrorMessage));
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

        private bool IsFormlyIgnored(PropertyInfo propertyInfo, out double orderIndex)
        {
            var attr = propertyInfo.GetCustomAttributes().FirstOrDefault(a => a is FormlyFieldAttribute);

            if (attr is FormlyFieldAttribute at)
            {
                orderIndex = at.OrderIndex;
                return at.IgnoreField;
            }

            orderIndex = 0;
            return false;
        }

        private static bool ShouldPatchSchema(Type type)
        {
            return typeof(IFormModel).IsAssignableFrom(type);
        }

        #endregion
    }
}