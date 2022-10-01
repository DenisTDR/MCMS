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
        public ServerClient Searchable { get; set; } = ServerClient.Both;
        public ServerClient Orderable { get; set; } = ServerClient.Both;
        public bool RowGroup { get; set; }
        public object SumTotal { get; set; }
        public string Tag { get; set; }
        public bool Invisible { get; set; }
        public string DbColumn { get; set; }
        public string DbFuncFormat { get; set; }
        public TableColumnType Type { get; set; }
        public string DataSelector { get; set; }
    }
}