using System;
using System.Collections.Generic;
using System.Reflection;
using LinFu.DynamicProxy;

namespace Moq.Proxy.LinFu
{
	internal class Interceptor : IInterceptor,
		/* Interface implemented so that we can detect breaking changes 
		*  and update our Intercept implementation accordingly */
		IProxy
	{
		BehaviorPipeline pipeline;

		internal Interceptor ()
		{
			pipeline = new BehaviorPipeline ();
		}

		public IList<IProxyBehavior> Behaviors => pipeline.Behaviors;


		public object Intercept (InvocationInfo invocation)
		{
			//ReturnValue = invocation.TargetMethod.Invoke (invocation.Target, invocation.Arguments);
			if (invocation.TargetMethod.DeclaringType == typeof (IProxy))
				return Behaviors;

			var input = new MethodInvocation(invocation.Target, invocation.TargetMethod, invocation.Arguments);
			var returns = pipeline.Invoke(input, (i, next) => {
				try {
					var returnValue = invocation.TargetMethod.Invoke (invocation.Target, invocation.Arguments);
					return input.CreateValueReturn(returnValue, invocation.Arguments);
				}
				catch (TargetInvocationException tie) {
					return input.CreateExceptionReturn(tie.InnerException);
				}
				catch (Exception ex) {
					return input.CreateExceptionReturn(ex);
				}
			});

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			for (int i = 0; i < returns.Outputs.Count; i++) {
				var name = returns.Outputs.GetName(i);
				var index = input.Arguments.IndexOf (name);
				invocation.SetArgument (index, returns.Outputs[index]);
			}

			return returns.ReturnValue;
		}
	}
}
