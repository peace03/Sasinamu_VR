using System.Collections;
using UnityEngine;

public class UIMoveHandler : MonoBehaviour
{
    private Coroutine uiStateCoroutine = null;      // UI 상태 코루틴
    private AnimationCurve curve;                   // 연출 효과
    private Transform target;                       // 이동할 위치

    private float duration;                         // 시간

    // 초기화 함수
    public void Init(AnimationCurve curve, float duration, Transform target)
    {
        // 초기화
        this.duration = duration;
        this.curve = curve;
        this.target = target;
    }

    // UI 상태 설정 함수
    public void SetUIState()
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
        uiStateCoroutine = StartCoroutine(UIStateRoutine());

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
    private IEnumerator UIStateRoutine()
    {
        // 이동하기
        yield return ApplyMoveRoutine();
    }

    // 이동 적용 루틴 함수
    private IEnumerator ApplyMoveRoutine()
    {
        // 현재 위치를 시작 위치로 저장하기
        Vector3 startPos = transform.position;
        // 시간을 확인할 변수 선언
        float timer = 0f;

        // 연출이 끝날 때까지
        while (timer < duration)
        {
            // 시간 더하기
            timer += Time.deltaTime;
            // 연출 효과 그래프에서 현재 시간에 해당하는 값을 가져와서 그 값으로 위치 조절
            transform.position = Vector3.Lerp(startPos, target.position, curve.Evaluate(timer / duration));
            // 프레임 기다리기
            yield return null;
        }

        // 위치 맞추기
        transform.position = target.position;
        // UI 상태 코루틴 초기화
        uiStateCoroutine = null;
    }
}