using UnityEngine;
using System.Collections;

public class Timer
{

    public bool isReady { get { return (elapsedTime > maxTime) && isRunning; } }

    float startTime;
    float maxTime;
    public float elapsedTime { get { return currentTime - startTime; } }
    public float remainingTime { get { return maxTime - elapsedTime; } }
	float currentTime { get { return timeType == TimeType.RealTime ? Time.unscaledTime : Time.time; } }
	bool isRunning = false;
	TimeType timeType = TimeType.GameTime;

	public Timer(float maxTime, TimeType timeType = TimeType.GameTime) {
        this.maxTime = maxTime;
		this.timeType = timeType;
		Start();
    }

	public void Start() {
		startTime = currentTime;
		isRunning = true;
	}

    public void Reset() {
        startTime = currentTime;
    }

}

public enum TimeType { RealTime, GameTime }
