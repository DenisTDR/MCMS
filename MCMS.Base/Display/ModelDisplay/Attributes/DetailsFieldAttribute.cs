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

        public DetailsFieldAttribute(string tag)
        {
            Tag = tag;
        }

        public DetailsFieldAttribute()
        {
        }

        public int Order { get; set; }
        public bool Hidden { get; set; }
        public string Tag { get; set; }
    }
}