using System;
using System.Threading.Tasks;

public class TimerSystem
{
    // Asynchronous method to wait for a specified amount of time
    public static async Task StartTimer(float waitTime, Action callback)
    {
        // Convert waitTime to milliseconds and await Task.Delay
        await Task.Delay(TimeSpan.FromSeconds(waitTime));

        // Execute the callback after the wait
        callback?.Invoke();
    }
}
