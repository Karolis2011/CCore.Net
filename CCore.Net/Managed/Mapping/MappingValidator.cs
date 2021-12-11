using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed.Mapping
{
    public abstract class MappingValidator
    {
        abstract public MappingInfo Map(Type type);
        abstract public MappingInfo Map(Type type, FieldInfo field);
        abstract public MappingInfo Map(Type type, PropertyInfo property);
        abstract public MappingInfo Map(Type type, MethodInfo method);
    }
}
