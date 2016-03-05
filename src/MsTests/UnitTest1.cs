using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Proxy.Generator.Templates;
using System.Linq;
using System.Reflection;

namespace MsTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GenerateCode()
        {
            var generator = new CsInterfaceProxy(
                "FooNamespace",
                "FooProxy",
                typeof(SecondDerived).GetTypeInfo(),
                new[] { typeof(ICalculator).GetTypeInfo() });

            Console.WriteLine(generator.TransformText());
        }
        
        [TestMethod]
        public void Reflection()
        {
			var foo = new SecondDerived();
			foo.Do ();
        }

        [TestMethod]
        public void CompareMethods()
        {
            var method = typeof(SecondDerived).GetMethod("Do");
			var declared = method.GetBaseDefinition ();

			var map = declared.DeclaringType.GetInterfaceMap(typeof(IFoo));
			MethodInfo info = null;

			for (int i = 0; i < map.InterfaceMethods.Length; i++) {
				if (declared == map.TargetMethods[i])
					info = map.TargetMethods[i];
			}

			Assert.IsNotNull (info);
			Assert.AreEqual (declared.DeclaringType, typeof (Foo));
        }
    }

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

		public override void Do ()
		{
			Action baseDo = base.Do;
			base.Do ();
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

		public override string ToString () => "Foo Rocks";

		public void Dispose()
        {
        }
    }
}
