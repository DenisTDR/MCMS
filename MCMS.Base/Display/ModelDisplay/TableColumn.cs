using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Base.Display.ModelDisplay
{
    public class TableColumn
    {
        public int OrderIndex { get; set; }

        public TableColumn(string name, string key, int orderIndex) : this(name, key)
        {
            OrderIndex = orderIndex;
        }

        public TableColumn(string name, string key, TableColumnAttribute attr) : this(name, key)
        {
            OrderIndex = attr?.OrderIndex ?? 0;
            Searchable = attr?.Searchable ?? true;
            Orderable = attr?.Orderable ?? true;
            RowGroups = attr?.RowGroup ?? false;
            SumTotal = attr?.SumTotal ?? false;
            Hidden = attr?.Hidden ?? false;
        }

        public TableColumn(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public TableColumn()
        {
        }

        public string Name { get; set; }
        public string Key { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public bool RowGroups { get; set; }
        public object SumTotal { get; set; }
        public bool Hidden { get; set; }

        public override string ToString()
        {
            return $"{Key} as {Name} at {OrderIndex}";
        }

        public object GetDataTablesObject()
        {
            return new
            {
                data = Key,
                defaultContent = "<span class='st-text'>null/empty</i>",
                orderable = Orderable,
                searchable = Searchable,
                sumTotal = SumTotal,
                visible = !Hidden
            };
        }
    }
}