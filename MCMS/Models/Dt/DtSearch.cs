namespace MCMS.Models.Dt
{
    public class DtSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }

        public bool HasValue => !string.IsNullOrEmpty(Value);

        public bool GetValueBool()
        {
            return Value.ToLower().Trim() == "true";
        }
        public decimal GetValueDecimal()
        {
            return decimal.Parse(Value);
        }

        public int GetValueInteger()
        {
            return int.Parse(Value);
        }
    }
}