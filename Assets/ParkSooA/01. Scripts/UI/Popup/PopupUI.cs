using System.Security.Cryptography;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [Header("크기 효과")]
    [SerializeField] private UIScaleHandler scaleHandler;

    [Header("불투명도 효과")]
    [SerializeField] private UIFadeHandler fadeHandler;

    [Header("불투명도가 바뀌는 UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("연출 시간")]
    [SerializeField][Range(0f, 1f)] private float openDuration = 0.3f;
    [SerializeField][Range(0f, 1f)] private float closeDuration = 0.2f;

    [Header("UI가 커질 크기")]
    [SerializeField][Range(1.01f, 1.075f)] private float overSize = 1.05f;

    [Header("연출 효과 그래프")]
    [SerializeField] private AnimationCurve openAnimCurve;
    [SerializeField] private AnimationCurve closeAnimCurve;

    private bool curState = false;

    private void OnValidate()
    {
        if (scaleHandler != null && fadeHandler != null)
        {
            if(curState)
            {
                scaleHandler.Init(openDuration, overSize, openAnimCurve);
                fadeHandler.Init(canvasGroup, openDuration, openAnimCurve);
            }
            else
            {
                scaleHandler.Init(closeDuration, overSize, closeAnimCurve);
                fadeHandler.Init(canvasGroup, closeDuration, closeAnimCurve);
            }

            Debug.Log("인스펙터 값이 반영되었습니다.");
        }
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

        // 현재 상태 저장
        curState = state;
        // 크기 조절
        scaleHandler.SetUIState(state);
        // 불투명도 조절
        fadeHandler.SetUIState(state);
    }
}