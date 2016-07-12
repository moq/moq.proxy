using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq.Proxy.Tests.Reflection;
using System.Reflection;

namespace Moq.Proxy.Tests
{
	public class ReflectionTests
	{
		[Fact]
		public void CompareMethods()
		{
			var method = typeof(SecondDerived).GetMethod("Do");
			var declared = method.GetBaseDefinition();

			var map = declared.DeclaringType.GetInterfaceMap(typeof(IFoo));
			MethodInfo info = null;

			for (int i = 0; i < map.InterfaceMethods.Length; i++)
			{
				if (declared == map.TargetMethods[i])
					info = map.TargetMethods[i];
			}

			Assert.NotNull(info);
			Assert.Equal(declared.DeclaringType, typeof(Foo));
		}
	}
}

namespace Moq.Proxy.Tests.Reflection
{
	public interface ICalculator
	{
		int Add(int x, int y);

		bool TryAdd(int seed, ref int x, ref int y, out int z, out int bar);

		void TurnOn();
	}

	public interface IFoo
	{
		void Do();
	}

	public interface IBar
	{
		void Do();
	}

	public class DerivedFoo : Foo
	{
	}

	public class SecondDerived : DerivedFoo
	{
		public bool TryAdd(int seed, ref int x, ref int y, out int z)
		{
			z = x + y;
			return true;
		}

		public override void Do()
		{
			Action baseDo = base.Do;
			base.Do();
		}

		public override void VirtualDo(int foo)
		{
			base.VirtualDo(foo);
		}
	}

	public class Foo : IFoo, IDisposable
	{
		public virtual void Do() { }

		public virtual void VirtualDo(int value) { }

		public void NonVirtualDo() { }

		public override string ToString() => "Foo Rocks";

		public void Dispose()
		{
		}
	}
}
