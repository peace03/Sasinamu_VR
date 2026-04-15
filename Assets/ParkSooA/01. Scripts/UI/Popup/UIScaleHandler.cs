using System.Collections;
using UnityEngine;

public class UIScaleHandler : MonoBehaviour
{
    private Coroutine scaleCoroutine = null;        // 크기 조절 코루틴
    private AnimationCurve animCurve;               // 연출 효과
                                                        
    private bool curState = false;                  // 현재 UI 상태
    private float duration;                         // 시간
    private float overSize;                         // 크기

    // 초기화 함수
    public void Init(float duration, float overSize, AnimationCurve curve)
    {
        // 초기화
        this.duration = duration;
        this.overSize = overSize;
        animCurve = curve;
    }

    // UI 상태 설정 함수
    public void SetUIState(bool state)
    {
        // 현재 UI 상태와 같다면
        if (curState == state)
            // 종료
            return;

        // 크기 조절 코루틴이 비어있지 않다면
        if (scaleCoroutine != null)
        {
            // 크기 조절 코루틴 멈추기
            StopCoroutine(scaleCoroutine);
            // 크기 조절 코루틴 초기화
            scaleCoroutine = null;
        }

        // 현재 UI 상태 변경
        curState = state;
        // 크기 조절 코루틴 시작
        scaleCoroutine = StartCoroutine(UIHandler());
    }

    // UI 관리 함수
    private IEnumerator UIHandler()
    {
        // UI를 열어야 된다면
        if (curState)
        {
            // 원래 크기보다 UI 크기 키우기
            yield return UIScaleCoroutine(Vector3.one * overSize);
            // 원래 크기로 바꾸기
            yield return UIScaleCoroutine(Vector3.one);
        }
        // UI를 닫아야 된다면
        else
            // 크기 줄이기 기다리기
            yield return UIScaleCoroutine(Vector3.zero);

        // 크기 조절 코루틴 초기화
        scaleCoroutine = null;
    }

    // UI 크기 조절 코루틴
    private IEnumerator UIScaleCoroutine(Vector3 scale)
    {
        // 현재 크기를 시작 값으로 저장하기
        Vector3 startScale = transform.localScale;
        // 시간을 확인할 변수 선언
        float timer = 0f;

        // 연출이 끝날 때까지
        while (timer < duration)
        {
            // 시간 더하기
            timer += Time.deltaTime;
            // 연출 효과 그래프에서 현재 시간에 해당하는 값을 가져와서 그 값으로 크기 조절
            transform.localScale = Vector3.Lerp(startScale, scale, animCurve.Evaluate(timer / duration));
            // 프레임 기다리기
            yield return null;
        }

        // 크기 맞추기
        transform.localScale = scale;
    }
}