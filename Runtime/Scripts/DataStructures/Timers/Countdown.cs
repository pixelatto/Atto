using UnityEngine;
using UnityEngine.Events;


public class Countdown
{
    private float duration;
    private float endTime;
    private bool isRunning;

    public UnityEvent OnComplete { get; private set; }

    public float elapsed => isRunning ? Time.time - (endTime - duration) : duration;
    public float remaining => isRunning ? endTime - Time.time : 0f;

    public Countdown(float initialTime = 0f)
    {
        duration = initialTime;
        OnComplete = new UnityEvent();
        Start();
    }

    public void Start()
    {
        endTime = Time.time + duration;
        isRunning = true;
    }

    public void Update()
    {
        if (isRunning && Time.time >= endTime)
        {
            isRunning = false;
            OnComplete.Invoke();
        }
    }

    public static implicit operator Countdown(float initialTime)
    {
        return new Countdown(initialTime);
    }
}
