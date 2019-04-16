using UnityEngine;

namespace Atto
{
	public class Timer
	{
		public float elapsedTime
		{
			get { return currentTime - startTime; }
		}

		public float remainingTime
		{
			get { return maxTime - elapsedTime; }
		}

		public bool isReady
		{
			get { return (elapsedTime > maxTime) && isRunning; }
		}

		private float startTime;
		private float maxTime;
		private bool isRunning = false;
		private TimeType timeType = TimeType.GameTime;

		private float currentTime
		{
			get { return (timeType == TimeType.RealTime ? Time.unscaledTime : Time.time); }
		}

		public Timer(float maxTime, TimeType timeType = TimeType.GameTime)
		{
			this.maxTime = maxTime;
			this.timeType = timeType;

			Start();
		}

		public void Start()
		{
			startTime = currentTime;
			isRunning = true;
		}

		public void Reset()
		{
			startTime = currentTime;
		}
	}

	public enum TimeType
	{
		RealTime,
		GameTime
	}
}
