namespace MCMS.Models.Dt
{
    public class DtSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }

        public bool HasValue => !string.IsNullOrEmpty(Value);

        public bool? GetValueBool(bool nullable = false)
        {
            if (Value?.ToLower().Trim() == "true")
            {
                return true;
            }

            return nullable ? Value?.ToLower().Trim() == "false" ? false : null : false;
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