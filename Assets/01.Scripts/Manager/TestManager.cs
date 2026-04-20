using UnityEngine;
using UnityEngine.Events;

public class TestManager : MonoBehaviour
{
    [SerializeField] private UnityEvent OnsubwayPause;
    [SerializeField] private UnityEvent OnSubwayContainue;
    private float currentTime = 0f;
    private bool ispaused = false;

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 30 && currentTime <= 45 && ispaused == false) { OnsubwayPause?.Invoke(); Debug.Log("스케줄러 사용 종료"); ispaused = true; }
        else if (currentTime > 45 && ispaused == true) { OnSubwayContainue?.Invoke(); Debug.Log("스케줄러 사용 시작"); ispaused = false; }
    }
}
