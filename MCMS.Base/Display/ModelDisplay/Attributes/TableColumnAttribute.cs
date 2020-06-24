using System;

namespace MCMS.Base.Display.ModelDisplay.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        public TableColumnAttribute(int order)
        {
            Order = order;
        }

        public TableColumnAttribute()
        {
        }

        public int Order { get; set; }
        public bool Hidden { get; set; }
    }
}