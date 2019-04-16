using System;

namespace Atto
{
	public class ObservedValue<T>
	{
		#pragma warning disable 0067
		public event Action OnValueChange;
		#pragma warning restore 0067

		private T currentValue;

		public ObservedValue()
		{
			currentValue = default(T);
		}

		public ObservedValue(T initialValue)
		{
			currentValue = initialValue;
		}

		public T Value
		{
			get { return currentValue; }

			set
			{
				if (!currentValue.Equals(value))
				{
					currentValue = value;

					if (OnValueChange != null)
					{
						OnValueChange();
					}
				}
			}
		}

		public void SetSilently(T value)
		{
			currentValue = value;
		}
	}
}
