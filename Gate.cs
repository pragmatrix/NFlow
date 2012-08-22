using System;
using NFlow.Toolbox;

namespace NFlow
{
	public enum Gate
	{
		Close, Open
	}

	public interface IGate<T> : ITransformer<T, T[]>
	{
		IInputPin<Gate> State { get; }
	}

	public static class GateExtensions
	{
		public static IActor<Gate> NestableOpener(this IInputPin<Gate> input)
		{
			int openedCounter = 0;

			Action<Gate> closer = s =>
			{
				if (s == Gate.Open)
				{
					++openedCounter;
					if (openedCounter == 1)
						input.Put(Gate.Open);
				}
				else
				{
					--openedCounter;
					if (openedCounter == 0)
						input.Put(Gate.Close);
				}
			};

			return new Actor<Gate>(closer);
		}

		public static IActor<Gate> NestableCloser(this IInputPin<Gate> input)
		{
			int closedCounter = 0;

			Action<Gate> closer = s =>
			{
				if (s == Gate.Close)
				{
					++closedCounter;
					if (closedCounter == 1)
						input.Put(Gate.Close);
				}
				else
				{
					--closedCounter;
					if (closedCounter == 0)
						input.Put(Gate.Open);
				}
			};

			return new Actor<Gate>(closer);
		}

		public static void Open(this IInputPin<Gate> pin)
		{
			pin.Put(Gate.Open);
		}

		public static void Close(this IInputPin<Gate> pin)
		{
			pin.Put(Gate.Close);
		}

		public static IDisposable Opener(this IInputPin<Gate> pin)
		{
			pin.Open();
			return new DisposeAction(pin.Close);
		}

		public static IDisposable Closer(this IInputPin<Gate> pin)
		{
			pin.Close();
			return new DisposeAction(pin.Open);
		}
	}
}
