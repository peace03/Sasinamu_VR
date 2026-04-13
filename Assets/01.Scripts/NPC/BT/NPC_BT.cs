using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPC_BT : MonoBehaviour
{
    [SerializeField] private List<Transform> gatePos; //지하철 게이트 줄서는 위치
    [SerializeField] private float speed = 4;

    private NavMeshAgent agent;
    private LineUpManager lineUpManager;
    private BT_Node root;
    private bool isLineUpTime = false;  //줄서야할 시간인지

    private Vector3 lineUpTarget; //줄서는 목표 좌표
    private Vector3 walkTarget;     //걷는 목표 좌표
    private Vector3 defaultVector = new Vector3(0f, 0f, 0f);

    private float CurrentTime = 0f;
    private float IdleEndTime = 0f; //Idle상태 유지 시간
    private float IdleStartTime = 0f;   //Idle 시작 시간
    private bool isIdle = true;     //Idle 상태인가?

    private void OnDisable()
    {
        //초기화
        isLineUpTime = false;
        lineUpTarget = defaultVector;
        walkTarget = defaultVector;
        CurrentTime = 0f;
    }
    private void Awake()
    {
        //초기화
        agent = GetComponent<NavMeshAgent>();
        lineUpManager = FindAnyObjectByType<LineUpManager>();
        lineUpTarget = defaultVector;
        walkTarget = defaultVector;
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
                new BT_Leaf(MoveLineUp) //줄서기
                //지하철 문 열리면 탑승
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
                agent.SetDestination(lineUpTarget); //NavMesh 사용
            }
            if (Vector3.Distance(transform.position, lineUpTarget) <= 0.01f) //오차범위 내에있으면 그만 이동
                return BT_NodeStatus.Success;
            return BT_NodeStatus.Running;
        }
        else return BT_NodeStatus.Failure;
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
            Debug.Log("걷기 시작");
        }
        //도착하면 성공
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isIdle = true;
            Debug.Log("걷기 종료");
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
            Debug.Log("대기 시작");
        }
        if (IdleEndTime <= CurrentTime) //Idle 시간 끝
        {
            //걷기 목표 좌표 초기화
            walkTarget = defaultVector;
            //Idle종료 초기화
            IdleEndTime = 0f;
            isIdle = false;
            Debug.Log("대기 종료");
            return BT_NodeStatus.Failure;
        }

        //Idle 애니메이션으로 변경
        return BT_NodeStatus.Success;
    }

    private void Move(Vector3 target)
    {
        //걷는 애니메이션 변환
        //Vector3 direction = (target - transform.position).normalized;
        //direction.y = 0;
        //transform.forward = direction;
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetIsLineUPTime(bool isTime)
    {
        isLineUpTime = isTime;
    }
}
