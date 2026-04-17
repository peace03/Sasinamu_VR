using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [Header("크기 조절")]
    [SerializeField] private UIScaleHandler scaleHandler;

    [Header("불투명도 조절")]
    [SerializeField] private UIFadeHandler fadeHandler;

    [Header("불투명도가 바뀌는 UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("연출 시간")]
    [SerializeField][Range(0f, 2f)] private float openDuration = 0.3f;
    [SerializeField][Range(0f, 2f)] private float closeDuration = 0.2f;

    [Header("UI가 커질 크기")]
    [SerializeField][Range(1.01f, 1.075f)] private float overSize = 1.05f;

    [Header("연출 효과 그래프")]
    [SerializeField] private AnimationCurve openAnimCurve;
    [SerializeField] private AnimationCurve closeAnimCurve;

    [Header("현재 상태")]
    [ContextMenuItem("열기 테스트", "DebugOpen")]
    [ContextMenuItem("닫기 테스트", "DebugClose")]
    [SerializeField] protected bool isOpened = false;

    // 팝업 UI 관리 함수
    public void PopupUIHandler(bool isOpen)
    {
        // 현재 UI 상태와 같다면
        if (isOpened == isOpen)
            // 종료
            return;

        // UI 상태 저장
        isOpened = isOpen;
        // 크기 조절이 비어있지 않다면, 현재 UI 상태에 따라서 초기화
        scaleHandler?.Init(isOpen ? openDuration : closeDuration, overSize, isOpen ? openAnimCurve : closeAnimCurve);
        // 불투명도 조절이 비어있지 않다면, 현재 UI 상태에 따라서 초기화
        fadeHandler?.Init(canvasGroup, isOpen ? openDuration : closeDuration, isOpen ? openAnimCurve : closeAnimCurve);
        // 크기 조절이 비어있지 않다면, 현재 UI 상태에 따라서 실행
        scaleHandler?.SetUIState(isOpen);
        // 불투명도 조절이 비어있지 않다면, 현재 UI 상태에 따라서 실행
        fadeHandler?.SetUIState(isOpen);
    }

    // 인스펙터 우클릭 전용 UI 열기 함수
    protected void DebugOpen()
    {
        // 게임 실행 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // UI가 열릴 수 있게 현재 UI 상태를 변경
        isOpened = false;
        // UI 열기
        PopupUIHandler(true);
    }

    // 인스펙터 우클릭 전용 UI 닫기 함수
    protected void DebugClose()
    {
        // 게임 실행 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // UI가 닫힐 수 있게 현재 UI 상태를 변경
        isOpened = true;
        // UI 닫기
        PopupUIHandler(false);
    }
}