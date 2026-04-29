using UnityEngine;

public class SceneChangeUI : MonoBehaviour
{
    #region 변수
    [Header("불투명도 조절")]
    [SerializeField] private UIFadeHandler fadeHandler;

    [Header("불투명도가 바뀌는 UI")]
    [SerializeField] private CanvasGroup openGroup;
    [SerializeField] private CanvasGroup closeGroup;

    [Header("연출 시간")]
    [SerializeField][Range(5f, 30f)] private float openDuration = 30f;
    [SerializeField][Range(5f, 30f)] private float closeDuration = 30f;

    [Header("연출 효과")]
    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private AnimationCurve closeCurve;

    [Header("현재 상태")]
    [ContextMenuItem("열기 테스트", "DebugOpen")]
    [ContextMenuItem("닫기 테스트", "DebugClose")]
    [SerializeField] private bool curState = false;

    public float Duration => curState ? openDuration : closeDuration;
    #endregion

    // 화면 전환 설정 함수
    public void SetSceneChange(bool isOpen)
    {
        // 현재 UI 상태와 같다면
        if (curState == isOpen)
            // 종료
            return;

        // UI 상태 저장
        curState = isOpen;
        // 불투명도 조절이 비어있지 않다면 현재 UI 상태에 따라서 초기화
        fadeHandler?.Init(isOpen ? openCurve : closeCurve, isOpen ? openDuration : closeDuration,
                                                                        isOpen ? openGroup : closeGroup);
        // 불투명도 조절이 비어있지 않다면 현재 UI 상태에 따라서 실행
        fadeHandler?.SetUIState(isOpen);
    }

    // 화면 전환 중지 함수
    public void StopSceneChange()
    {
        // 불투명도 조절이 비어있지 않다면 실행 중인 걸 중지
        fadeHandler?.StopUIStateRoutine();

        // 현재 UI 상태에 따라서
        if (curState)
            // 페이드 아웃이 필요한 UI는 투명하게 바꾸기
            openGroup.alpha = 0f;
        else
            // 페이드 인이 필요한 UI는 불투명하게 바꾸기
            closeGroup.alpha = 1f;

        // 현재 UI 상태의 반대로 저장
        curState = !curState;
    }

    // 인스펙터 우클릭 전용 UI 열기 함수
    private void DebugOpen()
    {
        // 게임 실행 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // 화면 전환 할 수 있게 현재 UI 상태를 변경
        curState = false;
        // 화면 전환(페이드 아웃) 시작
        SetSceneChange(true);
    }

    // 인스펙터 우클릭 전용 UI 닫기 함수
    private void DebugClose()
    {
        // 게임 실행 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // 화면 전환 할 수 있게 현재 UI 상태를 변경
        curState = true;
        // 화면 전환(페이드 인) 시작
        SetSceneChange(false);
    }
}