using System;
using System.Collections.Generic;

namespace NFlow
{
	static class Input
	{
		public static IInputPin<T> BindTo<T>(Action<T> action)
		{
			return new InputActionPin<T>(action);
		}
	}

	sealed class InputActionPin<T> : IInputPin<T>
	{
		readonly Action<T> _act;

		public InputActionPin(Action<T> act)
		{
			_act = act;
		}

		public void Put(T value)
		{
			_act(value);
		}
	}

	sealed class OutputPin<T> : IOutputPin<T>
	{
		readonly List<IInputPin<T>> _targets = new List<IInputPin<T>>();

		public void emit(T value)
		{
			foreach (var target in _targets)
				target.Put(value);
		}

		public void ConnectTo(IInputPin<T> outputPin)
		{
			_targets.Add(outputPin);
		}
	}

}
