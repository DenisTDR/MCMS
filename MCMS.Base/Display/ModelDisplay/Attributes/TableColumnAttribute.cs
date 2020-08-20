using System;

namespace MCMS.Base.Display.ModelDisplay.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        public TableColumnAttribute(int orderIndex)
        {
            OrderIndex = orderIndex;
        }

        public TableColumnAttribute()
        {
        }

        public int OrderIndex { get; set; }
        public bool Hidden { get; set; }
        public bool Searchable { get; set; } = true;
        public bool Orderable { get; set; } = true;
        public bool RowGroup { get; set; }
    }
}