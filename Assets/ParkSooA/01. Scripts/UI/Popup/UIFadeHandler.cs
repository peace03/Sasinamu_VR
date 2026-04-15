using System.Collections;
using UnityEngine;

public class UIFadeHandler : MonoBehaviour
{
    private CanvasGroup canvasGroup;                // UI
    private Coroutine fadeCoroutine = null;         // 불투명도 조절 코루틴

    private bool isOpen = false;                    // 현재 UI 상태
    private float duration;                         // 속도
    private float distance;                         // 값

    // 초기화 함수
    public void Init(CanvasGroup canvasGroup, float duration, float  distance)
    {
        // 초기화
        this.canvasGroup = canvasGroup;
        this.duration = duration;
        this.distance = distance;
    }

    // UI 상태 설정 함수
    public void SetUIState(bool state)
    {
        // 현재 UI 상태와 같다면
        if (isOpen == state)
            // 종료
            return;

        // 불투명도 조절 코루틴이 비어있지 않다면
        if (fadeCoroutine != null)
            // 종료
            return;

        // 현재 UI 상태 변경
        isOpen = state;
        // 불투명도 조절 코루틴 시작
        fadeCoroutine = StartCoroutine(UIHandler());
    }

    // UI 관리 함수
    private IEnumerator UIHandler()
    {
        // UI를 열어야 된다면
        if (isOpen)
            yield return StartCoroutine(FadeCoroutine(1f));
        // UI를 닫아야 된다면
        else
            yield return StartCoroutine(FadeCoroutine(0f));

        // 불투명도 조절 코루틴 초기화
        fadeCoroutine = null;
    }

    // 불투명도 조절 코루틴
    private IEnumerator FadeCoroutine(float value)
    {
        // 현재 속도를 저장할 변수 선언
        float velocity = 0f;

        // 목표 불투명도 값까지
        while (Mathf.Abs(value - canvasGroup.alpha) > distance)
        {
            // 불투명도 값 수정
            canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, value, ref velocity, duration);
            // 프레임 기다리기
            yield return null;
        }

        // 불투명도 맞추기
        canvasGroup.alpha = value;
    }
}