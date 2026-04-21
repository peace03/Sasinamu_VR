using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

//지하철 스케쥴 관리
public class SubwayScheduler : MonoBehaviourPun
{
    [Header("지하철 사이클 설정")]
    [SerializeField] private SubwayCycle subwayCycle;
    [Tooltip("다음 지하철이 오는 사이클 시간")]
    [SerializeField] private float subwayCycleTime = 120f;  //지하철 사이클 시간
    public float SubwayCycleTime => subwayCycleTime;        //지하철 사이클 시간
    [Tooltip("지하철이 정차하는 시간")]
    [SerializeField] private float stopTime = 10f;          //지하철 정차 시간

    [Header("지하철 사운드 설정")]
    [SerializeField] private float entryCountdown;          //지하철 진입 카운트 다운

    public float CurrentTime { get; private set; }
    private bool nearSubway = false; //역에 지하철 있는가?
    private bool usingScheduler = true;    //스케쥴러 작동할 것인가?

    //지하철 사운드 관리
    private bool brodcastSoundPlay = false;         //지하철 진입 안내음 재생 중
    
    private void Update()
    {
        NoneShareSchedule();
    }

    //방장만 실행하는 스케쥴
    public void NoneShareSchedule()
    {
        //내가 방장이 아니면, 스케줄러의 시간 연산을 아예하지 않는다.
        if (!PhotonNetwork.IsMasterClient) return;
        //역에 지하철 없으면 다음 지하철 생성까지 카운트
        if (nearSubway == false && usingScheduler)
        {
            CurrentTime += Time.deltaTime;
            //지하철 진입음 재생
            if (CurrentTime >= subwayCycleTime - entryCountdown && brodcastSoundPlay == false)
            {
                brodcastSoundPlay = true;
                //subwayCycle.PlayEntryBroadCastSound();
            }
            //지하철 진입 대기
            if (CurrentTime >= subwayCycleTime)
            {
                brodcastSoundPlay = false;
                CurrentTime = 0f;
                subwayCycle.SetSubwayStatus(SubwayStatus.StandBy);
                nearSubway = true;
            }
        }
        else if (nearSubway == true && usingScheduler) //역에 지하철 있으면 지하철 루틴
        {
            //StandBy 상태면 Start
            if (subwayCycle.GetSubwayStatus() == SubwayStatus.StandBy)
            {
                subwayCycle.SetSubwayStatus(SubwayStatus.Start);
            }
            if (subwayCycle.GetSubwayStatus() == SubwayStatus.Arrive)
            {
                CurrentTime += Time.deltaTime;
                //정차 시간 종료되면 떠나기
                if (CurrentTime >= stopTime)
                {
                    subwayCycle.SetSubwayStatus(SubwayStatus.CloseDoor);
                    CurrentTime = 0f;
                    nearSubway = false;
                }
            }
        }
    }

    

    //스케쥴러 사용 안함 (UnityEvent 연결)
    public void DontUseScheduler()
    {
        usingScheduler = false;
        subwayCycle.SetSubwayStatus(SubwayStatus.StandBy);
    }
    //스케줄러 사용 (UnityEvent 연결)
    public void UseScheduler()
    {
        usingScheduler = true;
    }
}
