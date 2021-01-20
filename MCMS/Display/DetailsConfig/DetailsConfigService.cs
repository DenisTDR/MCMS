using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Display.DetailsConfig
{
    public class DetailsConfigService<TVm> : IDetailsConfigServiceT<TVm> where TVm : class, IViewModel
    {
        public virtual Type ViewModelType => typeof(TVm);

        public virtual List<DetailsField> GetDetailsFields(Type viewModelType = null)
        {
            var props = (viewModelType ?? ViewModelType).GetProperties().ToList();
            var detailsFields = props.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault();
                return attr != null && !attr.Hidden;
            }).ToList();
            if (detailsFields.Count == 0)
            {
                detailsFields = props;
            }

            detailsFields = detailsFields.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault();
                return attr == null || !attr.Hidden;
            }).ToList();

            var list = detailsFields
                .Select(prop =>
                {
                    var field = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault() is { } attr
                        ? attr.ToDetailsField()
                        : new DetailsField();
                    field.PropertyInfo = prop;
                    return field;
                }).OrderBy(df => df.OrderIndex).ToList();

            return list;
        }

        public virtual DetailsViewModelT<TVm> Wrap(TVm model)
        {
            return new(model, GetDetailsFields());
        }
    }
}