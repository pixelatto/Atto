using UnityEngine;

public class Timer
{
    public float elapsed => Time.time - startTime;
    private float startTime = 0;

    public Timer(float startTime = 0f)
    {
        
    }

    public void Restart()
    {
        startTime = Time.time;
    }

    public static implicit operator Timer(float startTime)
    {
        return new Timer(startTime);
    }
}
