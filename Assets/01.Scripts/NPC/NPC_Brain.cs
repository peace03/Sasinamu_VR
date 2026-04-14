using UnityEngine;

public class NPC_Brain : MonoBehaviour
{
    [SerializeField] private SubwayScheduler subwayScheduler;
    [SerializeField] private NPC_BT BT;
    private TimeManager timeManager;

    private void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();

    }

    private void Start()
    {
        float subwayArrivalTime = subwayScheduler.SubwayCycleTime;
        //지하철 도착 몇분 전
        float randomWaitTime = Random.Range(5f, subwayArrivalTime-5f);
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
