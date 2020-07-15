using System.Reflection;

namespace MCMS.Display.ModelDisplay
{
    public class DetailsField
    {

        public DetailsField(string name, PropertyInfo propertyInfo, int order) : this(name, propertyInfo)
        {
            Order = order;
        }

        public DetailsField(string name, PropertyInfo propertyInfo)
        {
            Name = name;
            PropertyInfo = propertyInfo;
        }

        public int Order { get; set; }
        public string Tag { get; set; }
        public string Name { get; }
        public PropertyInfo PropertyInfo { get; }

        public object GetFor(object obj)
        {
            return PropertyInfo.GetValue(obj);
        }
    }
}