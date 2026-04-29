using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntroPopupUI : PopupUI
{
    [Header("화면 전환")]
    [SerializeField] private SceneChangeUI sceneChangeUI;

    [Header("시작 후 기다리는 시간")]
    [SerializeField][Range(0f, 10f)] private float startDelayDuration = 10f;

    [Header("UI를 보여주는 시간")]
    [SerializeField][Range(0f, 15f)] private float displayDuration = 15f;

    //[Header("인트로가 시작할 때 실행될 함수(테스트용)")]
    //[Space(10)][SerializeField] private UnityEvent OnStart;

    [Header("인트로가 끝났을 때 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnFinished;

    private void Start()
    {
        // 도입부 연출 시작
        StartCoroutine(IntroSequence());
    }

    [ContextMenu("인트로 전체 재생")]
    // 도입부 연출 호출 함수
    private void RestartIntro()
    {
        // 게임 시작 중이 아니라면
        if (!Application.isPlaying)
            // 종료
            return;

        // 혹시 모를 중복 방지
        StopAllCoroutines();
        //// 손목 UI가 안 열리게 바꾸기
        //OnStart?.Invoke();
        // 도입부 연출 시작
        StartCoroutine(IntroSequence());
    }

    // 도입부 연출 함수
    private IEnumerator IntroSequence()
    {
        // 화면 전환 UI가 비어있지 않다면
        if (sceneChangeUI != null)
        {
            // 화면 전환 UI 열기
            sceneChangeUI.gameObject.SetActive(true);
            // 화면 전환(페이드 인) 시작
            sceneChangeUI.SetSceneChange(false);
            // 화면 전환 기다리기
            yield return new WaitForSeconds(sceneChangeUI.Duration);
        }

        // 열릴 때의 위치가 비어있지 않다면
        if (OpenedPos != null)
            // 팝업 UI 위치 초기화
            transform.position = OpenedPos.position;

        // 시작 후 기다리기
        yield return new WaitForSeconds(startDelayDuration);
        // 인트로 UI 열기
        PopupUIHandler(true);
        // UI 보여주기
        yield return new WaitForSeconds(displayDuration);
        // 인트로 UI 닫기
        PopupUIHandler(false);
        // UI 닫히는 거 기다리기
        yield return new WaitForSeconds(CloseDuration);
        // 인트로가 끝났을 때 실행될 함수가 있다면 실행하기
        OnFinished?.Invoke();
    }
}