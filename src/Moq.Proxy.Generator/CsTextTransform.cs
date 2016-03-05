using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moq.Proxy.Generator
{
	public abstract class CsTextTransform : TextTransform
	{
		static readonly Dictionary<string, string> typeAliases = new Dictionary<string, string>
		{
			{ typeof (void).FullName, "void" },
			{ typeof (string).FullName, "string" },
			{ typeof (bool).FullName, "bool" },
			{ typeof (int).FullName, "int" },
			{ typeof (long).FullName, "long" },
			{ typeof (double).FullName, "double" },
			// Same types, but by ref since those aren't equivalent
			{ typeof (string).MakeByRefType().FullName, "string" },
			{ typeof (bool).MakeByRefType().FullName, "bool" },
			{ typeof (int).MakeByRefType().FullName, "int" },
			{ typeof (long).MakeByRefType().FullName, "long" },
			{ typeof (double).MakeByRefType().FullName, "double" },
		};

		public CsTextTransform (string targetNamespace, string className,
			TypeInfo baseType, IEnumerable<TypeInfo> implementedInterfaces)
			: base (targetNamespace, className, baseType, implementedInterfaces)
		{
			// Replace type aliases from type map.
			foreach (var alias in typeAliases) {
				TypeNameMap[alias.Key] = alias.Value;
			}
		}

		protected string GetDelegate (MethodInfo method)
		{
			var parameters = method.GetParameters();
			if (parameters.Length == 0)
				return method.IsVoid () ? "Action" : "Func<" + GetTypeName(method.ReturnType) + ">";

			if (method.HasOutOrRef ()) {
				// In this case the template will generate a custom delegate with the 
				// method name and all parameter types, to ensure uniqueness, i.e. FooInt32BooleanString
				return method.Name + string.Join ("", parameters.Select (x => x.ParameterType.Name.TrimEnd ('&')));
			} else {
				var typeParams = string.Join(", ", parameters.Select(x => GetTypeName(x.ParameterType)));
				return method.IsVoid () ?
					"Action<" + typeParams + ">" :
					"Func<" + typeParams + ", " + GetTypeName(method.ReturnType) + ">";
			}
		}

		// METHODS

		// Gets the arguments to pass to the actual method invocation.
		protected string GetArgs (MethodInfo method) => string.Join (", ", method.GetParameters ().Select (p =>
			(p.IsOut ? "out " : (p.ParameterType.IsByRef ? "ref " : "")) + (p.IsOut || p.ParameterType.IsByRef ? "local_" : "") + p.Name));

		// Gets the parameter delarations for the method signature.
		protected string GetParams (MethodInfo method) => string.Join (", ", method.GetParameters ().Select (p =>
			(p.IsOut ? "out " : (p.ParameterType.IsByRef ? "ref " : "")) + GetTypeName(p.ParameterType) + " " + p.Name));

		// Gets the variables that contain the values used in the invocation.
		protected string GetVars (MethodInfo method) => string.Join (", ", method.GetParameters ().Select (p =>
			(p.IsOut || p.ParameterType.IsByRef ? "local_" : "") + p.Name));


		// PROPERTIES
		protected string GetPropertyName (PropertyInfo property) =>
			property.GetIndexParameters ().Length != 0 ? "this" : property.Name;

		protected string CallProperty (PropertyInfo property) =>
			property.GetIndexParameters ().Length != 0 ? 
			"[" + GetIndexVars(property) + "]" : 
			"." + property.Name;


		// Gets the arguments to pass to the actual method invocation.
		protected string GetIndexArgs (PropertyInfo property) => property.GetIndexParameters().Length == 0 ? "" : 
			"[" + string.Join (", ", property.GetIndexParameters ().Select (p => p.Name)) + "]";

		// Gets the parameter delarations for the method signature.
		protected string GetIndexParams (PropertyInfo property) => property.GetIndexParameters().Length == 0 ? "" : 
			"[" + string.Join (", ", property.GetIndexParameters ().Select (p => GetTypeName (p.ParameterType) + " " + p.Name)) + "]";

		// Gets the variables that contain the values used in the invocation.
		protected string GetIndexVars (PropertyInfo property) => string.Join (", ", property.GetIndexParameters().Select (p => p.Name));

		protected override string GetTypeName (TypeInfo type)
		{
			if (type.IsGenericType) {
				return base.GetTypeName(type) + "<" + 
					string.Join (", ", type.GenericTypeArguments.Select (p => GetTypeName (p))) + ">";
			} 

			if (type.IsArray && type.HasElementType && type.GetElementType().GetTypeInfo().IsGenericType) {
				return GetTypeName (type.GetElementType ()) + string.Join("", 
					Enumerable.Range (0, type.GetArrayRank ()).Select (_ => "[]"));
			}

			return base.GetTypeName (type).TrimEnd ('&');
		}
	}
}
