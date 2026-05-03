using System.Collections;
using UnityEngine;

public class UIFadeHandler : MonoBehaviour
{
    private Coroutine uiStateCoroutine = null;      // UI 상태 코루틴
    private AnimationCurve curve;                   // 연출 효과
    private CanvasGroup canvasGroup;                // UI 불투명도

    private float duration;                         // 시간

    // 초기화 함수
    public void Init(AnimationCurve curve, float duration, CanvasGroup canvasGroup)
    {
        // 초기화
        this.curve = curve;
        this.duration = duration;
        this.canvasGroup = canvasGroup;
    }

    // UI 상태 설정 함수
    public void SetUIState(bool state)
    {
        // UI 상태 코루틴이 비어있지 않다면
        if (uiStateCoroutine != null)
        {
            // UI 상태 루틴 멈추기
            StopCoroutine(uiStateCoroutine);
            // UI 상태 코루틴 초기화
            uiStateCoroutine = null;
        }

        // UI 상태에 따른 루틴 시작
        uiStateCoroutine = StartCoroutine(UIStateRoutine(state));
    }

    // UI 상태 루틴 정지 함수
    public void StopUIStateRoutine()
    {
        // UI 상태 코루틴이 비어있지 않다면
        if (uiStateCoroutine != null)
        {
            // UI 상태 루틴 멈추기
            StopCoroutine(uiStateCoroutine);
            // UI 상태 코루틴 초기화
            uiStateCoroutine = null;
        }
    }

    // UI 상태 루틴 함수
    private IEnumerator UIStateRoutine(bool state)
    {
        // UI를 열어야 된다면
        if (state)
            // 불투명하게 바꾸기
            yield return ApplyFadeRoutine(1f);
        // UI를 닫아야 된다면
        else
            // 투명하게 바꾸기
            yield return ApplyFadeRoutine(0f);
    }

    // 불투명도 적용 루틴 함수
    private IEnumerator ApplyFadeRoutine(float value)
    {
        // 현재 불투명도를 시작 값으로 저장
        float startAlpha = canvasGroup.alpha;
        // 시간을 확인할 변수 선언
        float timer = 0f;

        // 연출이 끝날 때까지
        while (timer < duration)
        {
            // 시간 더하기
            timer += Time.deltaTime;
            // 연출 효과 그래프에서 현재 시간에 해당하는 값을 가져와서 그 값으로 불투명도 조절
            canvasGroup.alpha = Mathf.Lerp(startAlpha, value, curve.Evaluate(timer / duration));
            // 프레임 기다리기
            yield return null;
        }

        // 불투명도 맞추기
        canvasGroup.alpha = value;
        // UI 상태 코루틴 초기화
        uiStateCoroutine = null;
    }
}