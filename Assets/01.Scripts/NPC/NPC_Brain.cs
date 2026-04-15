using UnityEngine;
using UnityEngine.Pool;

public class NPC_Brain : MonoBehaviour
{
    [SerializeField] private NPC_BT BT;
    private TimeManager timeManager;
    private IObjectPool<NPC_Brain> _poolManager;

    private void Awake()
    {
        timeManager = FindAnyObjectByType<TimeManager>();
    }

    public void OnSpawn(SubwayScheduler subwayScheduler)
    {
        float subwayArrivalTime = subwayScheduler.SubwayCycleTime;
        //지하철 도착 몇분 전
        float randomWaitTime = Random.Range(5f, subwayArrivalTime-5f);
        float targetAlarmTime = subwayArrivalTime - randomWaitTime;
        timeManager.RegisterAlarm(targetAlarmTime, OnLineUpAlarmTriggered);
        Debug.Log("알람 울릴 시간: "+targetAlarmTime);
    }

    public void SetPoolManager(IObjectPool<NPC_Brain> pool)
    {
        _poolManager = pool;
    }

    private void OnLineUpAlarmTriggered()
    {
        Debug.Log("줄서야한다고!! 세팅");
        BT.SetIsLineUPTime(true);
    }

}
