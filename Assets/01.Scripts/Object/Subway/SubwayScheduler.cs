using UnityEngine;
using UnityEngine.Events;

//지하철 스케쥴 관리
public class SubwayScheduler : MonoBehaviour
{
    [SerializeField] private SubwayCycle subwayMove;
    [Tooltip("다음 지하철이 오는 사이클 시간")]
    [SerializeField] private float subwayCycleTime = 120f; //지하철 사이클 시간
    [Tooltip("지하철이 정차하는 시간")]
    [SerializeField] private float stopTime = 10f;          //지하철 정차 시간

    private float currentTime = 0f;
    private bool nearSubway = false; //역에 지하철 있는가?
    
    private void Update()
    {
        //역에 지하철 없으면 다음 지하철 생성까지 카운트
        if (nearSubway == false)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= subwayCycleTime)
            {
                currentTime = 0f;
                subwayMove.SetSubwayStatus(SubwayStatus.StandBy);
                nearSubway = true;
            }
        }
        else //역에 지하철 있으면 지하철 루틴
        {
            //StandBy 상태면 Start
            if (subwayMove.GetSubwayStatus() == SubwayStatus.StandBy)
            {
                subwayMove.SetSubwayStatus(SubwayStatus.Start);
            }
            if (subwayMove.GetSubwayStatus() == SubwayStatus.Arrive)
            {
                currentTime += Time.deltaTime;
                //정차 시간 종료되면 떠나기
                if (currentTime >= stopTime)
                {
                    subwayMove.SetSubwayStatus(SubwayStatus.Leave);
                    currentTime = 0f;
                    nearSubway = false;
                }
            }
        }
    }
}
