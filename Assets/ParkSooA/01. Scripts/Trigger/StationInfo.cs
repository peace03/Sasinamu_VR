using Photon.Pun;
using UnityEngine;

public class StationInfo : ArrivalTrigger
{
    [Header("지하철 UI들")]
    [SerializeField] private StationMapUI mapUI;            // 노선도 UI
    [SerializeField] private StationDetailUI detailUI;      // 상세정보 UI

    [Header("가이드 UI")]
    [SerializeField] private GuidePopupUI guideUI;

    private PhotonView pv;                                  // 포톤뷰
    private PlatformWristUI wristUI;                        // 플레이어 손목 UI

    private int? playerID = null;                           // 플레이어 ID

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    // 플레이어 ID 설정 동기화 함수
    public void RPC_SyncOwner(int id)
    {
        if (id == -1)
        {
            playerID = null;
            mapUI.SetPlayerID(playerID);
            detailUI.SetPlayerID(playerID);
        }
        else
        {
            playerID = id;
            mapUI.SetPlayerID(id);
            detailUI.SetPlayerID(id);
        }
    }

    [PunRPC]
    // UI 변경 동기화 함수
    public void RPC_SyncChangeUI(string uiType, int buttonType)
    {

    }

    // 동기화 요청 함수
    public void RequestSync(string uiType, int buttonType)
    {

    }

    // LED 화면에 도착했을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 이미 도착한 사람이 있다면
        if (playerID != null)
            // 종료
            return;

        // 제일 최상위 객체 저장
        Transform root = player.transform.root;
        // 플레이어 ID 저장
        playerID = root.GetComponentInChildren<PhotonView>()?.Owner?.ActorNumber;

        // 플레이어 ID가 있다면
        if (playerID != null)
        {
            // 노선도 UI가 있다면
            if (mapUI != null)
                // 노선도 UI에 플레이어 ID 설정하기 
                mapUI.SetPlayerID(playerID);

            // 상세정보 UI가 있다면
            if (detailUI != null)
                // 상세정보 UI에 플레이어 ID 설정하기
                detailUI.SetPlayerID(playerID);
        }

        // 손목 UI 저장
        wristUI = root.GetComponentInChildren<PlatformWristUI>();

        // 손목 UI가 있고 가이드 UI가 있다면
        if (wristUI != null && guideUI != null)
        {
            // 손목 UI 기능 막기
            wristUI.SetCanOpenUI(false);
            // 손목 UI의 가이드 진행 상황에 따라서 가이드 UI 초기화
            guideUI.Init(wristUI.CurrentProgress);
            // 가이드 UI 열기
            guideUI.PopupUIHandler(true);
        }
    }

    protected override void OnExited(GameObject player)
    {
        // 비교 ID 받아오기
        int? compareID = player.transform.root.GetComponentInChildren<PhotonView>()?.Owner?.ActorNumber;

        // 플레이어 ID가 없거나, 플레이어 ID와 비교 ID가 다르다면
        if (playerID == null || playerID != compareID)
            // 종료
            return;

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

        // 플레이어 ID 초기화
        playerID = null;
    }
}