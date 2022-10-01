using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Data = attr?.DataSelector ?? Key;

            Type = attr?.Type ?? TableColumnType.Default;
            this.BuildTypeAndPatchFilter(prop);
        }

        public TableColumn(string name, string key)
        {
            Name = name;
            Key = key;
            Data = key;
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
        public string Data { get; set; }

        public override string ToString()
        {
            return $"{Key} as {Name} at {OrderIndex}";
        }

        public object GetDataTablesObject(bool serverSide = false)
        {
            return new
            {
                data = Data,
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
            if (col.Type != TableColumnType.Default)
            {
                return;
            }

            if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                col.Type = TableColumnType.Bool;
                col.FilterValues = new List<ValueLabelPair>
                {
                    new("", "-"),
                    new("true", "True"),
                    new("false", "False"),
                };
                if (prop.PropertyType == typeof(bool?))
                {
                    col.Type = TableColumnType.NullableBool;
                    col.FilterValues.Add(new("null", "Not set"));
                }
            }
            else if (prop.PropertyType.IsNumericType())
            {
                col.Type = TableColumnType.Number;
            }
            else if (prop.PropertyType.IsEnum)
            {
                col.Type = TableColumnType.Select;
                col.FilterValues = Enum.GetValues(prop.PropertyType).Cast<Enum>()
                    .Select(enumValue =>
                        new ValueLabelPair(Convert.ToInt32(enumValue).ToString(), enumValue.GetDisplayName()))
                    .Prepend(
                        new("", "-")).ToList();
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                if (prop.PropertyType.GetCustomAttribute<DataTypeAttribute>() is { } attr)
                {
                    if (attr.DataType == DataType.Date)
                    {
                        col.Type = TableColumnType.Date;
                    }
                    else if (attr.DataType == DataType.Time)
                    {
                        col.Type = TableColumnType.Time;
                    }

                    col.Type = TableColumnType.DateTime;
                }
            }
        }
    }

    public enum TableColumnType
    {
        [EnumMember(Value = "default")] Default,
        [EnumMember(Value = "number")] Number,
        [EnumMember(Value = "bool")] Bool,
        [EnumMember(Value = "nBool")] NullableBool,
        [EnumMember(Value = "dateTime")] DateTime,
        [EnumMember(Value = "date")] Date,
        [EnumMember(Value = "time")] Time,
        [EnumMember(Value = "select")] Select,
    }
}