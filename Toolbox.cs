using System;

namespace NFlow
{
	namespace Toolbox
	{
		// http://stackoverflow.com/questions/786383/c-sharp-events-and-thread-safety

		static class EventExtensions
		{
			public static void raise(this Action action)
			{
				if (action != null)
					action();
			}

			public static void raise<T1>(this Action<T1> action, T1 value1)
			{
				if (action != null)
					action(value1);
			}

			public static void raise<T1, T2>(this Action<T1, T2> action, T1 value1, T2 value2)
			{
				if (action != null)
					action(value1, value2);
			}
		}

		public sealed class DisposeAction : IDisposable
		{
			readonly Action _action_;

			public DisposeAction(Action action)
			{
				_action_ = action;
			}

			public void Dispose()
			{
				if (_action_ != null)
					_action_();
			}

			// that should be faster than an empty DisposeAction 
			// (because boxing to IDisposable would always create a new instance).

			public static readonly IDisposable None = new DummyDisposable();

			sealed class DummyDisposable : IDisposable
			{
				public void Dispose()
				{
				}
			}
		}
	}

}
