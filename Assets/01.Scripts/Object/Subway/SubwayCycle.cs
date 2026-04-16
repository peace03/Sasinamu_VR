using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
//C# 기본 System.Collections.Hashtable과의 충돌을 막기 위해 Photon 전용 해시테이블을 명시합니다.
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum SubwayStatus
{
    StandBy,    //대기
    Start,      //출발
    Arrive,     //정차
    CloseDoor,  //문닫기
    Leave       //떠나기
}

public class SubwayCycle : MonoBehaviourPunCallbacks
{
    [Header("좌표")]
    [SerializeField] private Transform StartPoint;  //시작 포인트
    [SerializeField] private Transform StopPoint;   //정차 포인트
    [SerializeField] private Transform EndPoint;    //끝 포인트

    [Header("이동 설정")]
    [SerializeField] private float maxSpeed;        //최고속도
    [Tooltip("1초당 증가할 가속도")]
    [SerializeField] private float acceleration;    //1초당 증가할 가속도

    [Header("지하철 사이클 이벤트")]
    [SerializeField] private UnityEvent<int[]> OnPassengerReset;    //지하철 탑승객 인원 리셋 이벤트
    [SerializeField] private UnityEvent OnArrive;                   //지하철 역 도착시 이벤트 발행
    [SerializeField] private UnityEvent OnCloseDoor;                    //지하철 역 떠날시 이벤트 발행

    private SubwayPassenserLevel passengerLevel; //탑승객 관리자
    private float currentSpeed = 0f;                //현재속도
    private int[] passenserCount;                   //탑승객 분포 리스트
    private bool isCorouting = false;

    private SubwayStatus status = SubwayStatus.StandBy;

    private void Awake()
    {
        passengerLevel = GetComponent<SubwayPassenserLevel>();
    }

    // 💡 핵심: 방에 완벽하게 입장(네트워크 연결 100% 완료)했을 때 엔진이 자동으로 호출해 줍니다.
    public override void OnJoinedRoom()
    {
        // 💡 1. 마스터 클라이언트: 시작하자마자 1회차 데이터를 무조건 서버에 기록합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            passenserCount = passengerLevel.ResetCount();
            Hashtable hash = new Hashtable();
            hash.Add("PassengerCount", passenserCount);
            hash.Add("IsBoarding", true); // 탑승 중(전광판 켜짐) 상태 등록
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
        // 💡 2. 늦게 들어온 일반 참가자: 방에 들어오자마자 서버 기록을 읽고 맞춥니다.
        else if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsBoarding"))
            {
                bool isBoarding = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsBoarding"];

                // 문이 열려있으면 숫자를 표시
                if (isBoarding && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PassengerCount"))
                {
                    int[] savedCounts = (int[])PhotonNetwork.CurrentRoom.CustomProperties["PassengerCount"];
                    OnPassengerReset?.Invoke(savedCounts);
                }
                // 문이 닫혀있으면 전광판 끄기
                else if (!isBoarding)
                {
                    OnCloseDoor?.Invoke();
                }
            }
        }
    }

    private void Update()
    {
        //방장이 아니면 아무것도 하지 않음
        if (!PhotonNetwork.IsMasterClient) return;

        switch (status)
        {
            case SubwayStatus.StandBy:
                //시작 포인트로 위치 초기화
                if (transform.position != StartPoint.position)
                {
                    transform.position = StartPoint.position;
                    passenserCount = passengerLevel.ResetCount(); //탑승객 리스트 리셋
                    //서버의 화이트 보드에 뽑아낸 숫자 덮어쓰기
                    Hashtable hash = new Hashtable();
                    hash.Add("PassengerCount", passenserCount);
                    hash.Add("IsBoarding", true);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                    currentSpeed = 0f;

                    //OnPassengerReset?.Invoke(passenserCount);
                    //Debug.Log($"탑승객\n1호:{passenserCount[0]}    2호:{passenserCount[1]}    3호:{passenserCount[2]}");
                    //currentSpeed = 0f;
                    //
                    ////다른 컴퓨터에도 똑같은 숫자로 인원수 변경
                    //photonView.RPC("SyncPassengerCountRPC", RpcTarget.Others, passenserCount);
                }
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
                    EventBus<SubwayArrive>.Publish(default);
                }
                break;
            case SubwayStatus.Arrive:
                OnArrive?.Invoke();
                //Debug.Log("Arrive");
                break;
            case SubwayStatus.CloseDoor:
                if (!isCorouting)
                {
                    StopAllCoroutines();
                    StartCoroutine(ClosingDoor());
                    isCorouting = true;
                }
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
        
        // 🚨 기존 코드 삭제: if (status == SubwayStatus.CloseDoor) OnCloseDoor?.Invoke();
        
        // 💡 3. 방장이 상태를 CloseDoor로 바꿀 때 혼자 이벤트를 실행하지 않고, 서버에 '문 닫힘(False)'을 기록합니다.
        if (PhotonNetwork.IsMasterClient && status == SubwayStatus.CloseDoor)
        {
            Hashtable hash = new Hashtable();
            hash.Add("IsBoarding", false);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    private IEnumerator ClosingDoor()
    {
        yield return new WaitForSeconds(5f);
        status = SubwayStatus.Leave;
        isCorouting = false;
    }

    // [모두를 위한 처리]
    // 방장이 SetCustomProperties로 화이트보드를 수정하는 순간, 방에 있는 모든 사람(방장 본인 포함)이 이 함수를 자동 실행합니다.
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("IsBoarding") || propertiesThatChanged.ContainsKey("PassengerCount"))
        {
            // 방의 '현재 최종 상태'를 통째로 읽어와서 적용합니다.
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsBoarding"))
            {
                bool isBoarding = (bool)propertiesThatChanged["IsBoarding"];

                if (isBoarding)
                {
                    // IsBoarding이 True로 바뀌면 (지하철 역 도착 시) -> 숫자 표시
                    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("PassengerCount"))
                    {
                        int[] syncedCounts = (int[])PhotonNetwork.CurrentRoom.CustomProperties["PassengerCount"];
                        OnPassengerReset?.Invoke(syncedCounts);
                    }
                }
                else
                {
                    // IsBoarding이 False로 바뀌면 (지하철 문 닫힐 시) -> 모든 컴퓨터에서 전광판을 StandBy 모드로 변경!
                    OnCloseDoor?.Invoke();
                }
            }
        }
    }

    //[PunRPC]
    //public void SyncPassengerCountRPC(int[] syncedCounts)
    //{
    //    Debug.Log("방장으로부터 혼잡도 데이터 받음");
    //    //내 컴퓨터 혼잡도 표시
    //    OnPassengerReset?.Invoke(syncedCounts);
    //}
}
