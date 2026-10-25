using UnityEngine;

public class PCB_Timer : MonoBehaviour
{
    public float elapsed_time = 0f;
    public bool timer_running = false;
    public bool IsRunning => timer_running;
    public void StartTimer()
    {
        elapsed_time = 0f;
        timer_running = true;
    }

    private void Update()
    {
        if (!timer_running) { return; }
        elapsed_time += Time.deltaTime;
    }
}