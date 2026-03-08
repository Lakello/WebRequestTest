namespace Common.Runtime.Energy
{
	using System;
	using R3;

	public sealed class Energy : IEnergy, IDisposable
	{
		public int Max { get; }

		private readonly ReactiveProperty<int> _current;
		public ReadOnlyReactiveProperty<int> Current => _current;

		public Energy(int max, int startValue)
		{
			if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));
			if (startValue < 0 || startValue > max) throw new ArgumentOutOfRangeException(nameof(startValue));

			Max = max;
			_current = new ReactiveProperty<int>(startValue);
		}

		public bool TrySpend(int amount)
		{
			if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

			if (_current.Value < amount)
				return false;

			_current.Value -= amount;
			return true;
		}

		public void Add(int amount)
		{
			if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

			var next = _current.Value + amount;
			if (next > Max) next = Max;

			_current.Value = next;
		}

		public void Dispose()
		{
			_current.Dispose();
		}
	}
}