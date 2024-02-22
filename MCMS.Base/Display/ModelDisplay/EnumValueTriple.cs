using Newtonsoft.Json;

namespace MCMS.Base.Display.ModelDisplay
{
    public record EnumValueTriple
    {
        [JsonProperty("value")] public string Value { get; init; }
        [JsonProperty("dbValue")] public object DbValue { get; init; }
        [JsonProperty("label")] public string Label { get; init; }

        public EnumValueTriple(string value, object dbValue, string label)
        {
            Value = value;
            DbValue = dbValue;
            Label = label;
        }

        public EnumValueTriple(object dbValue, string label)
        {
            DbValue = dbValue;
            Label = label;
        }
    }
}