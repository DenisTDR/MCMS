using System;

namespace MCMS.Base.Attributes
{
    public class ReadOnlyApiAttribute : Attribute
    {
        public bool IsReadOnly { get; }

        public ReadOnlyApiAttribute(bool isReadOnly = true)
        {
            IsReadOnly = isReadOnly;
        }
    }
}