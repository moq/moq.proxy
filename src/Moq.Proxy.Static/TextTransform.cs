using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Moq.Proxy
{
	public abstract class TextTransform : TextTransformBase
	{
		Dictionary<TypeInfo, string> fieldNames = new Dictionary<TypeInfo, string>();

		public TextTransform (string targetNamespace, string className,
			TypeInfo baseType, IEnumerable<TypeInfo> implementedInterfaces)
		{
			if (string.IsNullOrWhiteSpace (targetNamespace))
				throw new ArgumentNullException (nameof (targetNamespace), nameof (targetNamespace));
			if (string.IsNullOrWhiteSpace (className))
				throw new ArgumentNullException (nameof (className), nameof (className));
			if (baseType == null)
				throw new ArgumentNullException (nameof (baseType), nameof (baseType));
			if (implementedInterfaces == null)
				implementedInterfaces = Enumerable.Empty<TypeInfo> ();
			if (baseType.IsGenericTypeDefinition || implementedInterfaces.Any (i => i.IsGenericTypeDefinition))
				throw new ArgumentException ();

			TargetNamespace = targetNamespace;
			ClassName = className;
			BaseType = baseType;
			ImplementedInterfaces = implementedInterfaces.SelectMany (i => i.ImplementedInterfaces).Select (i => i.GetTypeInfo ()).Distinct ();

			if (BaseType != typeof (object).GetTypeInfo ())
				BaseTypes = new[] { BaseType }.Concat (ImplementedInterfaces).ToArray ();
			else
				BaseTypes = ImplementedInterfaces.ToArray ();

			OverrideMethods = FindVirtualMethods (baseType);
			OverrideProperties = FindVirtualProperties (baseType);

			InterfaceMethods = ImplementedInterfaces.SelectMany (t =>
				 // TODO: need to implement events
				 t.DeclaredMethods.Where (m => !m.IsSpecialName).OrderBy (m => m.Name).ThenBy (m => m.GetParameters ().Length))
				 .ToArray ();
			InterfaceProperties = ImplementedInterfaces.SelectMany (t =>
				 t.DeclaredProperties.OrderBy (m => m.Name).ThenBy (m => m.GetIndexParameters ().Length))
				 .ToArray ();

			OutRefMethods = FindOutRefMethods ();
			Usings = BuildTypeMap ();

			foreach (var iface in ImplementedInterfaces) {
				var fieldName = iface.IsGenericType ? iface.Name.Substring(0, iface.Name.IndexOf('`')) : iface.Name;
				// Make it pascalCase, remove the I if present.
				if (fieldName.StartsWith ("I", StringComparison.Ordinal)) {
					var chars = new char[fieldName.Length - 1];
					chars[0] = CultureInfo.CurrentCulture.TextInfo.ToLower (fieldName[1]);
					fieldName.CopyTo (2, chars, 1, chars.Length - 1);
					fieldName = new string (chars);
				} else {
					var chars = new char[fieldName.Length];
					chars[0] = CultureInfo.CurrentCulture.TextInfo.ToLower (fieldName[0]);
					fieldName.CopyTo (1, chars, 1, chars.Length - 1);
					fieldName = new string (chars);
				}

				if (!fieldNames.ContainsValue(fieldName)) {
					fieldNames.Add(iface, fieldName);
				} else {
					for (int i = 1; i < 20; i++) {
						var numberField = fieldName + i;
						if (!fieldNames.ContainsValue (numberField)) {
							fieldNames.Add (iface, numberField);
							break;
						}
					}
				}
			}
		}

		protected string TargetNamespace { get; }

		protected string ClassName { get; }

		protected IEnumerable<string> Usings { get; }

		protected TypeInfo BaseType { get; }

		protected IEnumerable<TypeInfo> ImplementedInterfaces { get; }

		protected IEnumerable<TypeInfo> BaseTypes { get; }

		protected IEnumerable<MethodInfo> OverrideMethods { get; }

		protected IEnumerable<PropertyInfo> OverrideProperties { get; }

		protected IEnumerable<MethodInfo> InterfaceMethods { get; }

		protected IEnumerable<PropertyInfo> InterfaceProperties { get; }

		protected IEnumerable<MethodInfo> OutRefMethods { get; }

		protected IDictionary<string, string> TypeNameMap { get; private set; }

		protected string GetFieldName (TypeInfo type) => fieldNames[type];

		protected string GetFieldName (Type type) => fieldNames[type.GetTypeInfo ()];

		protected string GetTypeName (Type type) => GetTypeName (type.GetTypeInfo ());

		protected virtual string GetTypeName (TypeInfo type)
		{
			string name;
			if (TypeNameMap.TryGetValue (type.FullName, out name)) {
				return name;
			}

			return type.IsGenericType ? type.FullName.Substring (0, type.FullName.IndexOf ('`')) : type.FullName;
		}

		static IEnumerable<MethodInfo> FindVirtualMethods (TypeInfo baseType)
		{
			var type = baseType;
			var overrides = new Dictionary<string, MethodInfo>();
			while (true) {
				foreach (var method in type.DeclaredMethods.Where (x => x.IsVirtual && !x.IsSpecialName)) {
					if (type == typeof (object).GetTypeInfo () && method.Name == "Finalize")
						continue;

					if (!overrides.ContainsKey (method.ToString ()))
						overrides.Add (method.ToString (), method);
				}

				if (type.BaseType == null)
					break;

				type = type.BaseType.GetTypeInfo ();
			}

			return overrides.Values.ToArray ();
		}

		static IEnumerable<PropertyInfo> FindVirtualProperties (TypeInfo baseType)
		{
			var type = baseType;
			var overrides = new Dictionary<string, PropertyInfo>();
			while (true) {
				foreach (var property in type.DeclaredProperties.Where (x => (x.CanRead && x.GetMethod.IsVirtual) || (x.CanWrite && x.SetMethod.IsVirtual))) {
					if (!overrides.ContainsKey (property.ToString ()))
						overrides.Add (property.ToString (), property);
				}

				if (type.BaseType == null)
					break;

				type = type.BaseType.GetTypeInfo ();
			}

			return overrides.Values.ToArray ();
		}

		IEnumerable<MethodInfo> FindOutRefMethods ()
		{
			var outRefs = new Dictionary<string, MethodInfo>();
			var allMethods = OverrideMethods.Concat(InterfaceMethods)
				.Where(x => x.GetParameters().Any(p => p.IsOut || p.ParameterType.IsByRef));
			foreach (var method in allMethods) {
				if (!outRefs.ContainsKey (method.ToString ()))
					outRefs.Add (method.ToString (), method);
			}

			return outRefs.Values.ToArray ();
		}

		IEnumerable<string> BuildTypeMap ()
		{
			var paramTypes = OverrideMethods.Concat(InterfaceMethods)
				.SelectMany(method => method.GetParameters().Select(parameter => parameter.ParameterType));
			var returnTypes = OverrideMethods.Concat(InterfaceMethods)
				.Select(method => method.ReturnType);
			var genericTypes = BaseTypes.SelectMany(i => i.GenericTypeParameters);

			var usedTypes = paramTypes.Concat(returnTypes).Concat(genericTypes)
				.Concat(OverrideProperties.Concat(InterfaceProperties).Select(p => p.PropertyType))
				.Concat(OverrideProperties.Concat(InterfaceProperties).SelectMany(p => p.GetIndexParameters().Select(i => i.ParameterType)))
				.Concat(ImplementedInterfaces.Select(t => t.AsType()))
				.Concat(new[] { typeof(IList<>), typeof(IProxy), typeof(MethodInfo) }).Distinct().ToList();

			TypeNameMap = usedTypes.GroupBy (t => t.Name)
				// Only add a map for the names that we don't find duplicated
				.Where (g => g.Count () == 1).Select (g => g.First ())
				.ToDictionary (t => t.FullName, t =>
					t.GetTypeInfo ().IsGenericType ? t.Name.Substring (0, t.Name.IndexOf ('`')) : t.Name.TrimEnd ('&'));

			return usedTypes.Concat (returnTypes).Select (t => t.Namespace).Distinct ().OrderBy(s => s).ToArray ();
		}

		class TypeEqualityComparer : IEqualityComparer<Type>
		{
			public bool Equals (Type x, Type y) => x.GetTypeInfo().IsGenericType ? 
				x.Equals(y) :
 				x.FullName.TrimEnd ('&').Equals (y.FullName.TrimEnd ('&'));

			public int GetHashCode (Type obj) => obj.GetTypeInfo().IsGenericType ? 
				obj.GetTypeInfo().GetHashCode() : 
				obj.FullName.TrimEnd ('&').GetHashCode ();
		}
	}
}
