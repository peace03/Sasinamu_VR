using UnityEngine;
using UnityEngine.Events;

//지하철 스케쥴 관리
public class SubwayScheduler : MonoBehaviour
{
    [SerializeField] private SubwayCycle subwayCycle;
    [Tooltip("다음 지하철이 오는 사이클 시간")]
    [SerializeField] private float subwayCycleTime = 120f; //지하철 사이클 시간
    public float SubwayCycleTime => subwayCycleTime;  //지하철 사이클 시간
    [Tooltip("지하철이 정차하는 시간")]
    [SerializeField] private float stopTime = 10f;          //지하철 정차 시간

    private float currentTime = 0f;
    private bool nearSubway = false; //역에 지하철 있는가?
    private bool usingScheduler = true;    //스케쥴러 작동할 것인가?
    
    private void Update()
    {
        //역에 지하철 없으면 다음 지하철 생성까지 카운트
        if (nearSubway == false && usingScheduler)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= subwayCycleTime)
            {
                currentTime = 0f;
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
                currentTime += Time.deltaTime;
                //정차 시간 종료되면 떠나기
                if (currentTime >= stopTime)
                {
                    subwayCycle.SetSubwayStatus(SubwayStatus.CloseDoor);
                    currentTime = 0f;
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
