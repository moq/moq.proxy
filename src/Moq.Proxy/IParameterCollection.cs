using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy
{
	public interface IParameterCollection : IEnumerable<object>
	{
		bool Contains (string parameterName);

		int Count { get; }

		string GetName (int index);

		ParameterInfo GetInfo (int index);

		ParameterInfo GetInfo (string parameterName);

		int IndexOf (string parameterName);

		object this[string parameterName] { get; set; }

		object this[int index] { get; set; }
	}
}
