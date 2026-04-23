using System.Collections;
using UnityEngine;

public enum GuideState { Select, Check, Move }

public class GuidePopupUI : PopupUI
{
    [Header("가이드 UI들")]
    [SerializeField] private GameObject[] guideUI;

    [Header("가이드 UI 전환 시간")]
    [SerializeField][Range(0f, 1f)] private float duration = 0.3f;

    [Header("확인 이미지")]
    [SerializeField] private GameObject stationCheckImage;
    [SerializeField] private GameObject exitCheckImage;
    [SerializeField] private GameObject foodCheckImage;

    private Coroutine guideCoroutine;                                       // 가이드 UI 코루틴

    private GuideState guideState = GuideState.Select;                      // 가이드 UI 상태

    private bool checkStation = false;                                      // 역 혼잡도 확인 여부
    private bool checkExit = false;                                         // 출구 혼잡도 확인 여부
    private bool checkFood = false;                                         // 역 주변 맛집 확인 여부
    private bool ReadyToMove => checkStation && checkExit && checkFood;     // 이동 가능 여부 확인

    private void Awake()
    {
        // 초기화
        guideUI[(int)guideState].SetActive(true);
    }

    // 다음 가이드 UI 열기 함수
    public void OpenNextGuideUI()
    {
        // 마지막 가이드이거나, 가이드 UI 코루틴이 비어있지 않다면
        if (guideState == GuideState.Move || guideCoroutine != null)
            // 종료
            return;

        // 가이드 UI 연출 시작
        guideCoroutine = StartCoroutine(GuideUIRoutine());
    }

    // 가이드 UI 루틴 함수
    private IEnumerator GuideUIRoutine()
    {
        // UI 닫기
        SetUIFade(false, duration);
        // UI 닫기 기다리기
        yield return new WaitForSeconds(duration);
        // 가이드 UI 닫기
        guideUI[(int)guideState++].SetActive(false);
        // 다음 가이드 UI 열기
        guideUI[(int)guideState].SetActive(true);
        // UI 열기
        SetUIFade(true, duration);
        // UI 열기 기다리기
        yield return new WaitForSeconds(duration);
        // 가이드 UI 코루틴 초기화
        guideCoroutine = null;
    }

    // 가이드 UI 연출 중지 함수
    public void StopGuideUIRoutine()
    {
        // 가이드 UI 코루틴이 비어있다면
        if (guideCoroutine == null)
            // 종료
            return;

        // 가이드 UI 연출 중지
        StopCoroutine(guideCoroutine);
        // 가이드 UI 코루틴 초기화
        guideCoroutine = null;
    }

    // 역 혼잡도 확인 함수
    public void CheckStation()
    {
        // 확인 이미지 활성화
        stationCheckImage.SetActive(true);
        // 역 혼잡도 확인
        checkStation = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 출구 혼잡도 확인 함수
    public void CheckExit()
    {
        // 확인 이미지 활성화
        exitCheckImage.SetActive(true);
        // 출구 혼잡도 확인
        checkExit = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 역 주변 맛집 확인 함수
    public void CheckFood()
    {
        // 확인 이미지 활성화
        foodCheckImage.SetActive(true);
        // 역 주변 맛집 확인
        checkFood = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }
}