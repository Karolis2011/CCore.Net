using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed.Mapping
{
    public class MaxMappingValidator : MappingValidator
    {
        public override MappingInfo Map(Type type) => new MappingInfo { Freeze = true, Mapped = true };

        public override MappingInfo Map(Type type, FieldInfo field) => new MappingInfo { Mapped = true, Name = field.Name, Enumerable = true, Freeze = true };

        public override MappingInfo Map(Type type, PropertyInfo property) => new MappingInfo { Mapped = true, Name = property.Name, Enumerable = true, Freeze = true };

        public override MappingInfo Map(Type type, MethodInfo method) => new MappingInfo { Mapped = true, Name = method.Name, Enumerable = true, Freeze = true };
    }
}
