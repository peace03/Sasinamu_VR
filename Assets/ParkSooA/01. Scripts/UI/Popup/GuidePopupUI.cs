using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuidePopupUI : PopupUI
{
    [Header("가이드 UI 전환 시간")]
    [SerializeField][Range(0f, 1f)] private float duration = 0.3f;

    [Header("가이드 UI들")]
    [SerializeField] private GameObject[] guideUI;

    [Header("확인 이미지")]
    [SerializeField] private Image stationCheckImage;
    [SerializeField] private Image exitCheckImage;
    [SerializeField] private Image foodCheckImage;
    [SerializeField] private Sprite[] stationCheckSprites;
    [SerializeField] private Sprite[] exitCheckSprites;
    [SerializeField] private Sprite[] foodCheckSprites;

    [Header("다음 가이드 단계가 되면 실행되는 함수")]
    [Space(10)][SerializeField] private UnityEvent OnNextGuide;

    private Coroutine guideCoroutine;                           // 가이드 UI 코루틴

    private GuideData guideData;                                // 현재 가이드 정보
    private bool ReadyToMove
        => guideData.station && guideData.exit && guideData.food;

    // 초기화 함수
    public void Init(GuideData data)
    {
        // 현재 가이드 정보 저장
        guideData = data;
        // UI 새로고침
        RefreshUI();
    }

    // UI 새로고침 함수
    private void RefreshUI()
    {
        // 가이드 UI들 닫기
        foreach (var guide in guideUI)
            guide.SetActive(false);

        // 현재 가이드 UI 열기
        guideUI[(int)guideData.state].SetActive(true);
        // 확인 이미지 새로고침
        RefreshCheckImage();
    }

    // 확인 이미지 새로고침 함수
    private void RefreshCheckImage()
    {
        // 현재 가이드가 확인 UI가 아니라면
        if (guideData.state != GuideState.Check)
            // 종료
            return;

        // 역 혼잡도 이미지가 있다면
        if (stationCheckImage != null)
        {
            // 역 혼잡도 확인 여부에 따라서 확인 이미지 초기화
            if (guideData.station)
                stationCheckImage.sprite = stationCheckSprites[1];
            else
                stationCheckImage.sprite = stationCheckSprites[0];
        }

        // 출구 혼잡도 이미지가 있다면
        if (exitCheckImage != null)
        {
            // 출구 혼잡도 확인 여부에 따라서 확인 이미지 초기화
            if (guideData.exit)
                exitCheckImage.sprite = exitCheckSprites[1];
            else
                exitCheckImage.sprite = exitCheckSprites[0];
        }

        // 역 주변 맛집 이미지가 있다면
        if (foodCheckImage != null)
        {
            // 역 주변 맛집 확인 여부에 따라서 확인 이미지 초기화
            if (guideData.food)
                foodCheckImage.sprite = foodCheckSprites[1];
            else
                foodCheckImage.sprite = foodCheckSprites[0];
        }
    }

    // 다음 가이드 UI 열기 함수
    public void OpenNextGuideUI()
    {
        // 마지막 가이드이거나, 가이드 UI 코루틴이 비어있지 않다면
        if (guideData.state == GuideState.Move || guideCoroutine != null)
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
        guideUI[(int)guideData.state++].SetActive(false);
        // 다음 가이드 UI 열기
        guideUI[(int)guideData.state].SetActive(true);
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
        // 역 혼잡도 확인
        guideData.station = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 출구 혼잡도 확인 함수
    public void CheckExit()
    {
        // 출구 혼잡도 확인
        guideData.exit = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 역 주변 맛집 확인 함수
    public void CheckFood()
    {
        // 역 주변 맛집 확인
        guideData.food = true;

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }
}