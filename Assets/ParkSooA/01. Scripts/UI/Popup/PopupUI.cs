using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [Header("크기 조절 스크립트")]
    [SerializeField] private UIScaleHandler scaleHandler;

    [Header("불투명도 조절 스크립트")]
    [SerializeField] private UIFadeHandler fadeHandler;

    [Header("불투명도를 변경할 UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("연출의 대략적인 시간(열리는 것은 약 2배)")]
    [SerializeField][Range(0f, 1f)] private float duration = 0.5f;

    [Header("연출이 끝나는 지점(값)")]
    [SerializeField][Range(0f, 1f)] private float distance = 0.01f;

    [Header("UI가 한번만 커질 크기")]
    [SerializeField][Range(1.01f, 1.075f)] private float overSize = 1.05f;

    private void Awake()
    {
        // 초기화
        scaleHandler.Init(duration, distance, overSize);
        fadeHandler.Init(canvasGroup, duration, distance);
    }

    // 팝업 UI 관리 함수
    public void PopupUIHandler(bool state)
    {
        // 크기 조절이나 불투명도 조절이 비어있다면
        if (scaleHandler == null || fadeHandler == null)
        {
            Debug.Log("크기 조절 혹은 불투명도 조절이 비어있습니다.");
            // 종료
            return;
        }

        // 크기 조절
        scaleHandler.SetUIState(state);
        // 불투명도 조절
        fadeHandler.SetUIState(state);
    }
}