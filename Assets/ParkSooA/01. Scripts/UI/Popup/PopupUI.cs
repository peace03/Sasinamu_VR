using UnityEngine;

public class PopupUI : MonoBehaviour
{
    #region 변수
    [Header("크기 조절")]
    [SerializeField] private UIScaleHandler scaleHandler;

    [Header("불투명도 조절")]
    [SerializeField] private UIFadeHandler fadeHandler;

    [Header("불투명도가 바뀌는 UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("이동 조절")]
    [SerializeField] private UIMoveHandler moveHandler;

    [Header("이동 위치")]
    [SerializeField] private Transform openedPos;
    [SerializeField] private Transform closedPos;

    [Header("연출 시간")]
    [SerializeField][Range(0f, 2f)] private float openDuration = 0.3f;
    [SerializeField][Range(0f, 2f)] private float closeDuration = 0.2f;

    [Header("UI가 커질 크기")]
    [SerializeField][Range(1.01f, 1.075f)] private float overSize = 1.05f;

    [Header("연출 효과 그래프")]
    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private AnimationCurve closeCurve;

    [Header("현재 상태")]
    [ContextMenuItem("열기 테스트", "DebugOpen")]
    [ContextMenuItem("닫기 테스트", "DebugClose")]
    [SerializeField] protected bool curState = false;

    public Transform OpenedPos => openedPos;

    public float CloseDuration => closeDuration;
    #endregion

    // 팝업 UI 관리 함수
    public void PopupUIHandler(bool isOpen)
    {
        // 현재 UI 상태와 같다면
        if (curState == isOpen)
            // 종료
            return;

        // UI 상태 저장
        curState = isOpen;
        // 크기, 불투명도, 이동 조절이 비어있지 않다면 현재 UI 상태에 따라서 초기화
        scaleHandler?.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration, overSize);
        fadeHandler?.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration, canvasGroup);
        moveHandler?.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration,
                                                                            isOpen ? openedPos : closedPos);
        // 크기, 불투명도, 이동 조절이 비어있지 않다면 현재 UI 상태에 따라서 실행
        scaleHandler?.SetUIState(isOpen);
        fadeHandler?.SetUIState(isOpen);
        moveHandler?.SetUIState();
    }

    // 인스펙터 우클릭 전용 UI 열기 함수
    protected void DebugOpen()
    {
        // 게임 실행 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // UI가 열릴 수 있게 현재 UI 상태를 변경
        curState = false;
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
        curState = true;
        // UI 닫기
        PopupUIHandler(false);
    }
}