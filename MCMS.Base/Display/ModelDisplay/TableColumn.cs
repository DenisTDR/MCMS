using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;

namespace MCMS.Base.Display.ModelDisplay
{
    public class TableColumn
    {
        public int OrderIndex { get; set; }

        public TableColumn(string name, string key, int orderIndex) : this(name, key)
        {
            OrderIndex = orderIndex;
        }

        public TableColumn(PropertyInfo prop, IList<TableColumnAttribute> attrs)
            : this(TypeHelpers.GetDisplayNameOrDefault(prop), prop.Name.ToCamelCase())
        {
            var attr = attrs.FirstOrDefault();
            OrderIndex = attr?.OrderIndex ?? 0;
            Searchable = attr?.Searchable ?? ServerClient.Both;
            Orderable = attr?.Orderable ?? ServerClient.Both;
            RowGroups = attr?.RowGroup ?? false;
            SumTotal = attr?.SumTotal ?? false;
            Hidden = attr?.Hidden ?? false;
            Tag = attr?.Tag;
            Invisible = attr?.Invisible ?? false;
            DbColumn = attr?.DbColumn ?? Key;
            DbFuncFormat = attr?.DbFuncFormat;

            Type = attr?.Type ?? TableColumnType.Default;
            this.BuildTypeAndPatchFilter(prop);
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
        public ServerClient Searchable { get; set; }
        public ServerClient Orderable { get; set; }
        public bool RowGroups { get; set; }
        public object SumTotal { get; set; }
        public bool Hidden { get; set; }
        public string Tag { get; set; }
        public bool Invisible { get; set; }

        public string DefaultContent { get; set; }
        public string ClassName { get; set; }
        public string HeaderClassName { get; set; }

        public string DbColumn { get; set; }
        public string DbFuncFormat { get; set; }
        public TableColumnType Type { get; set; }
        public List<ValueLabelPair> FilterValues { get; set; }

        public override string ToString()
        {
            return $"{Key} as {Name} at {OrderIndex}";
        }

        public object GetDataTablesObject(bool serverSide = false)
        {
            return new
            {
                data = Key,
                defaultContent = DefaultContent ?? "<span class='st-text'>null/empty</i>",
                orderable = Orderable.IsClient(serverSide),
                searchable = Searchable.IsClient(serverSide),
                sumTotal = SumTotal,
                visible = !Hidden && !Invisible,
                tag = Tag,
                className = ClassName,
                mType = Type.GetCustomValue(),
                mFilterValues = FilterValues
            };
        }
    }

    public static class TableColumnExtensions
    {
        public static string BuildHeaderClassSyntax(this TableColumn col)
        {
            if (string.IsNullOrEmpty(col.HeaderClassName)) return null;
            return "class=\"" + col.HeaderClassName + "\"";
        }

        public static void BuildTypeAndPatchFilter(this TableColumn col, PropertyInfo prop)
        {
            if (col.Type == TableColumnType.Default && prop.PropertyType == typeof(bool))
            {
                col.Type = TableColumnType.Bool;
                col.FilterValues = new List<ValueLabelPair>
                {
                    new("", "-"),
                    new("true", "True"),
                    new("false", "False"),
                };
            }

            if (col.Type == TableColumnType.Default && prop.PropertyType.IsNumericType())
            {
                col.Type = TableColumnType.Number;
            }

            if (col.Type == TableColumnType.Default && prop.PropertyType.IsEnum)
            {
                col.Type = TableColumnType.Select;
                col.FilterValues = Enum.GetValues(prop.PropertyType).Cast<Enum>()
                    .Select(enumValue =>
                        new ValueLabelPair(Convert.ToInt32(enumValue).ToString(), enumValue.GetDisplayName()))
                    .Prepend(
                        new("", "-")).ToList();
            }
        }
    }

    public enum TableColumnType
    {
        [EnumMember(Value = "default")] Default,
        [EnumMember(Value = "number")] Number,
        [EnumMember(Value = "bool")] Bool,
        [EnumMember(Value = "select")] Select,
    }
}