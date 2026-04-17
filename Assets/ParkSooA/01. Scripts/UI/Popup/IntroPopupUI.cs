using System.Collections;
using UnityEngine;

public class IntroPopupUI : PopupUI
{
    [Header("화면 전환")]
    [SerializeField] private ScreenFader screenFader;

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
        // 화면 전환이 비어있지 않다면
        if (screenFader != null)
        {
            // 화면 전환 활성화
            screenFader.gameObject.SetActive(true);
            // 화면 전환 시작
            screenFader.ScreenFadeHandler(false);
            // 화면 전환 기다리기
            yield return new WaitForSeconds(screenFader.Duration);
        }

        // 열릴 때의 위치가 비어있지 않다면
        if (OpenedPos != null)
            // 팝업 UI 위치 초기화
            transform.position = OpenedPos.position;

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