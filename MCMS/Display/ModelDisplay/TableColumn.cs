namespace MCMS.Display.ModelDisplay
{
    public class TableColumn
    {
        public int Order { get; set; }

        public TableColumn(string name, string key, int order) : this(name, key)
        {
            Order = order;
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

        public override string ToString()
        {
            return $"{Key} as {Name} at {Order}";
        }
    }
}