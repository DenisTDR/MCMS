using System;

namespace MCMS.Base.Display.ModelDisplay.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DetailsFieldAttribute : Attribute
    {
        public DetailsFieldAttribute(int order)
        {
            Order = order;
        }

        public DetailsFieldAttribute()
        {
        }

        public int Order { get; set; }
        public bool Hidden { get; set; }
    }
}