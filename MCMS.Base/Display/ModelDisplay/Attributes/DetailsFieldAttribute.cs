using System;

namespace MCMS.Base.Display.ModelDisplay.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DetailsFieldAttribute : Attribute
    {
        public DetailsFieldAttribute(double orderIndex)
        {
            OrderIndex = orderIndex;
        }

        public DetailsFieldAttribute(string tag)
        {
            Tag = tag;
        }

        public DetailsFieldAttribute()
        {
        }

        public double OrderIndex { get; set; }
        public bool Hidden { get; set; }
        public string Tag { get; set; }
        public string ClassName { get; set; }

        public DetailsField ToDetailsField()
        {
            return new DetailsField
            {
                OrderIndex = OrderIndex,
                Tag = Tag,
                ClassName = ClassName
            };
        }
    }
}