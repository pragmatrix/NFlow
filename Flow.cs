using System;
using System.Linq;

namespace NFlow
{
	// language

	// output connectors

	public static class FlowConnect
	{
		public static ITransformer<T[], T[]> Deduplicate<T>(this IOutputPin<T[]> output)
		{
			var converter = Flow.Convert<T[], T[]>(input => input.Distinct().ToArray());
			output.ConnectTo(converter.In);
			return converter;
		}

		public static IFilter<T> Filter<T>(this IOutputPin<T> output, Func<T, bool> function)
		{
			var filter = Flow.Filter(function);
			output.ConnectTo(filter.In);
			return filter;
		}

		public static IOutputPin<T> Act<T>(this IOutputPin<T> output, Action<T> action)
		{
			var actor = Flow.Act(action);
			output.ConnectTo(actor.In);
			return output;
		}
	}

	// primitives

	public static class Flow
	{
		// parts

		public static IGate<T> Gate<T>()
		{
			return new Gate<T>();
		}

		public static IConverter<IT, OT> Convert<IT, OT>(Func<IT, OT> f)
		{
			return new Converter<IT, OT>(f);
		}

		public static IFilter<T> Filter<T>(Func<T, bool> f)
		{
			return new Filter<T>(f);
		}

		public static IActor<T> Act<T>(Action<T> action)
		{
			return new Actor<T>(action);
		}
	}

	public interface IConverter<IT, OT> : ITransformer<IT, OT>
	{
		IInputPin<Func<IT, OT>> Function { get; }
	}

	public interface IFilter<T> : ITransformer<T, T>
	{
		IInputPin<Func<T, bool>> Function { get; }
	}

	// parts

	public interface ITransformer<in IT, out OT>
	{
		IInputPin<IT> In { get; }
		IOutputPin<OT> Out { get; }
	}

	public interface IActor<in IT>
	{
		IInputPin<IT> In { get; }
	}

	public interface IOutputPin<out OT>
	{
		void ConnectTo(IInputPin<OT> outputPin);
	}

	public interface IInputPin<in IT>
	{
		void Put(IT value);
	}
}
