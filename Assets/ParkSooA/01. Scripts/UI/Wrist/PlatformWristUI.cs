public enum GuideState { Select, Check, Move }

public class GuideProgress
{
    public GuideState currentStep;
    public bool stationCheck;
    public bool exitCheck;
    public bool foodCheck;
}

public class PlatformWristUI : WristUI
{
    private GuideProgress playerProgress = new();
    public GuideProgress CurrentProgress => playerProgress;
}