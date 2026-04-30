using Photon.Pun;
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

    private PhotonView platformPV;                          // 지하철 포톤뷰
    private PlayerID leftHand;                              // 왼손
    private PlayerID rightHand;                             // 오른손
    private PlatformWristUI wristUI;                        // 플레이어 손목 UI

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
        // 왼손이 비어있지 않고 UI를 가리키고 있다면
        if (leftHand != null && leftHand.IsUIHovering)
            // 가리키고 있음
            return true;

        // 오른손이 비어있지 않고 UI를 가리키고 있다면
        if (rightHand != null && rightHand.IsUIHovering)
            // 가리키고 있음
            return true;

        // 가리키고 있지 않음
        return false;
    }

    // LED 화면에 도착했을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        Transform root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        PhotonView playerPV = root.GetComponentInChildren<PhotonView>();

        // 플레이어에 포톤뷰가 없거나, 내 캐릭터에서 발생한 상황이 아니거나, 서버에 접속되어 있는 상태에서 준비가 안된 캐릭터라면
        if (playerPV == null || !playerPV.IsMine || (PhotonNetwork.InRoom && playerPV.Owner == null))
            // 종료
            return;

        // 플레이어 ID가 비어있지 않다면
        if (playerID != null)
            // 종료
            return;

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

        //// 캔버스 가져오기
        //var canvas = transform.GetComponentInChildren<Canvas>();
        //// 플레이어의 카메라 가져오기
        //var camera = root.GetComponentInChildren<Camera>();

        //// 캔버스와 카메라 둘다 비어있지 않다면
        //if (canvas != null && camera != null)
        //    // 캔버스의 카메라를 플레이어 카메라로 설정하기
        //    canvas.worldCamera = camera;

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

        // 손목 UI가 있고 가이드 UI가 있다면
        if (wristUI != null && guideUI != null && mapUI != null)
        {
            // 손목 UI 기능 막기
            wristUI.SetCanOpenUI(false);
            mapUI.Init(wristUI.CurrentProgress.currentStep);
            // 손목 UI의 가이드 진행 상황에 따라서 가이드 UI 초기화
            guideUI.Init(wristUI.CurrentProgress);
            // 가이드 UI 열기
            guideUI.PopupUIHandler(true);
        }
    }

    // LED 화면에서 멀어졌을 때 실행되는 함수
    protected override void OnExited(GameObject player)
    {
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        Transform root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        PhotonView playerPV = root.GetComponentInChildren<PhotonView>();

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

        //// 캔버스 가져오기
        //var canvas = transform.GetComponentInChildren<Canvas>();

        //// 캔버스가 비어있지 않다면
        //if (canvas != null)
        //    // 캔버스의 카메라 초기화
        //    canvas.worldCamera = null;

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

        // 가이드 UI가 있다면
        if (guideUI != null)
        {
            // 가이드가 진행 중이였다면
            if (guideUI.GuideCoroutine != null)
                // 가이드 진행 중지
                guideUI.StopGuideUIRoutine();

            // 가이드 UI 닫기
            guideUI.PopupUIHandler(false);
        }

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