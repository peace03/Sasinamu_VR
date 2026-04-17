using System.Collections;
using UnityEngine;

public class IntroPopupUI : PopupUI
{
    [Header("시작 후 기다리는 시간")]
    [SerializeField][Range(0f, 10f)] private float startDelayDuration = 10f;

    [Header("UI를 보여주는 시간")]
    [SerializeField][Range(0f, 15f)] private float displayDuration = 15f;

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
        // 도입부 연출 시작
        StartCoroutine(IntroSequence());
    }

    // 도입부 연출 함수
    private IEnumerator IntroSequence()
    {
        // 시작 후 기다리기
        yield return new WaitForSeconds(startDelayDuration);
        // 팝업 UI 띄워주기
        PopupUIHandler(true);
        // UI 보여주기
        yield return new WaitForSeconds(displayDuration);
        // 팝업 UI 닫기
        PopupUIHandler(false);
    }
}