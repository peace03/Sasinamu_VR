using System.Collections;
using UnityEngine;

public enum SubwayUIType { First, Second, Third, Fourth }

public class SubwayPopupUI : PopupUI
{
    [Header("다음 UI로 넘어가는 시간(총 시간은 2배)")]
    [SerializeField][Range(0f, 1f)] private float nextDuration = 0.3f;

    [Header("지하철 UI들")]
    [SerializeField] private GameObject[] subwayUis;

    [Header("마지막 페이지 보여주는 시간")]
    [SerializeField][Range(5f, 15f)] private float lastPageOpenDuration = 10f;

    private CanvasGroup[] subCanvasGroup;                           // 지하철 UI들의 개별 캔버스 그룹

    private SubwayDestination destination;                          // 승강장
    private Coroutine nextPageCoroutine;                            // 다음 페이지 코루틴 변수
    private Coroutine lastPageCoroutine;                            // 마지막 페이지 코루틴 변수]

    private SubwayUIType curUIType = SubwayUIType.First;            // 현재 UI 종류
    private SubwayUIType targetUIType;                              // 목표 UI

    public SubwayUIType CurUIType => curUIType;

    private void Awake()
    {
        // 지하철 UI가 있다면
        if (subwayUis != null)
        {
            // 지하철 UI들의 개수만큼 개별 캔버스 그룹 개수를 정하기
            subCanvasGroup = new CanvasGroup[subwayUis.Length];

            // 지하철 UI 개수만큼
            for (int i = 0; i < subwayUis.Length; i++)
            {
                // 지하철 UI가 비어있다면
                if (subwayUis[i] == null)
                    // 건너뛰기
                    continue;
                // 캔버스 그룹이 없다면
                else if (!subwayUis[i].TryGetComponent<CanvasGroup>(out var canvas))
                {
                    Debug.Log($"{i}번째 지하철 UI에 캔버스 그룹 없음");
                    // 건너뛰기
                    continue;
                }
                else
                    // 캔버스 그룹 저장
                    subCanvasGroup[i] = canvas;
            }
        }
    }

    // 초기화 함수
    public void Init(SubwayDestination destination) => this.destination = destination;

    // 진행상황 초기화 함수
    public void ResetProgress() => curUIType = SubwayUIType.First;

    // UI 열기 함수
    public void OpenUI()
    {
        // 지하철 UI 재설정
        RefreshSubwayUI();
        // 레이저가 UI를 인식할 수 있게 하기
        CanvasGroup.blocksRaycasts = true;
        // 팝업 열기 연출 시작
        PopupUIHandler(true);
    }

    // 지하철 UI 재설정 함수
    private void RefreshSubwayUI()
    {
        // 지하철 UI 모두 닫기
        foreach (var ui in subwayUis)
            ui.SetActive(false);

        // 현재 UI 열기
        subwayUis[(int)curUIType].SetActive(true);
        // 개별 캔버스 그룹 재설정
        RefreshSubCanvasGroup();
    }

    // 개별 캔버스 그룹 재설정 함수
    private void RefreshSubCanvasGroup()
    {
        // 지하철 UI 모두 투명하게 바꾸기
        foreach (var canvas in subCanvasGroup)
            canvas.alpha = 0f;

        // 현재 UI 불투명하게 바꾸기
        subCanvasGroup[(int)curUIType].alpha = 1f;
    }

    // UI 닫기 함수
    public void CloseUI()
    {
        // 다음 페이지 연출 중지
        StopNextPageRoutine();
        // 마지막 페이지 연출 중지
        StopLastPageRouine();
        // 레이저가 UI를 인식할 수 없게 하기
        CanvasGroup.blocksRaycasts = false;
        // 팝업 닫기 연출 시작
        PopupUIHandler(false);
    }

    // UI 초기화
    public void ResetUI(bool allReset)
    {
        // 팝업 UI 상태 설정
        SetPopupUIState(false);
        // 닫는 위치 초기화
        SetClosedPosition(OpenedPos);
        // 이동 조절 연출 중지
        StopUIMove();
        // UI 여는 위치로 초기화
        SetUIMove(false, 0f);

        // 전부 초기화한다면
        if (allReset)
        {
            // 크기 조절 연출 중지
            StopUIScale();
            // UI 닫는 크기로 초기화
            SetUIScale(false, 0f);
            // 불투명도 조절 연출 중지
            StopUIFade();
            // UI 투명으로 초기화
            SetUIFade(false, 0f);
        }
    }

    // 다음 페이지 변경 함수
    public void ChangeNextPage()
    {
        // 마지막 페이지이거나, 다음 페이지 코루틴 변수가 비어있지 않다면
        if (curUIType == SubwayUIType.Fourth || nextPageCoroutine != null)
            // 종료
            return;

        // 현재 UI 종류에 따라서
        switch (curUIType)
        {
            // 첫번째라면
            case SubwayUIType.First:
                // 목표 UI 설정
                targetUIType = SubwayUIType.Second;
                break;
            case SubwayUIType.Second:
                targetUIType = SubwayUIType.Third;
                break;
            case SubwayUIType.Third:
                targetUIType = SubwayUIType.Fourth;
                break;
        }

        // 다음 페이지 연출 시작
        nextPageCoroutine = StartCoroutine(NextPageRoutine());
    }

    // 다음 페이지 연출 함수
    private IEnumerator NextPageRoutine()
    {
        // 레이저가 UI를 인식할 수 없게 하기
        CanvasGroup.blocksRaycasts = false;
        // 페이지 닫기
        SetUIFade(false, nextDuration, subCanvasGroup[(int)curUIType]);
        // 페이지 닫기 기다리기
        yield return new WaitForSeconds(nextDuration);
        // 현재 페이지 닫기
        subwayUis[(int)curUIType++].SetActive(false);
        // 다음 페이지 열기
        subwayUis[(int)curUIType].SetActive(true);
        // 레이저가 UI를 인식할 수 있게 하기
        CanvasGroup.blocksRaycasts = true;
        // 페이지 열기
        SetUIFade(true, nextDuration, subCanvasGroup[(int)curUIType]);
        // 페이지 열기 기다리기
        yield return new WaitForSeconds(nextDuration);
        // 다음 페이지 코루틴 변수 초기화
        nextPageCoroutine = null;

        // 마지막 페이지라면
        if (curUIType == SubwayUIType.Fourth)
            // 마지막 페이지 연출 시작
            lastPageCoroutine = StartCoroutine(LastPageRoutine());
    }

    // 마지막 페이지 연출 함수
    private IEnumerator LastPageRoutine()
    {
        // 마지막 페이지 보여주는 거 기다리기
        yield return new WaitForSeconds(lastPageOpenDuration);
        // 레이저가 UI를 인식할 수 없게 하기
        CanvasGroup.blocksRaycasts = false;
        // UI 닫기
        PopupUIHandler(false);
        // UI 닫기 기다리기
        yield return new WaitForSeconds(CloseDuration);
        // 마지막 페이지 코루틴 변수 초기화
        lastPageCoroutine = null;
        // 지하철 UI 닫기
        destination.CloseSubwayUI();
    }

    // 다음 페이지 연출 중지 함수
    private void StopNextPageRoutine()
    {
        // 다음 페이지 코루틴 변수가 비어있다면
        if (nextPageCoroutine == null)
            // 종료
            return;

        // 다음 페이지 연출 중지
        StopCoroutine(nextPageCoroutine);
        // 현재 UI 종류를 목표 UI로 변경
        curUIType = targetUIType;
        // 다음 페이지 코루틴 변수 초기화
        nextPageCoroutine = null;
    }

    // 마지막 페이지 연출 중지 함수
    private void StopLastPageRouine()
    {
        // 마지막 페이지 코루틴 변수가 비어있다면
        if (lastPageCoroutine == null)
            // 종료
            return;

        // 마지막 페이지 연출 중지
        StopCoroutine(lastPageCoroutine);
        // 마지막 페이지 코루틴 변수 초기화
        lastPageCoroutine = null;
    }
}