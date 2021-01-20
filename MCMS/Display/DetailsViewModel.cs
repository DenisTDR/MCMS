using System.Collections.Generic;
using MCMS.Base.Display.ModelDisplay;

namespace MCMS.Display
{
    public class DetailsViewModelT<T> : DetailsViewModel where T : class
    {
        public DetailsViewModelT()
        {
            
        }

        public DetailsViewModelT(T model, List<DetailsField> fields)
        {
            Model = model;
            Fields = fields;
        }

        public T GetModel() => TModel;

        public T TModel => Model as T;
    }

    public class DetailsViewModel
    {
        public object Model { get; set; }
        public List<DetailsField> Fields { get; set; }
    }
}