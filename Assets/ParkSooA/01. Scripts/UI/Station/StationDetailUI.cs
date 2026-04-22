using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StationDetailState
{
    None,
    Main,
    Station,
    Exit_Select,
    Exit_Congestion,
    Food_Select,
    Food_List
}

public class StationDetailUI: MonoBehaviour
{
    #region 변수
    [Header("노선도 UI")]
    [SerializeField] private GameObject mapUI;

    [Header("메인 UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private Button stationButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button foodbutton;

    [Header("공통 UI")]
    [SerializeField] private GameObject baseUI;
    [SerializeField] private Button backButton;
    [SerializeField] private Button homeButton;

    [Header("역 혼잡도 UI")]
    [SerializeField] private GameObject stationUI;

    [Header("출구 혼잡도 UI")]
    [SerializeField] private GameObject exitSelectUI;
    [SerializeField] private GameObject exitCongestionUI;
    [SerializeField] private Image exitCongestionImage;
    [SerializeField] private Button[] exitButtons;
    [SerializeField] private Sprite[] exitSprites;

    [Header("주변 맛집 UI")]
    [SerializeField] private GameObject foodSelectUI;
    [SerializeField] private GameObject foodListUI;
    [SerializeField] private Image foodListImage;
    [SerializeField] private Button[] foodButtons;
    [SerializeField] private Sprite[] foodSprites;

    private List<GameObject> allUIList = new();                         // 모든 UI들 리스트

    private StationDetailState curState = StationDetailState.None;      // 현재 UI 상태
    #endregion

    private void Awake()
    {
        // UI
        InitUI(mainUI);
        InitUI(baseUI);
        InitUI(stationUI);
        InitUI(exitSelectUI);
        InitUI(exitCongestionUI);
        InitUI(foodSelectUI);
        InitUI(foodListUI);
        // 버튼들
        InitButtons();
    }

    private void OnEnable()
    {
        // 메인 UI 열기
        OpenUI(StationDetailState.Main);
    }

    // UI 초기화 함수
    private void InitUI(GameObject newUI)
    {
        // 모든 UI들 리스트에 추가
        allUIList.Add(newUI);
        // UI 닫기
        newUI.SetActive(false);
    }

    // 버튼들 초기화 함수
    private void InitButtons()
    {
        // 역 혼잡도 버튼
        stationButton.onClick.AddListener(() => OpenUI(StationDetailState.Station));
        // 출구 혼잡도 버튼
        exitButton.onClick.AddListener(() => OpenUI(StationDetailState.Exit_Select));
        // 출구별 주변 맛집 버튼
        foodbutton.onClick.AddListener(() => OpenUI(StationDetailState.Food_Select));
        // 돌아가기 버튼
        backButton.onClick.AddListener(SelectBackButton);
        // 홈 버튼
        homeButton.onClick.AddListener(SelectHomeButton);

        // 출구 선택 버튼들
        for (int i = 0; i < exitButtons.Length; i++)
        {
            // 람다식(미래 실행)은 현재의 i를 기억하지 못하므로 따로 저장
            int index = i;
            // 출구 혼잡도 이미지 변경
            exitButtons[i].onClick.AddListener(() => ChangeExitCongestionImage(index));
            // 출구 혼잡도 UI 열기
            exitButtons[i].onClick.AddListener(() => OpenUI(StationDetailState.Exit_Congestion));
        }

        // 맛집 출구 선택 버튼들
        for (int i = 0; i < foodButtons.Length; i++)
        {
            // 람다식(미래 실행)은 현재의 i를 기억하지 못하므로 따로 저장
            int index = i;
            // 맛집 리스트 이미지 변경
            foodButtons[i].onClick.AddListener(() => ChangeFoodListImage(index));
            // 맛집 리스트 UI 열기
            foodButtons[i].onClick.AddListener(() => OpenUI(StationDetailState.Food_List));
        }
    }

    // UI 열기 함수
    private void OpenUI(StationDetailState state)
    {
        // 모든 UI 닫기
        foreach (var ui in allUIList)
            ui.SetActive(false);

        // 현재 UI 상태 변경
        curState = state;
        // 공통 UI 열기
        baseUI.SetActive(true);

        // UI 상태에 따라
        switch (state)
        {
            // 메인이라면
            case StationDetailState.Main:
                // 공통 UI 닫기
                baseUI.SetActive(false);
                // 메인 UI 열기
                mainUI.SetActive(true);
                break;
            // 역 혼잡도라면
            case StationDetailState.Station:
                // 역 혼잡도 UI 열기
                stationUI.SetActive(true);
                break;
            // 출구 선택이라면
            case StationDetailState.Exit_Select:
                // 출구 선택 UI 열기
                exitSelectUI.SetActive(true);
                break;
            // 출구 혼잡도라면
            case StationDetailState.Exit_Congestion:
                // 출구 혼잡도 UI 열기
                exitCongestionUI.SetActive(true);
                break;
            // 맛집 출구 선택이라면
            case StationDetailState.Food_Select:
                // 맛집 출구 선택 UI 열기
                foodSelectUI.SetActive(true);
                break;
            // 맛집 리스트라면
            case StationDetailState.Food_List:
                // 맛집 리스트 UI 열기
                foodListUI.SetActive(true);
                break;
            // 그 외라면
            default:
                Debug.Log("없는 UI 상태 입니다.");
                break;
        }
    }

    // 돌아가기 버튼 선택 함수
    private void SelectBackButton()
    {
        // 현재 UI 상태에 따라서
        switch (curState)
        {
            // 역 혼잡도라면
            case StationDetailState.Station:
            // 출구 선택이라면
            case StationDetailState.Exit_Select:
            // 맛집 출구 선택이라면
            case StationDetailState.Food_Select:
                // 메인 UI 열기
                OpenUI(StationDetailState.Main);
                break;
            // 출구 혼잡도라면
            case StationDetailState.Exit_Congestion:
                // 출구 선택 UI 열기
                OpenUI(StationDetailState.Exit_Select);
                break;
            // 맛집 리스트라면
            case StationDetailState.Food_List:
                // 맛집 출구 선택 UI 열기
                OpenUI(StationDetailState.Food_Select);
                break;
        }
    }

    // 홈 버튼 선택 함수
    private void SelectHomeButton()
    {
        // 상세 정보 UI 닫기
        gameObject.SetActive(false);
        // 노선도 UI 열기
        mapUI.SetActive(true);
    }

    // 출구 혼잡도 이미지 변경 함수
    private void ChangeExitCongestionImage(int index) => exitCongestionImage.sprite = exitSprites[index];

    // 맛집 리스트 이미지 변경 함수
    private void ChangeFoodListImage(int index) => foodListImage.sprite = foodSprites[index];
}