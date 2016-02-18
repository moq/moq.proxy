using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using System.Linq;

namespace Moq.Proxy.Castle
{
	public class ProxyFactory : IProxyFactory
	{
		static readonly ProxyGenerator generator;
		static readonly ProxyGenerationOptions proxyOptions;

		[SuppressMessage ("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "By Design")]
		static ProxyFactory ()
		{
#pragma warning disable 618
			AttributesToAvoidReplicating.Add<SecurityPermissionAttribute> ();
#pragma warning restore 618

			AttributesToAvoidReplicating.Add<ReflectionPermissionAttribute> ();
			AttributesToAvoidReplicating.Add<PermissionSetAttribute> ();
			AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.MarshalAsAttribute> ();
			AttributesToAvoidReplicating.Add<UIPermissionAttribute> ();
			AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.TypeIdentifierAttribute> ();

			proxyOptions = new ProxyGenerationOptions { Hook = new ProxyMethodHook () };
			generator = new ProxyGenerator ();
		}

		/// <inheritdoc />
		public object CreateProxy (Type baseType, Type[] implementedInterfaces, object[] constructorArguments)
		{
			if (baseType.IsInterface) {
				// TODO: should Moq.Core do this work? It's the same for 
				// Castle and LinFu and presumably other (future?) libraries

				var fixedInterfaces = new Type[implementedInterfaces.Length + 1];
				fixedInterfaces[0] = baseType;
				implementedInterfaces.CopyTo (fixedInterfaces, 1);
				implementedInterfaces = fixedInterfaces;
				baseType = typeof (object);
			}

			if (!implementedInterfaces.Contains(typeof(IProxy))) {
				var fixedInterfaces = new Type[implementedInterfaces.Length + 1];
				fixedInterfaces[0] = typeof(IProxy);
				implementedInterfaces.CopyTo (fixedInterfaces, 1);
				implementedInterfaces = fixedInterfaces;
			}

			// TODO: the proxy factory should automatically detect requests to proxy 
			// delegates and generate an interface on the fly for them, without Moq 
			// having to know about it at all.

			return generator.CreateClassProxy (baseType, implementedInterfaces, proxyOptions, constructorArguments, new Interceptor ());
		}
	}
}
