using UnityEngine;

public enum GuideState { Select, Check, Move }

public struct GuideData
{
    public GuideState state;
    public bool station;
    public bool exit;
    public bool food;

    public GuideData(GuideState state, bool station, bool exit, bool food)
    {
        this.state = state;
        this.station = station;
        this.exit = exit;
        this.food = food;
    }
}

public class PlatformWristUI : WristUI
{
    [Header("손목 UI들")]
    [SerializeField] private GameObject[] wristUI;

    private GuideState playerGuideState = GuideState.Select;                // 플레이어 가이드 상태

    private bool checkStation = false;                                      // 역 혼잡도 확인 여부
    private bool checkExit = false;                                         // 출구 혼잡도 확인 여부
    private bool checkFood = false;                                         // 역 주변 맛집 확인 여부

    public GuideData CurrentProgress => new(playerGuideState, checkStation, checkExit, checkFood);

    // 손목 UI 갱신 함수
    public void UpdateWristUI()
    {
        // 마지막 가이드라면
        if (playerGuideState == GuideState.Move)
            // 종료
            return;

        // 손목 UI 닫기
        wristUI[(int)playerGuideState++].SetActive(false);
        // 다음 손목 UI 열기
        wristUI[(int)playerGuideState].SetActive(true);
    }
}