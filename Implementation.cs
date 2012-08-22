using System;
using System.Collections.Generic;

namespace NFlow
{
	
	/*
		A gate is by default closed (like actual gates).

		A gate always delivers its input to its output pin in chunks.

		When the gate is closed, all input values are queued and as soon it 
		is opened are being delivered as a single chunk.

		A chunk is represented by an array.

		If the gate is open, one single value (as a chunk) is deliverd.

		I decided to use "chunks" for the output is to maintain
		maintain the transactional nature behind the gate.
	
		For example, a deduplication could be easily done behind a gate, to remove 
		duplicates that appeared while waiting the gate to be opened.
	*/

	sealed class Gate<T> : Transformer<T, T[]>, IGate<T>
	{
		readonly List<T> _queue = new List<T>();
		Gate _state;

		public Gate()
		{
			State = Input.BindTo<Gate>(setState);
		}

		protected override void process(T value)
		{
			if (_state == Gate.Open)
				emit(new[] {value});
			else
				_queue.Add(value);
		}
	
		void setState(Gate state)
		{
			_state = state;
			if (_state == Gate.Open)
				flushQueue();
		}

		void flushQueue()
		{
			if (_queue.Count == 0)
				return;
			var payload = _queue.ToArray();
			_queue.Clear();
			emit(payload);
		}

		public IInputPin<Gate> State { get; private set; }
	}
	
	/*
		A converter converts inputs to outputs. Its conversion function can be parameterized from 
		within the constructor and the Function pin.
	*/

	sealed class Converter<IT, OT> : Transformer<IT, OT>, IConverter<IT, OT>
	{
		Func<IT, OT> _function;

		public Converter(Func<IT, OT> function)
		{
			setFunction(function);
			Function = Input.BindTo<Func<IT, OT>>(setFunction);
		}

		protected override void process(IT i)
		{
			emit(_function(i));
		}

		void setFunction(Func<IT, OT> f)
		{
			_function = f;
		}

		public IInputPin<Func<IT, OT>> Function { get; private set; }
	}

	abstract class Transformer<IT, OT> : ITransformer<IT, OT>
	{
		readonly OutputPin<OT> _out = new OutputPin<OT>();

		protected Transformer()
		{
			In = Input.BindTo<IT>(process);
		}

		protected void emit(OT o)
		{
			_out.emit(o);
		}

		protected abstract void process(IT i);

		public IInputPin<IT> In { get; private set; }
		public IOutputPin<OT> Out { get { return _out; } }
	}

	sealed class Actor<T> : IActor<T>
	{
		readonly Action<T> _action;

		public Actor(Action<T> action)
		{
			_action = action;

			In = Input.BindTo<T>(act);
		}

		void act(T value)
		{
			_action(value);
		}

		public IInputPin<T> In { get; private set; }
	}

	sealed class Filter<T> : Transformer<T, T>, IFilter<T>
	{
		Func<T, bool> _function;

		public Filter(Func<T, bool> function)
		{
			Function = Input.BindTo<Func<T, bool>>(setFunction);

			setFunction(function);
		}

		void setFunction(Func<T, bool> function)
		{
			_function = function;
		}

		protected override void process(T i)
		{
			if (_function(i))
				emit(i);
		}

		public IInputPin<Func<T, bool>> Function { get; private set; }
	}
}
