using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy.Tests
{
	public class ManualProxyTest
	{
	}

	public interface ICalculator
	{
		int Add (int x, int y);
	}

	public class CalculatorProxy : ICalculator, IProxy
	{
		BehaviorPipeline pipeline;
		ICalculator target;

		public CalculatorProxy ()
		{
			pipeline = new BehaviorPipeline ();
		}

		public CalculatorProxy (ICalculator calculator)
			: this()
		{
			target = calculator;
		}

		public IList<IProxyBehavior> Behaviors => pipeline.Behaviors;

		public int Add (int x, int y)
		{
			var returns = pipeline.Invoke(
				new MethodInvocation(this, MethodBase.GetCurrentMethod(), x, y),
				(input, next) => {
					try {
						// Four possible scenarios here:
						// - call base implementation 
						// - call wrapped instance
						// - generate a dummy return value (like AutoFixture)
						// - return default value for return type.

						var returnValue = target?.Add(x, y);
						return input.CreateValueReturn(returnValue, x, y);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			return ((int?)returns.ReturnValue).GetValueOrDefault();
		}
	}
}
