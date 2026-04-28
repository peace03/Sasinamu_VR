using UnityEngine;

public enum StationDetailButtonType { None, Station, Exit_1, Exit_2, Exit_3, Exit_4, Food, Back, Home }

public class StationDetailButtonData : ButtonData
{
    [Header("버튼 종류")]
    [SerializeField] private StationDetailButtonType buttonType = StationDetailButtonType.None;
    public StationDetailButtonType ButtonType => buttonType;
}