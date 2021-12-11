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

		public static bool IsFullyFledgedMethod(MethodInfo method)
		{
			if (!method.Attributes.HasFlag(MethodAttributes.SpecialName))
			{
				return true;
			}

			string name = method.Name;
			bool isFullyFledged = !(name.StartsWith("get_", StringComparison.Ordinal)
				|| name.StartsWith("set_", StringComparison.Ordinal));

			return isFullyFledged;
		}
	}
}
