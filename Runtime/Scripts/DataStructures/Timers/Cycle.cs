using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class Cycle
{
    private float rate;
    public Action OnCycle { get; private set; }
    private Thread cycleThread;
    private bool running = true;
    private SynchronizationContext mainThreadContext;
    TimeSpan durationTimeSpan;

    public Cycle(float rate, Action onCycle, bool autoStart = false)
    {
        if (rate <= 0) { Debug.Log("Cycle error. Time must be grater than 0."); return; }
        this.rate = rate;
        this.OnCycle = onCycle;
        this.mainThreadContext = SynchronizationContext.Current;
        if (autoStart) { Start(); }
    }

    public void Start()
    {
        cycleThread = new Thread(RunCycle);
        cycleThread.Start();
    }

    private void RunCycle()
    {
        durationTimeSpan = TimeSpan.FromSeconds(rate);
        while (running)
        {
            Thread.Sleep(durationTimeSpan);
            if (OnCycle != null && mainThreadContext != null)
            {
                mainThreadContext.Post(_ => OnCycle.Invoke(), null);
            }
        }
    }

    public void SetNewRate(float newRateInSeconds)
    {
        if (rate <= 0) { Debug.Log("Cycle error. Time must be grater than 0."); return; }

        rate = newRateInSeconds;
        durationTimeSpan = TimeSpan.FromSeconds(rate);
    }

    public void Stop()
    {
        running = false;
        if (cycleThread != null && cycleThread.IsAlive)
        {
            cycleThread.Join();
        }
    }
}
