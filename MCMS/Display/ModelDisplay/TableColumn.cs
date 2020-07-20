using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Display.ModelDisplay
{
    public class TableColumn
    {
        public int Order { get; set; }

        public TableColumn(string name, string key, int order) : this(name, key)
        {
            Order = order;
        }

        public TableColumn(string name, string key, TableColumnAttribute attr) : this(name, key)
        {
            Order = attr?.Order ?? 0;
            Searchable = attr?.Searchable ?? true;
            Orderable = attr?.Orderable ?? true;
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

        public override string ToString()
        {
            return $"{Key} as {Name} at {Order}";
        }

        public object GetDataTablesObject()
        {
            return new
            {
                data = Key,
                defaultContent = "<span class='st-text'>null/empty</i>",
                orderable = Orderable,
                searchable = Searchable
            };
        }
    }
}