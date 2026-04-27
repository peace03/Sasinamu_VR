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

    public Transform OpenedPos => openedPos;            // UI 여는 위치

    public float CloseDuration => closeDuration;        // UI 닫기 연출 시간

    private bool curState = false;                      // 현재 UI 상태
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
        // UI 크기 조절
        SetUIScale(curState);
        // UI 불투명도 조절
        SetUIFade(curState);
        // UI 이동 조절
        SetUIMove(curState);
    }

    // UI 크기 설정 함수
    public void SetUIScale(bool isOpen)
    {
        // 크기 조절이 비어있다면
        if (scaleHandler == null)
            // 종료
            return;

        // UI 상태에 따라서 초기화
        scaleHandler.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration, overSize);
        // UI 상태에 따라서 실행
        scaleHandler.SetUIState(isOpen);
    }

    // UI 불투명도 설정 함수
    public void SetUIFade(bool isOpen, float? custom = null, CanvasGroup customGroup = null)
    {
        // 불투명도 조절이 비어있다면
        if (fadeHandler == null)
            // 종료
            return;

        // 초 설정이 없다면, UI 상태에 따라서 연출 시간 저장
        float duration = custom ?? (isOpen ? openDuration : closeDuration);
        // 그룹 설정이 없다면, 기존 캔버스 그룹으로 연출 대상 저장
        CanvasGroup group = customGroup ?? canvasGroup;
        // UI 상태에 따라서 초기화
        fadeHandler.Init(isOpen ? openCurve : closeCurve, duration, group);
        // UI 상태에 따라서 실행
        fadeHandler.SetUIState(isOpen);
    }

    // UI 이동 설정 함수
    public void SetUIMove(bool isOpen)
    {
        // 이동 조절이 비어있다면
        if (moveHandler == null)
            // 종료
            return;

        // UI 상태에 따라서 초기화
        moveHandler.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration,
                                                                            isOpen ? openedPos : closedPos);
        // 실행
        moveHandler.SetUIState();
    }
}