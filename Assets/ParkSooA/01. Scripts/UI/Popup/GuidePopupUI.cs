using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuidePopupUI : PopupUI
{
    [Header("다음 가이드로 넘어가는 시간")]
    [SerializeField][Range(0f, 1f)] private float nextGuideDuration = 0.3f;

    [Header("가이드 UI들")]
    [SerializeField] private GameObject[] guideAllUI;

    [Header("확인 이미지")]
    [SerializeField] private Image stationCheckImage;
    [SerializeField] private Image exitCheckImage;
    [SerializeField] private Image foodCheckImage;
    [SerializeField] private Sprite[] stationCheckSprites;
    [SerializeField] private Sprite[] exitCheckSprites;
    [SerializeField] private Sprite[] foodCheckSprites;

    private CanvasGroup[] guideCanvasGroup;     // 가이드 UI들의 캔버스 그룹
    private Coroutine guideCoroutine;           // 가이드 UI 코루틴
    public Coroutine GuideCoroutine => guideCoroutine;

    private GuideProgress guideProgress;        // 현재 가이드 진행 상황
    private bool ReadyToMove
        => guideProgress.stationCheck && guideProgress.exitCheck && guideProgress.foodCheck;

    private void Awake()
    {
        // 가이드 UI가 있다면
        if (guideAllUI != null)
        {
            // 가이드 UI의 개수만큼 캔버스 그룹 개수를 정하기
            guideCanvasGroup = new CanvasGroup[guideAllUI.Length];

            // 가이드 UI 개수만큼
            for (int i = 0; i < guideAllUI.Length; i++)
            {
                // 가이드 UI가 비어있다면
                if (guideAllUI[i] == null)
                    // 건너뛰기
                    continue;
                // 캔버스 그룹이 없다면
                else if (!guideAllUI[i].TryGetComponent<CanvasGroup>(out var canvas))
                {
                    Debug.Log($"{i}번째 가이드 UI에 캔버스 그룹 없음");
                    continue;
                }
                else
                    // 캔버스 그룹 저장
                    guideCanvasGroup[i] = canvas;
            }
        }
    }

    // 초기화 함수
    public void Init(GuideProgress progress)
    {
        // 현재 가이드 진행 상황 저장
        guideProgress = progress;
        // UI 새로고침
        RefreshUI();
    }

    // UI 재설정 함수
    private void RefreshUI()
    {
        // 가이드 UI들 닫기
        foreach (var guide in guideAllUI)
            guide.SetActive(false);

        // 현재 가이드 진행 상황에 맞는 UI 열기
        guideAllUI[(int)guideProgress.currentStep].SetActive(true);
        // 확인 이미지 새로고침
        RefreshCheckImage();
    }

    // 확인 이미지 재설정 함수
    private void RefreshCheckImage()
    {
        // 현재 가이드 진행 상황이 확인 이미지가 필요한 가이드가 아니라면
        if (guideProgress.currentStep != GuideState.Check)
            // 종료
            return;

        // 역 혼잡도 이미지가 있다면
        if (stationCheckImage != null)
        {
            // 역 혼잡도 확인 여부에 따라서 확인 이미지 초기화
            if (guideProgress.stationCheck)
                stationCheckImage.sprite = stationCheckSprites[1];
            else
                stationCheckImage.sprite = stationCheckSprites[0];
        }

        // 출구 혼잡도 이미지가 있다면
        if (exitCheckImage != null)
        {
            // 출구 혼잡도 확인 여부에 따라서 확인 이미지 초기화
            if (guideProgress.exitCheck)
                exitCheckImage.sprite = exitCheckSprites[1];
            else
                exitCheckImage.sprite = exitCheckSprites[0];
        }

        // 역 주변 맛집 이미지가 있다면
        if (foodCheckImage != null)
        {
            // 역 주변 맛집 확인 여부에 따라서 확인 이미지 초기화
            if (guideProgress.foodCheck)
                foodCheckImage.sprite = foodCheckSprites[1];
            else
                foodCheckImage.sprite = foodCheckSprites[0];
        }
    }

    // 다음 가이드 UI 열기 함수
    public void OpenNextGuideUI()
    {
        // 마지막 가이드이거나, 가이드 UI 코루틴이 비어있지 않다면
        if (guideProgress.currentStep == GuideState.Move || guideCoroutine != null)
            // 종료
            return;

        // 가이드 UI 연출 시작
        guideCoroutine = StartCoroutine(GuideUIRoutine());
    }

    // 가이드 UI 루틴 함수
    private IEnumerator GuideUIRoutine()
    {
        // UI 닫기
        SetUIFade(false, nextGuideDuration, guideCanvasGroup[(int)guideProgress.currentStep]);
        // UI 닫기 기다리기
        yield return new WaitForSeconds(nextGuideDuration);
        // 가이드 UI 닫기
        guideAllUI[(int)guideProgress.currentStep++].SetActive(false);
        // 다음 가이드 UI 열기
        guideAllUI[(int)guideProgress.currentStep].SetActive(true);
        // UI 열기
        SetUIFade(true, nextGuideDuration, guideCanvasGroup[(int)guideProgress.currentStep]);
        // UI 열기 기다리기
        yield return new WaitForSeconds(nextGuideDuration);
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
        guideProgress.stationCheck = true;
        // 확인 이미지 재설정
        RefreshCheckImage();

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 출구 혼잡도 확인 함수
    public void CheckExit()
    {
        // 출구 혼잡도 확인
        guideProgress.exitCheck = true;
        // 확인 이미지 재설정
        RefreshCheckImage();

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }

    // 역 주변 맛집 확인 함수
    public void CheckFood()
    {
        // 역 주변 맛집 확인
        guideProgress.foodCheck = true;
        // 확인 이미지 재설정
        RefreshCheckImage();

        // 상세 화면을 전부 확인했다면
        if (ReadyToMove)
            // 다음 가이드 UI 열기
            OpenNextGuideUI();
    }
}