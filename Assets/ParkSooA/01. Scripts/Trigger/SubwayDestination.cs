using System.Collections;
using UnityEngine;

public class SubwayDestination : ArrivalTrigger
{
    private PlatformWristUI wristUI;        // 손목 UI
    private ArrowPointer arrowPointer;      // 방향 화살표
    private BillBoard billBoard;            // 빌보드
    private SubwayPopupUI subwayUI;         // 지하철 UI
    private Coroutine closeCoroutine;       // 닫기 코루틴

    private void Awake()
    {
        // 초기화
        subwayUI = GetComponentInChildren<SubwayPopupUI>();

        if (subwayUI != null)
        {
            subwayUI.Init(this);
            billBoard = subwayUI.GetComponent<BillBoard>();
        }
    }

    // 지하철 UI 닫기 함수
    public void CloseSubwayUI()
    {
        // 지하철 UI의 닫는 위치 초기화
        subwayUI.SetClosedPosition(subwayUI.OpenedPos);
    }

    // 승강장에 도착했을 때 실행되는 함수
    protected override void OnArrival(GameObject player)
    {
        // 플레이어의 최상위 객체 저장
        var root = player.transform.root;
        // 손목 UI 받아오기
        wristUI = root.GetComponentInChildren<PlatformWristUI>();

        // 손목 UI가 없거나, 승강장 차례가 아니라면
        if (wristUI == null || wristUI?.CurrentProgress.currentStep != GuideState.Move)
            // 종료
            return;

        // 지하철 UI 닫기 연출이 진행 중이라면
        if (closeCoroutine != null)
        {
            // 지하철 UI 닫기 연출 중지
            StopCoroutine(closeCoroutine);
            // 닫기 코루틴 변수 초기화
            closeCoroutine = null;
            
            // 지하철 UI가 있다면
            if (subwayUI != null)
                // 지하철 UI 초기화
                subwayUI.ResetUI(true);
        }

        // 손목 UI 못 열게 바꾸기
        wristUI.SetCanOpenUI(false);

        // 빌보드가 있다면
        if (billBoard != null)
            // 초기화
            billBoard.Init();

        // 지하철 UI가 있다면
        if (subwayUI != null)
        {
            // 지하철 UI가 손목에 있다면
            if (subwayUI.CurUIType == SubwayUIType.Fourth)
            {
                // 지하철 UI 위치 초기화
                subwayUI.ResetUI(false);
                // 지하철 UI 진행상황 초기화
                subwayUI.ResetProgress();
            }

            // 지하철 UI 열기
            subwayUI.OpenUI();
            // 지하철 UI의 닫는 위치 설정
            subwayUI.SetClosedPosition(wristUI.LeftHand);
            // 방향 화살표 저장
            arrowPointer = root.GetComponentInChildren<ArrowPointer>();

            // 방향 화살표가 있다면
            if (arrowPointer != null)
                // 방향 화살표 숨기기
                arrowPointer.ArrowPointerHandler(false);
        }
    }

    // 승강장에서 나갔을 때 실행되는 함수
    protected override void OnExited(GameObject player)
    {
        // 손목 UI가 없거나, 승강장 차례가 아니라면
        if (wristUI == null || wristUI?.CurrentProgress.currentStep != GuideState.Move)
            // 종료
            return;

        // 지하철 UI가 있다면
        if (subwayUI != null)
        {
            // 지하철 UI 진행이 끝났다면
            if (subwayUI.CurUIType == SubwayUIType.Fourth)
            {
                // 손목 UI 변경
                wristUI.ChangeUI(PlatformWristType.Subway);
                // 지하철 UI 닫기 연출 시작
                closeCoroutine = StartCoroutine(CloseSubwayUIRoutine());
            }
            // 지하철 UI 진행이 끝나지 않았다면
            else
            {
                // 지하철 UI의 닫는 위치 초기화
                subwayUI.SetClosedPosition(subwayUI.OpenedPos);
                // 지하철 UI 닫기
                subwayUI.CloseUI();
                // 손목 UI 열 수 있게 바꾸기
                wristUI.SetCanOpenUI(true);
                // 손목 UI 초기화
                wristUI = null;
            }
        }

        // 방향 화살표가 있다면
        if (arrowPointer != null)
        {
            // 방향 화살표 보여주기
            arrowPointer.ArrowPointerHandler(true);
            // 방향 화살표 초기화
            arrowPointer = null;
        }
    }

    // 지하철 UI 닫기 연출 함수
    private IEnumerator CloseSubwayUIRoutine()
    {
        // 지하철 UI 닫기
        subwayUI.CloseUI();
        // 지하철 UI 닫기 기다리기
        yield return new WaitForSeconds(subwayUI.CloseDuration);
        // 지하철 UI의 닫는 위치 초기화
        subwayUI.SetClosedPosition(subwayUI.OpenedPos);
        // 손목 UI 열 수 있게 바꾸기
        wristUI.SetCanOpenUI(true);
        // 손목 UI 초기화
        wristUI = null;
        // 닫기 코루틴 변수 초기화
        closeCoroutine = null;
    }
}