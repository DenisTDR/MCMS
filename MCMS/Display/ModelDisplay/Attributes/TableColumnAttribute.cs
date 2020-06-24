using System;

namespace MCMS.Display.ModelDisplay.Attributes
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

        public int Order { get; }
    }
}