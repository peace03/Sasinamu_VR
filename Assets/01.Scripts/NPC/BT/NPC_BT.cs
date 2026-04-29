using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.Events;
using Unity.VisualScripting;

public class NPC_BT : MonoBehaviourPun
{
    [Header("NPC 설정")]
    [SerializeField] private float speed = 4;

    [Header("이동 위치")]
    [SerializeField] private float boardPos = 4f;   //지하철 올라탈 거리
    [SerializeField] private float seatPosRange = 3f;//지하철 자리 배치 범위

    private Animator anim;              //애니메이션
    private CharacterController cc;     //속도로 애니메이션 재생
    private BoxCollider groundPos;      //이동 가능 범위
    private List<Transform> gatePos;    //지하철 게이트 줄서는 위치
    private Transform subway;           //NPC 부모 변경용
    private NavMeshAgent agent;
    private LineUpManager lineUpManager;
    private BT_Node root;
    private bool isLineUpTime = false;  //줄서야할 시간인지

    private Vector3 lineUpTarget;       //줄서는 목표 좌표
    private Vector3 walkTarget;         //걷는 목표 좌표
    private Vector3 defaultVector = new Vector3(0f, 0f, 0f);
    private Vector3 onBoardTarget;      //탑승 목표 좌표
    private Vector3 onSeatTarget;       //자리 목표 좌표
    private Vector3 decreaseGatePos = new Vector3(0f, 0f, 1f);  //게이트 줄서는 좌표 계산용
    private bool isBoarded = false;     //탑승 했는가?
    private bool isOpenDoor = false;

    private float CurrentTime = 0f;
    private float IdleEndTime = 0f; //Idle상태 유지 시간
    private float IdleStartTime = 0f;   //Idle 시작 시간
    private bool isIdle = true;     //Idle 상태인가?

    private Vector3 lastPosition;
    private float smoothedVelocity;

    private void OnEnable()
    {
        EventBus<SubwayArrive>.OnEvent += SetIsBoardedTrue;
    }

    private void OnDisable()
    {
        EventBus<SubwayArrive>.OnEvent -= SetIsBoardedTrue;
        isOpenDoor = false;
    }

    public void OnCreate(Transform subway, BoxCollider groundPos, List<Transform> gatePos)
    {
        //초기화
        this.gatePos = gatePos;
        this.subway = subway;
        this.groundPos = groundPos;

        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        lineUpManager = FindAnyObjectByType<LineUpManager>();
        lineUpTarget = defaultVector;
        walkTarget = defaultVector;
    }
    public void OnSpawn(Transform parent, Vector3 spawnPos)
    {
        transform.SetParent(parent);

        //방장만 NavMesh 켬. 참가자는 껍데기만 움직여야함
        if (PhotonNetwork.IsMasterClient)
        {
            agent.enabled = false;
            cc.enabled = false;


            transform.position = spawnPos;

            agent.enabled = true;
            agent.Warp(spawnPos);
            //cc 비활성으로 인한 고스트velocity 초기화
            cc.enabled = true;
            cc.Move(Vector3.forward * 0.0001f);
        }
        else
        {
            agent.enabled = false;
            cc.enabled = false;
            transform.position = spawnPos;
        }

        //초기화
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

        //방장이 아니면 BT 정지
        if (!PhotonNetwork.IsMasterClient) return;

        //애니메이션 재생
        Vector3 movementDelta = transform.position - lastPosition;
        movementDelta.y = 0f;

        float currentSpeed = movementDelta.magnitude / Time.deltaTime;

        smoothedVelocity = Mathf.Lerp(smoothedVelocity, currentSpeed, Time.deltaTime * 15);

        if (smoothedVelocity < 0.1f) smoothedVelocity = 0f;

        anim.SetFloat("Speed", smoothedVelocity);
        lastPosition = transform.position;

        //방장 위임 처리: 참가자 시절 agent가 꺼져있던 경우 다시 켜줌
        //단, 이미 지하철에 탑승해서 고의로 끈 상태가 아닐때만 작동
        if (!agent.enabled && !isBoarded && !isOpenDoor && transform.parent != subway)
        {
            //Debug.Log("agent on");
            agent.enabled = true;
            //현재 위치 내비메시 동기화
            agent.Warp(transform.position);
        }
        //안전성: 에이전트가 켜져있어도 네비메시 위에 없으면 연산 정지
        if (agent.enabled && !agent.isOnNavMesh) return;

        CurrentTime += Time.deltaTime;
        root.Evaluate();
    }

    //줄 서기
    private BT_NodeStatus MoveLineUp()
    {
        //에이전트가 꺼져있으면 다음 노드로
        if (!agent.enabled || !agent.isOnNavMesh) return BT_NodeStatus.Success;

        //Debug.Log("줄서기 BT 호출 완료");
        if (isLineUpTime == true)
        {
            //Debug.Log("줄서기 시작!!");
            //랜덤 타겟 설정
            if (lineUpTarget == defaultVector)
            {
                int gateIndex = Random.Range(0, gatePos.Count);
                //Debug.Log($"NPC {photonView.ViewID}번이 선택한 게이트: {gateIndex}번, 좌표: {gatePos[gateIndex].position}");
                //타겟 한번만 초기화
                lineUpTarget = lineUpManager.GetStaticLineUpPos(gateIndex, gatePos[gateIndex].position - decreaseGatePos, gatePos[gateIndex].forward);
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
        if (agent.enabled) { agent.enabled = false; }
            if (isBoarded || Vector3.Distance(transform.position, onBoardTarget) <= 0.2)
        {
            //1회만 실행 부모가 안 바꼈을 시
            if (transform.parent != subway)
            {
                //지하철 줄 초기화
                lineUpManager.ResetAllLine();
                //지하철을 부모 오브젝트로 수정
                photonView.RPC("SyncSetParentRPC", RpcTarget.All);
                //자리 선택
                onSeatTarget = 
                    transform.localPosition + new Vector3(Random.Range(-seatPosRange, seatPosRange), 0f, 0f);
                isBoarded = true;
                //Debug.Log($"{photonView.ViewID} BoardSubway 실행 되는 중");
            }
            return BT_NodeStatus.Success;
        }
        if (isOpenDoor) Move(onBoardTarget, false);
        return BT_NodeStatus.Running;
    }

    //지하철 자리로 이동
    private BT_NodeStatus ChooseSeat()
    {
        //도착하면 성공반환
        if (Vector3.Distance(transform.localPosition, onSeatTarget) <= 0.2f)
        {
            if (cc.enabled) cc.enabled = false;
            return BT_NodeStatus.Success;
        }
        //방어로직
        if (!cc.enabled) cc.enabled = true;
        //이동 로직
        Move(onSeatTarget, true);
        return BT_NodeStatus.Running;
    }

    //걷기
    private BT_NodeStatus Walk()
    {
        //Idle 상태면 넘어가기
        if (isIdle == true) return BT_NodeStatus.Failure;
        //agent가 비정상 상태면 길찾기 실패 반환
        if (!agent.enabled || !agent.isOnNavMesh) return BT_NodeStatus.Failure;

        //랜덤 타겟 설정
        if (walkTarget == defaultVector)
        {
            walkTarget = GetRandomTargetPos();
            agent.SetDestination(walkTarget); //NavMesh 사용
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
        //목표지점까지의 방향 계산
        Vector3 currentPos = isLocal ? transform.localPosition : transform.position;
        Vector3 direction = (target - currentPos).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            //캐릭터 회전
            if (isLocal) transform.localRotation = Quaternion.LookRotation(direction);
            else transform.rotation = Quaternion.LookRotation(direction);
        }

        //캐릭터 회전으로 방향을 맞춰두었기 때문에 transform.forward사용하면 됨
        cc.Move(transform.forward * speed * Time.deltaTime);
    }

    //이동범위 내 NavMesh 위치 반환
    private Vector3 GetRandomTargetPos()
    {
        Bounds bounds = groundPos.bounds;
        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 randomPos = new Vector3(x, y, z);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, 1f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }

    public void SetIsLineUPTime(bool isTime) => isLineUpTime = isTime;
    public void SetIsBoardedTrue(SubwayArrive _) => isOpenDoor = true;

    //방장이 RPC를 쏘면 모든 클라이언트의 메모리에서 이 함수가 동시에 실행됨
    [PunRPC]
    public void SyncSetParentRPC()
    {
        //참가자들의 NPC도 지하철의 자식 오브젝트로 들어감
        transform.SetParent(subway, true);
        //Debug.Log($"{photonView.ViewID} 네트워크 NPC 부모 변경 완료!");
    }
}
