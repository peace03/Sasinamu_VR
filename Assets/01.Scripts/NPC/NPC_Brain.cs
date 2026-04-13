using UnityEngine;

public class NPC_Brain : MonoBehaviour
{
    [SerializeField] private NPC_BT BT;
    private TimeManager timeManager;

    private void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
    }

    private void Start()
    {
        //지하철 도착 몇분 전
        float randomWaitTime = Random.Range(20f, 90f);
        float subwayArrivalTime = 120f; //지하철 도착시간
        float targetAlarmTime = subwayArrivalTime - randomWaitTime;
        timeManager.RegisterAlarm(targetAlarmTime, OnLineUpAlarmTriggered);
        Debug.Log("알람 울릴 시간: "+targetAlarmTime);
    }

    private void OnLineUpAlarmTriggered()
    {
        Debug.Log("줄서야한다고!! 세팅");
        BT.SetIsLineUPTime(true);
    }
}
