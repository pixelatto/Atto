using System;
using UnityEditor;

namespace Atto.Utils
{
	public class EditorTimer
	{
		public event Action OnTimeout;

		public float TotalTime { get; set; } = 5f;
		public float TimeElapsed { get; private set; }
		public float Remaining => TotalTime - TimeElapsed;

		private State state = State.Stop;
		private double previousTime;
		private double instantElapsed;

		public void Start()
		{
			Reset();
			Continue();
		}

		public void Continue()
		{
			if (state == State.Stop)
			{
				previousTime = EditorApplication.timeSinceStartup;
				EditorApplication.update += Update;
				state = State.Running;
			}
		}

		public void Stop()
		{
			if (state == State.Running)
			{
				EditorApplication.update -= Update;
				state = State.Stop;
			}
		}

		public void Reset() => TimeElapsed = 0;
		public bool Passed(float time) => TimeElapsed >= time;

		private void Update()
		{
			instantElapsed = EditorApplication.timeSinceStartup - previousTime;
			TimeElapsed += (float)instantElapsed;
			//Debug.Log(TimeElapsed);
			if (Passed(TotalTime))
			{
				Stop();
				OnTimeout?.Invoke();
			}

			previousTime = EditorApplication.timeSinceStartup;
		}

		private enum State { Stop, Running }
	}
}