using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    //알람 데이터를 담을 구조체
    private struct Alarm
    {
        public float TriggerTime;
        public Action Callback;
    }

    //알람 명부 리스트
    private List<Alarm> alarmQueue = new List<Alarm>();
    public float CurrentTime { get; private set; } = 0f;

    [SerializeField] private SubwayScheduler subwayScheduler;
    //private float subwayCycleTime = 120f;

    [Header("게임 시작시 플레이어 대기 시간")]
    [SerializeField] private LoadScene currentScene;        //현재 씬
    [SerializeField] private float onlyMovePauseTime;       //움직임 정지 시간

    private void Start()
    {
        //플레이어 움직임 정지
        if (currentScene == LoadScene.StartScene)
            StartCoroutine(StandBy());
    }

    private void OnEnable()
    {
        EventBus<OnSelfInstantiate>.OnEvent += StartStandBy;
    }
    private void OnDisable()
    {
        EventBus<OnSelfInstantiate>.OnEvent -= StartStandBy;
    }

    private void Update()
    {
        //Debug.Log(CurrentTime);
        //알람큐에 액션이 등록되어있고
        //NPC의 줄서는 시간이 되었으면 실행
        while (alarmQueue.Count > 0 && alarmQueue[alarmQueue.Count - 1].TriggerTime <= subwayScheduler.CurrentTime)
        {
            int lastIndex = alarmQueue.Count - 1;
            //마지막 인덱스 줄서기 실행
            alarmQueue[lastIndex].Callback?.Invoke();
            //실행 후 리스트에서 지워주기
            alarmQueue.RemoveAt(lastIndex);
            //Debug.Log("줄서기 알람 발송 완료");
        }
    }

    private void StartStandBy(OnSelfInstantiate _)
    {
        StartCoroutine(StandBy());
    }
    private IEnumerator StandBy()   //처음 시작하고 대기할 때(고개만 움직일 수 있음)
    {
        Debug.Log("플레이어 대기");
        EventBus<OnEnterStandbyStart>.Publish(default);
        yield return new WaitForSeconds(onlyMovePauseTime);
        EventBus<OnEnterStandbyEnd>.Publish(default);
        Debug.Log("플레이어 대기 종료");
    }

    //NPC 알람 등록 메서드
    public void RegisterAlarm(float targetTime, Action callback)
    {
        Alarm newAlarm = new Alarm { TriggerTime = targetTime, Callback = callback };
        alarmQueue.Add(newAlarm);

        //내림차순 정렬 (가장 작은 시간 값이 마지막에 오도록)
        alarmQueue.Sort((a, b) => b.TriggerTime.CompareTo(a.TriggerTime));
    }

    //NPC 알람 초기화 메서드
    public void RegisterAlarmReset()
    {
        //Debug.Log("알람 리셋");
        alarmQueue.Clear();
    }
}
