using System;

namespace MCMS.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ViewLayoutAttribute : Attribute
    {
        public ViewLayoutAttribute(string layoutName)
        {
            LayoutName = layoutName;
        }

        public string LayoutName { get; }
    }
}