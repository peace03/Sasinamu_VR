using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPC_BT : MonoBehaviour
{
    private List<Transform> gatePos; //지하철 게이트 줄서는 위치
    [Header("NPC")]
    [SerializeField] private float speed = 4;

    private Transform subway;
    private NavMeshAgent agent;
    private LineUpManager lineUpManager;
    private BT_Node root;
    private bool isLineUpTime = false;  //줄서야할 시간인지

    private Vector3 lineUpTarget; //줄서는 목표 좌표
    private Vector3 walkTarget;     //걷는 목표 좌표
    private Vector3 defaultVector = new Vector3(0f, 0f, 0f);
    private Vector3 onBoardTarget;  //탑승 목표 좌표
    private Vector3 onSeatTarget;   //자리 목표 좌표
    private bool isBoarded = false;         //탑승 했는가?
    private bool isOpenDoor = false;

    [Header("이동 위치")]
    [SerializeField] private float boardPos = 4f;   //지하철 올라탈 거리
    [SerializeField] private float seatPosRange = 3f;//지하철 자리 배치 범위

    private float CurrentTime = 0f;
    private float IdleEndTime = 0f; //Idle상태 유지 시간
    private float IdleStartTime = 0f;   //Idle 시작 시간
    private bool isIdle = true;     //Idle 상태인가?

    private void OnEnable()
    {
        EventBus<SubwayArrive>.OnEvent += SetIsBoardedTrue;
    }

    private void OnDisable()
    {
        EventBus<SubwayArrive>.OnEvent -= SetIsBoardedTrue;
        isOpenDoor = false;
    }

    public void OnCreate(List<Transform> gatePos, Transform subway)
    {
        //초기화
        this.gatePos = gatePos;
        this.subway = subway;

        agent = GetComponent<NavMeshAgent>();
        lineUpManager = FindAnyObjectByType<LineUpManager>();
        lineUpTarget = defaultVector;
        walkTarget = defaultVector;
    }
    public void OnSpawn(Transform parent, Vector3 spawnPos)
    {
        transform.SetParent(parent);
        agent.Warp(spawnPos);
        
        //초기화
        agent.enabled = true;
        isLineUpTime = false;
        isBoarded = false;
        lineUpTarget = defaultVector;
        walkTarget = defaultVector;
        CurrentTime = 0f;
        IdleStartTime = 0f;
        IdleEndTime = 0f;
    }

    private void Start()
    {

        //(랜덤시간)Idle
        //(지하철 오기 랜덤 전 시간) 랜덤 gate로 이동
        //지하철 문 열리면 안으로 이동
        root = new BT_Selector(new List<BT_Node>
        {
            //지하철 오기 전 줄서기
            new BT_Sequence(new List<BT_Node>
            {
                new BT_Leaf(MoveLineUp),    //줄서기
                new BT_Leaf(BoardSubway),   //지하철 문 열리면 탑승
                new BT_Leaf(ChooseSeat)     //자리 찾아가기
            }),
            //걷기
            new BT_Sequence(new List<BT_Node>
            {
                new BT_Leaf(Walk)
            }),
            new BT_Leaf(Idle)   //대기
        });
    }

    private void Update()
    {
        CurrentTime += Time.deltaTime;
        root.Evaluate();
    }

    //줄 서기
    private BT_NodeStatus MoveLineUp()
    {
        //Debug.Log("줄서기 BT 호출 완료");
        if (isLineUpTime == true)
        {
            //Debug.Log("줄서기 시작!!");
            //랜덤 타겟 설정
            if (lineUpTarget == defaultVector)
            {
                int gateIndex = Random.Range(0, gatePos.Count);
                //타겟 한번만 초기화
                lineUpTarget = lineUpManager.GetStaticLineUpPos(gateIndex, gatePos[gateIndex].position, gatePos[gateIndex].forward);
                onBoardTarget = gatePos[gateIndex].position + new Vector3(0f, 0f, boardPos); //지하철 탑승 위치 초기화
                agent.SetDestination(lineUpTarget); //NavMesh 사용
            }
            if (!agent.enabled || !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) //오차범위 내에있으면 그만 이동
            {
                //Debug.Log("줄서기 끝!!");
                return BT_NodeStatus.Success;
            }
            return BT_NodeStatus.Running;
        }
        else return BT_NodeStatus.Failure;
    }

    //지하철 탑승
    private BT_NodeStatus BoardSubway()
    {
        //Debug.Log("지하철 탑승 BT 호출 완료");
        if (agent.enabled) agent.enabled = false;
        if (isBoarded || Vector3.Distance(transform.position, onBoardTarget) <= 0.2)
        {
            //1회만 실행 부모가 안 바꼈을 시
            if (transform.parent != subway)
            {
                //지하철 줄 초기화
                lineUpManager.ResetAllLine();
                //지하철을 부모 오브젝트로 수정
                transform.SetParent(subway, true);
                //자리 선택
                onSeatTarget = 
                    transform.localPosition + new Vector3(Random.Range(-3f, 3f), 0f, 0f);
                isBoarded = true;
            }
            return BT_NodeStatus.Success;
        }
        if (isOpenDoor) Move(onBoardTarget, false);
        return BT_NodeStatus.Running;
    }

    //지하철 자리로 이동
    private BT_NodeStatus ChooseSeat()
    {
        if (Vector3.Distance(transform.localPosition, onSeatTarget) <= 0.2f)
            return BT_NodeStatus.Success;
        //Debug.Log("지하철 자리이동 BT 호출 완료");
        Move(onSeatTarget, true);
        return BT_NodeStatus.Running;
    }

    //걷기
    private BT_NodeStatus Walk()
    {
        //Idle 상태면 넘어가기
        if (isIdle == true) return BT_NodeStatus.Failure;
        //랜덤 타겟 설정
        if (walkTarget == defaultVector)
        {
            walkTarget = new Vector3(Random.Range(-9, 9), 0, Random.Range(-9, 9));
            agent.SetDestination(walkTarget); //NavMesh 사용
            //Debug.Log("걷기 시작");
        }
        //도착하면 성공
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isIdle = true;
            //Debug.Log("걷기 종료");
            return BT_NodeStatus.Success;
        }
        return BT_NodeStatus.Running;
    }

    private BT_NodeStatus Idle()
    {
        if (IdleEndTime == 0f)  //Idle 시간 시작
        {
            IdleStartTime = CurrentTime;
            IdleEndTime = Random.Range(3f, 5f) + IdleStartTime;
            isIdle = true;
            //Debug.Log("대기 시작");
        }
        if (IdleEndTime <= CurrentTime) //Idle 시간 끝
        {
            //걷기 목표 좌표 초기화
            walkTarget = defaultVector;
            //Idle종료 초기화
            IdleEndTime = 0f;
            isIdle = false;
            //Debug.Log("대기 종료");
            return BT_NodeStatus.Failure;
        }

        //Idle 애니메이션으로 변경
        return BT_NodeStatus.Success;
    }

    private void Move(Vector3 target, bool isLocal)
    {
        //걷는 애니메이션 변환
        Vector3 direction = (target - (isLocal ? transform.localPosition : transform.position)).normalized;
        direction.y = 0;
        transform.forward = direction;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetIsLineUPTime(bool isTime) => isLineUpTime = isTime;
    public void SetIsBoardedTrue(SubwayArrive _) => isOpenDoor = true;

}
