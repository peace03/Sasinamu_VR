using System.Collections;
using UnityEngine;

public class UIScaleHandler : MonoBehaviour
{
    private Coroutine scaleCoroutine = null;        // 크기 조절 코루틴
                                                        
    private bool curState = false;                  // 현재 UI 상태
    private float duration;                         // 시간
    private float distance;                         // 거리
    private float overSize;                         // 크기

    // 초기화 함수
    public void Init(float duration, float distance, float overSize)
    {
        // 초기화
        this.duration = duration;
        this.distance = distance;
        this.overSize = overSize;
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
            yield return StartCoroutine(UIScaleCoroutine(Vector3.one * overSize));
            // 원래 크기로 바꾸기
            yield return StartCoroutine(UIScaleCoroutine(Vector3.one));
        }
        // UI를 닫아야 된다면
        else
            // 크기 줄이기 기다리기
            yield return StartCoroutine(UIScaleCoroutine(Vector3.zero));

        // 크기 조절 코루틴 초기화
        scaleCoroutine = null;
    }

    // UI 크기 조절 코루틴
    private IEnumerator UIScaleCoroutine(Vector3 scale)
    {
        // 현재 속도를 저장할 변수 선언
        Vector3 velocity = Vector3.zero;

        // 목표 크기에 도달할 때까지
        while (Vector3.Distance(transform.localScale, scale) > distance)
        {
            // 크기 조절
            transform.localScale = Vector3.SmoothDamp(transform.localScale, scale, ref velocity, duration);
            // 프레임 기다리기
            yield return null;
        }

        // 크기 맞추기
        transform.localScale = scale;
    }
}