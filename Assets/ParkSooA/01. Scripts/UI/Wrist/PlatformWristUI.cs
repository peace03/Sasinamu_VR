using UnityEngine;

public enum GuideState { Select, Check, Move }
public enum PlatformWristType { Station, Move, Subway }

public class GuideProgress
{
    public GuideState currentStep;
    public bool stationCheck;
    public bool exitCheck;
    public bool foodCheck;
}

public class PlatformWristUI : WristUI
{
    [Header("손목 UI들")]
    [SerializeField] private GameObject[] wristUis;

    private GuideProgress playerProgress = new();                       // 가이드 진행상황
    public GuideProgress CurrentProgress => playerProgress;

    private PlatformWristType curType = PlatformWristType.Station;      // 현재 손목 UI 종류

    // UI 변경 함수
    public void ChangeUI(PlatformWristType type)
    {
        // 현재 UI 종류와 같다면
        if (curType == type)
            // 종료
            return;

        // 현재 UI 닫기
        wristUis[(int)curType].SetActive(false);
        // 새로운 UI 열기
        wristUis[(int)type].SetActive(true);
        // 현재 UI 종류 변경
        curType = type;
    }
}