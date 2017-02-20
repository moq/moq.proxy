using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy
{
	public interface IParameterCollection : IEnumerable<object>
	{
		bool Contains(string parameterName);

		int Count { get; }

		string GetName(int index);

		ParameterInfo GetInfo(int index);

		ParameterInfo GetInfo(string parameterName);

		int IndexOf(string parameterName);

		object this[string parameterName] { get; set; }

		object this[int index] { get; set; }
	}
}
