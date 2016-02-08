using System.Reflection;

namespace Moq.Proxy
{
	public interface IMethodCall
	{
		object[] Arguments { get; }

		MethodInfo Method { get; }

		object ReturnValue { get; set; }

		void InvokeTarget();

		void SetArgument(int index, object value);
	}
}