using Photon.Pun;
using UnityEngine;

public class StationInfo : ArrivalTrigger
{
    [Header("가이드 UI")]
    [SerializeField] private GuidePopupUI guideUI;

    private PhotonView view;                // 포톤 뷰
    private PlatformWristUI wristUI;        // 플랫폼 손목 UI

    // LED 화면에 도착했을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 이미 도착한 사람이 있다면
        if (view != null)
            // 종료
            return;

        // 제일 최상위 객체 저장
        Transform root = player.transform.root;
        // 포톤뷰 저장
        view = root.GetComponent<PhotonView>();
        // 손목 UI 저장
        wristUI = root.GetComponentInChildren<PlatformWristUI>();

        // 손목 UI가 있다면
        if (wristUI != null)
        {
            // 손목 UI의 가이드 진행률에 따라서 가이드 UI 초기화
            guideUI.Init(wristUI.CurrentProgress);
            // 가이드 UI 열기
            guideUI.PopupUIHandler(true);
        }
    }

    protected override void OnExited(GameObject player)
    {

    }

    public void UpdateWristUI() => wristUI?.UpdateWristUI();
}