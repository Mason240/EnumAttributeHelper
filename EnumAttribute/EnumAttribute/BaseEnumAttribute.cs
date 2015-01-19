using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumAttribute
{
    public abstract class BaseEnumAttribute : Attribute
    {
        public string Value { get; private set; }

        protected BaseEnumAttribute(string value)
        {
            Value = value;
        }
    }
}
