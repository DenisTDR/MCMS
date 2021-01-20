using Newtonsoft.Json;

namespace MCMS.Base.Display.ModelDisplay
{
    public class ValueLabelPair
    {
        public ValueLabelPair(string value, string label)
        {
            Value = value;
            Label = label;
        }

        public ValueLabelPair()
        {
        }

        [JsonProperty("value")] public string Value { get; set; }

        [JsonProperty("label")] public string Label { get; set; }
    }
}