using Photon.Pun;
using System;
using UnityEngine;

public class StationInfo : ArrivalTrigger
{
    public const string UI_NAME_MAP = "Map";                // 노선도
    public const string UI_NAME_DETAIL = "Detail";          // 상세정보

    [Header("지하철 UI들")]
    [SerializeField] private StationMapUI mapUI;            // 노선도 UI
    [SerializeField] private StationDetailUI detailUI;      // 상세정보 UI

    [Header("가이드 UI")]
    [SerializeField] private GuidePopupUI guideUI;

    private PhotonView platformPV;                          // 지하철 포톤뷰
    private PlatformWristUI wristUI;                        // 플레이어 손목 UI

    private int? playerID = null;                           // 플레이어 ID

    private void Awake()
    {
        platformPV = GetComponent<PhotonView>();
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

        // 노선도 UI가 있다면
        if (mapUI != null)
            // 노선도 UI에 ID 설정하기
            mapUI.SetPlayerID(playerID);

        // 상세정보 UI가 있다면
        if (detailUI != null)
            // 상세정보 UI에 ID 설정하기
            detailUI.SetPlayerID(playerID);
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
                // 노선도 UI 변경 함수 실행
                mapUI.ExecuteNetworkAction(buttonPath);
                break;
            // 상세정보라면
            case UI_NAME_DETAIL:
                // 상세정보 UI 변경 함수 실행
                detailUI.ExecuteNetworkAction(buttonPath);
                break;
        }
    }

    // 동기화 요청 함수
    public void RequestSync(string uiType, string buttonPath)
    {
        // UI 변경 동기화 함수 실행
        platformPV.RPC("RPC_SyncChangeUI", RpcTarget.AllBuffered, uiType, buttonPath);
    }

    // LED 화면에 도착했을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        Transform root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        PhotonView playerPV = root.GetComponentInChildren<PhotonView>();

        // 플레이어에 포톤뷰가 없거나, 내 캐릭터에서 발생한 상황이 아니라면
        if (playerPV == null || !playerPV.IsMine)
            // 종료
            return;

        // 플레이어 ID가 비어있지 않다면
        if (playerID != null)
            // 종료
            return;

        // 플레이어 ID 동기화 함수 실행
        platformPV.RPC("RPC_SyncOwner", RpcTarget.AllBuffered, playerPV.Owner.ActorNumber);
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
        // 플레이어 요소를 찾기 편하게 제일 최상위 요소로 이동
        Transform root = player.transform.root;
        // 플레이어의 포톤뷰 가져오기
        PhotonView playerPV = root.GetComponentInChildren<PhotonView>();

        // 플레이어에 포톤뷰가 없거나, 내 캐릭터에서 발생한 상황이 아니라면
        if (playerPV == null || !playerPV.IsMine)
            // 종료
            return;

        // 플레이어 ID가 없거나, 플레이어 ID와 나간 사람의 ID가 다르다면
        if (playerID == null || playerID != playerPV.Owner.ActorNumber)
            // 종료
            return;

        // 플레이어 ID 동기화 함수 실행
        platformPV.RPC("RPC_SyncOwner", RpcTarget.AllBuffered, -1);
        // UI 변경 동기화 함수 실행
        //platformPV.RPC("RPC_SyncChangeUI", RpcTarget.AllBuffered, uiType, buttonPath);

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