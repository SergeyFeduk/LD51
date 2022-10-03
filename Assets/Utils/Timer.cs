using UnityEngine;

public class Timer
{
    private float timeLeft = 0;
    private bool useIndependentTimeScale = false;
    public Timer() { }
    public Timer(float time) {
        SetTimer(time);
    }

    public void SetIndependentTime(bool value) {
        useIndependentTimeScale = value;
    }

    public void SetTimerFrequency(float frequency) {
        timeLeft = 1 / frequency;
    }

    public void SetTimer(float time) {
        timeLeft = time;
    }

    public bool ExecuteTimer() {
        if (timeLeft <= 0) return true;
        timeLeft -= useIndependentTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        return false;
    }

    public float GetTimeLeft() {
        return timeLeft;
    }
}
