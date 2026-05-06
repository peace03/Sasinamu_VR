using Photon.Pun;
using System.Collections;
using UnityEngine;

public class StationInfo : ArrivalTrigger
{
    public const string UI_NAME_MAP = "Map";                // 노선도
    public const string UI_NAME_DETAIL = "Detail";          // 상세정보

    [Header("플레이어 손 이름")]
    [SerializeField] private string leftHandName = "LeftHand Controller";
    [SerializeField] private string rightHandName = "RightHand Controller";

    [Header("지하철 UI들")]
    [SerializeField] private StationMapUI mapUI;            // 노선도 UI
    [SerializeField] private StationDetailUI detailUI;      // 상세정보 UI

    [Header("가이드 UI")]
    [SerializeField] private GuidePopupUI guideUI;

    [Header("승강장 위치")]
    [SerializeField] private GameObject subwayDestination;

    private PhotonView platformPV;                          // 지하철 포톤뷰
    private PlayerID leftHand;                              // 왼손
    private PlayerID rightHand;                             // 오른손
    private PlatformWristUI wristUI;                        // 플레이어 손목 UI
    private ArrowPointer arrowPointer;                      // 방향 화살표
    private Coroutine closeCoroutine;                       // 닫기 코루틴

    private int? playerID = null;                           // 플레이어 ID

    private void Awake()
    {
        platformPV = transform.GetComponent<PhotonView>();
    }

    [PunRPC]
    // 플레이어 ID 설정 동기화 함수
    public void RPC_SyncOwner(int id)
    {
        // id가 -1이라면
        if (id == -1)
            // ID 초기화
            playerID = null;
        // id가 -1이 아니라면
        else
            // ID 저장하기
            playerID = id;
    }

    [PunRPC]
    // UI 변경 동기화 함수
    public void RPC_SyncChangeUI(string uiType, string buttonPath)
    {
        // UI 종류에 따라서
        switch (uiType)
        {
            // 노선도라면
            case UI_NAME_MAP:
                // 노선도 UI가 있고 열려있다면
                if (mapUI != null && mapUI.gameObject.activeSelf)
                    // 노선도 UI 변경 함수 실행
                    mapUI.ExecuteNetworkAction(buttonPath);
                break;
            // 상세정보라면
            case UI_NAME_DETAIL:
                // 상세정보 UI가 있고 열려있다면
                if (detailUI != null && detailUI.gameObject.activeSelf)
                    // 상세정보 UI 변경 함수 실행
                    detailUI.ExecuteNetworkAction(buttonPath);
                break;
        }
    }

    // 동기화 요청 함수
    public void RequestSync(string uiType, string buttonPath)
    {
        // 서버에 접속되어 있다면
        if (PhotonNetwork.InRoom)
            // UI 변경 동기화 함수 실행
            platformPV.RPC("RPC_SyncChangeUI", RpcTarget.AllBuffered, uiType, buttonPath);
        // 로컬 테스트라면
        else
        {
            Debug.Log($"오프라인 테스트 : 로컬에서 직접 UI 변경{buttonPath} 함수를 실행합니다.");
            RPC_SyncChangeUI(uiType, buttonPath);
        }
    }

    // 플레이어가 UI를 가리키고 있는지 판단하는 함수
    public bool IsPlayerUIHovering()
    {
        // 왼손이 비어있지 않고 플레이어의 ID와 일치하며 UI를 가리키고 있다면
        if (leftHand != null && leftHand.ID == playerID && leftHand.IsUIHovering)
            // 가리키고 있음
            return true;

        // 오른손이 비어있지 않고 플레이어의 ID와 일치하며 UI를 가리키고 있다면
        if (rightHand != null && rightHand.ID == playerID && rightHand.IsUIHovering)
            // 가리키고 있음
            return true;

        // 가리키고 있지 않음
        return false;
    }

    // LED 화면에 들어왔을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        var root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        var playerPV = root.GetComponentInChildren<PhotonView>();

        // 플레이어에 포톤뷰가 없거나, 내 캐릭터에서 발생한 상황이 아니거나, 서버에 접속되어 있는 상태에서 준비가 안된 캐릭터라면
        if (playerPV == null || !playerPV.IsMine || (PhotonNetwork.InRoom && playerPV.Owner == null))
            // 종료
            return;

        // 플레이어 ID가 비어있지 않다면
        if (playerID != null)
            // 종료
            return;

        // 가이드 닫기 연출이 진행 중이라면
        if (closeCoroutine != null)
        {
            // 가이드 닫기 연출 중지
            StopCoroutine(closeCoroutine);
            // 닫기 코루틴 변수 초기화
            closeCoroutine = null;

            // 가이드 UI가 있다면
            if (guideUI != null)
                // 가이드 UI 전부 초기화
                guideUI.ResetUI(true);
        }

        // 서버에 접속되어 있다면
        if (PhotonNetwork.InRoom)
            // 플레이어 ID 설정 동기화 함수 실행
            platformPV.RPC("RPC_SyncOwner", RpcTarget.AllBuffered, playerPV.Owner.ActorNumber);
        // 로컬 테스트라면
        else
        {
            Debug.Log("오프라인 테스트 : 로컬에서 직접 Owner(999) 설정 함수를 실행합니다.");
            RPC_SyncOwner(999);
        }

        // 플레이어 ID 컴포넌트를 가지고 있는 손 가져오기
        var ids = root.GetComponentsInChildren<PlayerID>();
        
        // 플레이어 ID 컴포넌트가 있다면
        if (ids != null)
        {
            // 컴포넌트 수 만큼
            foreach (var id in ids)
            {
                // 오브젝트 이름이 왼손과 같다면
                if (id.name == leftHandName)
                    // 왼손에 저장
                    leftHand = id;
                // 오른손과 같다면
                else if (id.name == rightHandName)
                    // 오른손에 저장
                    rightHand = id;
            }
        }

        // 손목 UI 저장
        wristUI = root.GetComponentInChildren<PlatformWristUI>();

        // 손목 UI가 있다면
        if (wristUI != null)
        {
            // 손목 UI 기능 막기
            wristUI.SetCanOpenUI(false);

            // 맵 UI가 있다면
            if (mapUI != null)
                // 손목 UI의 가이드 진행 상황에 따라서 맵 UI 버튼 기능 초기화
                mapUI.Init(wristUI.CurrentProgress);

            // 가이드 UI가 있다면
            if (guideUI != null)
            {
                // 손목 UI의 가이드 진행 상황에 따라서 가이드 UI 초기화
                guideUI.Init(wristUI.CurrentProgress);

                // 가이드 UI의 닫힌 위치가 손목이라면
                if (wristUI.CurrentProgress.currentStep == GuideState.Move)
                    // 가이드 UI의 현재 위치만 초기화
                    guideUI.ResetUI(false);

                // 가이드 UI 열기
                guideUI.PopupUIHandler(true);
                // 방향 화살표 저장
                arrowPointer = root.GetComponentInChildren<ArrowPointer>();

                // 방향 화살표가 있다면
                if (arrowPointer != null)
                    // 방향 화살표 숨기기
                    arrowPointer.ArrowPointerHandler(false);
            }
        }
    }

    // LED 화면에서 나갔을 때 실행되는 함수
    protected override void OnExited(GameObject player)
    {
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        var root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        var playerPV = root.GetComponentInChildren<PhotonView>();

        // 플레이어에 포톤뷰가 없거나, 내 캐릭터에서 발생한 상황이 아니라면
        if (playerPV == null || !playerPV.IsMine)
            // 종료
            return;

        // 서버에 접속되어 있는 상태라면
        if (PhotonNetwork.InRoom)
        {
            // 플레이어 ID가 없거나, 준비가 안된 캐릭터거나, 플레이어 ID와 나간 사람의 ID가 다르다면
            if (playerID == null || playerPV.Owner == null || playerID != playerPV.Owner?.ActorNumber)
                // 종료
                return;

            // 플레이어 ID 초기화 동기화 함수 실행
            platformPV.RPC("RPC_SyncOwner", RpcTarget.AllBuffered, -1);
        }
        // 로컬 테스트라면
        else
        {
            Debug.Log("오프라인 테스트 : 로컬에서 직접 Owner(초기화) 설정 함수를 실행합니다.");
            RPC_SyncOwner(-1);
        }

        // 왼손 초기화
        leftHand = null;
        // 오른손 초기화
        rightHand = null;

        // 상세정보 UI가 있고 열려있다면
        if (detailUI != null && detailUI.gameObject.activeSelf)
        {
            // 서버에 접속되어 있다면
            if (PhotonNetwork.InRoom)
                // 상세정보 UI 닫기 동기화 함수 실행
                platformPV.RPC("RPC_SyncChangeUI", RpcTarget.AllBuffered, UI_NAME_DETAIL, StationDetailUI.EXIT_PATH);
            // 로컬 테스트라면
            else
            {
                Debug.Log("오프라인 테스트 : 로컬에서 직접 UI 변경(초기화) 함수를 실행합니다.");
                RPC_SyncChangeUI(UI_NAME_DETAIL, StationDetailUI.EXIT_PATH);
            }
        }

        // 방향 화살표가 있다면
        if (arrowPointer != null)
        {
            // 방향 화살표 보여주기
            arrowPointer.ArrowPointerHandler(true);
            // 방향 화살표 초기화
            arrowPointer = null;
        }

        // 가이드 UI가 있다면
        if (guideUI != null)
        {
            // 가이드가 진행 중이였다면
            if (guideUI.GuideCoroutine != null)
                // 가이드 진행 중지
                guideUI.StopGuideUIRoutine();

            // 손목 UI가 없다면
            if (wristUI == null)
            {
                // 가이드 UI 닫기
                guideUI.PopupUIHandler(false);

                // 만약 플레이어 ID가 초기화가 안됐다면
                if (playerID != null)
                    // 플레이어 ID 초기화
                    playerID = null;

                // 종료
                return;
            }

            // 다음 목적지가 승강장이라면
            if (wristUI.CurrentProgress.currentStep == GuideState.Move)
            {
                // 손목 UI 변경
                wristUI.ChangeUI(PlatformWristType.Move);

                // 승강장 위치가 있다면
                if (subwayDestination != null)
                    // 승강장 위치 활성화
                    subwayDestination.SetActive(true);

                // 닫는 위치 설정하기
                guideUI.SetClosedPosition(wristUI.LeftHand);
                // 가이드 UI 닫기
                closeCoroutine = StartCoroutine(WaitForCloseGuideUI());
            }
            // 다음 목적지가 승강장이 아니라면
            else
            {
                // 가이드 UI 닫기
                guideUI.PopupUIHandler(false);

                // 손목 UI가 있다면
                if (wristUI != null)
                {
                    // 손목 UI 기능 풀기
                    wristUI.SetCanOpenUI(true);
                    // 손목 UI 초기화
                    wristUI = null;
                }

                // 만약 플레이어 ID가 초기화가 안됐다면
                if (playerID != null)
                    // 플레이어 ID 초기화
                    playerID = null;
            }
        }
    }

    // 가이드 UI 닫기 함수
    private IEnumerator WaitForCloseGuideUI()
    {
        // 가이드 UI 닫기
        guideUI.PopupUIHandler(false);
        // 가이드 UI 닫기 기다리기
        yield return new WaitForSeconds(guideUI.CloseDuration);
        
        // 가이드 UI가 있다면
        if (guideUI != null)
            // 닫는 위치 초기화
            guideUI.SetClosedPosition(guideUI.OpenedPos);

        // 손목 UI가 있다면
        if (wristUI != null)
        {
            // 손목 UI 기능 풀기
            wristUI.SetCanOpenUI(true);
            // 손목 UI 초기화
            wristUI = null;
        }

        // 만약 플레이어 ID가 초기화가 안됐다면
        if (playerID != null)
            // 플레이어 ID 초기화
            playerID = null;

        // 닫기 코루틴 변수 초기화
        closeCoroutine = null;
    }
}