//using System.Collections;
//using UnityEngine;

//public class UIMoveHandler : MonoBehaviour
//{
//    private Coroutine uiStateCoroutine = null;      // UI 상태 코루틴
//    private AnimationCurve animCurve;               // 연출 효과

//    private float duration;                         // 시간
//    private float overSize;                         // 크기

//    // 초기화 함수
//    public void Init(float duration, float overSize, AnimationCurve curve)
//    {
//        // 초기화
//        this.duration = duration;
//        this.overSize = overSize;
//        animCurve = curve;
//    }

//    // UI 상태 설정 함수
//    public void SetUIState(bool state)
//    {
//        // UI 상태 코루틴이 비어있지 않다면
//        if (uiStateCoroutine != null)
//        {
//            // UI 상태 루틴 멈추기
//            StopCoroutine(uiStateCoroutine);
//            // UI 상태 코루틴 초기화
//            uiStateCoroutine = null;
//        }

//        // UI 상태에 따른 루틴 시작
//        uiStateCoroutine = StartCoroutine(UIStateRoutine(state));
//    }

//    // UI 상태 루틴 함수
//    private IEnumerator UIStateRoutine(bool state)
//    {
//        // UI를 열어야 된다면
//        if (state)
//        {
//            // 원래 크기보다 UI 크기 키우기
//            yield return ApplyScaleRoutine(Vector3.one * overSize);
//            // 원래 크기로 바꾸기
//            yield return ApplyScaleRoutine(Vector3.one);
//        }
//        // UI를 닫아야 된다면
//        else
//            // 크기 줄이기
//            yield return ApplyScaleRoutine(Vector3.zero);

//        // UI 상태 코루틴 초기화
//        uiStateCoroutine = null;
//    }

//    // 이동 적용 루틴 함수
//    private IEnumerator ApplyMoveRoutine(Vector3 pos)
//    {
//        // 현재 위치를 시작 위치로 저장하기
//        Vector3 startPos = transform.;
//        // 시간을 확인할 변수 선언
//        float timer = 0f;

//        // 연출이 끝날 때까지
//        while (timer < duration)
//        {
//            // 시간 더하기
//            timer += Time.deltaTime;
//            // 연출 효과 그래프에서 현재 시간에 해당하는 값을 가져와서 그 값으로 크기 조절
//            transform.localScale = Vector3.Lerp(startScale, pos, animCurve.Evaluate(timer / duration));
//            // 프레임 기다리기
//            yield return null;
//        }

//        // 크기 맞추기
//        transform.localScale = scale;
//    }
//}