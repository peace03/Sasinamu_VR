using UnityEngine;
using UnityEngine.Events;

public enum SubwayStatus
{
    StandBy,//대기
    Start,  //출발
    Arrive, //정차
    Leave   //떠나기
}

public class SubwayCycle : MonoBehaviour
{
    [Header("좌표")]
    [SerializeField] private Transform StartPoint;  //시작 포인트
    [SerializeField] private Transform StopPoint;   //정차 포인트
    [SerializeField] private Transform EndPoint;    //끝 포인트

    [Header("이동 설정")]
    [SerializeField] private float maxSpeed;        //최고속도
    [Tooltip("1초당 증가할 가속도")]
    [SerializeField] private float acceleration;    //1초당 증가할 가속도

    [Header("지하철 탑승객 인원 리셋 이벤트")]
    [SerializeField] private UnityEvent<int[]> OnPassengerReset;   //지하철 탑승객 인원 리셋 이벤트
    [SerializeField] private UnityEvent OnLeave;   //지하철 탑승객 인원 리셋 이벤트

    private SubwayPassenserLevel passengerLevel; //탑승객 관리자
    private float currentSpeed = 0f;                //현재속도
    private int[] passenserCount;                   //탑승객 분포 리스트

    private SubwayStatus status = SubwayStatus.StandBy;

    private void Awake()
    {
        passengerLevel = GetComponent<SubwayPassenserLevel>();
    }

    private void Update()
    {
        switch (status)
        {
            case SubwayStatus.StandBy:
                //시작 포인트로 위치 초기화
                if (transform.position != StartPoint.position)
                {
                    transform.position = StartPoint.position;
                    passenserCount = passengerLevel.ResetCount(); //탑승객 리스트 리셋
                    OnPassengerReset?.Invoke(passenserCount);
                    Debug.Log($"탑승객\n1호:{passenserCount[0]}    2호:{passenserCount[1]}    3호:{passenserCount[2]}");
                }
                currentSpeed = 0f;
                //Debug.Log("StandBy");
                break;
            case SubwayStatus.Start:
                //정차 지점까지 이동
                //Debug.Log("Start");
                if (Vector3.Distance(StopPoint.position, transform.position) >= 0.1) Move(StopPoint.position);
                //도착
                else
                {
                    currentSpeed = 0f;
                    status = SubwayStatus.Arrive;
                }
                break;
            case SubwayStatus.Arrive:
                //Debug.Log("Arrive");
                break;
            case SubwayStatus.Leave:
                //종착지까지 이동
                //Debug.Log("Leave");
                if (Vector3.Distance(EndPoint.position, transform.position) >= 0.1) Move(EndPoint.position);
                //이동 끝나면 처음으로
                else 
                {
                    currentSpeed = 0f;
                    status = SubwayStatus.StandBy;
                }
                break;
        }
    }

    public void Move(Vector3 targetPoint)
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPoint, currentSpeed * Time.deltaTime);
    }

    //상태 Getter
    public SubwayStatus GetSubwayStatus() { return status; }
    //상태 Setter
    public void SetSubwayStatus(SubwayStatus status)
    {
        this.status = status;
        if (status == SubwayStatus.Leave) OnLeave?.Invoke();
    }
}
