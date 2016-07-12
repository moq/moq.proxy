using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;
using System.Reflection;
using Moq.Proxy.Generator.Templates;
using System.IO;
using System.Collections;

namespace Moq.Proxy.Tests
{
	public class GeneratorTests
	{
		ITestOutputHelper output;

		public GeneratorTests (ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void when_creating_proxy_then_it_compiles ()
		{
			var generator = new CsInterfaceProxy(
				"Foo",
				"BarProxy",
				typeof(object).GetTypeInfo(),
				new [] { typeof(IDictionary<string, int>).GetTypeInfo() });

			var source = generator.TransformText();
			File.WriteAllText ("Foo.cs", source);

			var syntax = CSharpSyntaxTree.ParseText(source, path: Path.Combine(Directory.GetCurrentDirectory(), "Foo.cs"));
			var compilation = CSharpCompilation.Create ("Foo",
				new [] { syntax },
				AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.ManifestModule.FullyQualifiedName)),
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			var result = compilation.Emit("Foo.dll");
			if (!result.Success)
				output.WriteLine (string.Join (Environment.NewLine, result.Diagnostics.Select (d => d.ToString ())));

			Assert.True (result.Success);
		}

		public class MyDict<TKey, TValue> : IDictionary<TKey, TValue>
		{
			TValue IDictionary<TKey, TValue>.this[TKey key]
			{
				get
				{
					throw new NotImplementedException ();
				}

				set
				{
					throw new NotImplementedException ();
				}
			}

			int ICollection<KeyValuePair<TKey, TValue>>.Count
			{
				get
				{
					throw new NotImplementedException ();
				}
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
			{
				get
				{
					throw new NotImplementedException ();
				}
			}

			ICollection<TKey> IDictionary<TKey, TValue>.Keys
			{
				get
				{
					throw new NotImplementedException ();
				}
			}

			ICollection<TValue> IDictionary<TKey, TValue>.Values
			{
				get
				{
					throw new NotImplementedException ();
				}
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException ();
			}

			void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
			{
				throw new NotImplementedException ();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
			{
				throw new NotImplementedException ();
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException ();
			}

			bool IDictionary<TKey, TValue>.ContainsKey (TKey key)
			{
				throw new NotImplementedException ();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				throw new NotImplementedException ();
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				throw new NotImplementedException ();
			}

			IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
			{
				throw new NotImplementedException ();
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException ();
			}

			bool IDictionary<TKey, TValue>.Remove (TKey key)
			{
				throw new NotImplementedException ();
			}

			bool IDictionary<TKey, TValue>.TryGetValue (TKey key, out TValue value)
			{
				throw new NotImplementedException ();
			}
		}

		public interface IFoo
		{
			void Do ();
		}

		public interface IBar
		{
			void Do ();
		}

		public class Foo : IFoo
		{
			public virtual void Do () { }
		}

		public class DerivedFoo : Foo
		{
			public override void Do ()
			{
				base.Do ();

				Action method = base.Do;

				var info = method.GetMethodInfo();

				Assert.Equal (info.DeclaringType, typeof (Foo));
			}
		}
	}
}
