using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class StationDetailUI: MonoBehaviour
{
    #region 변수
    [Header("노선도 UI")]
    [SerializeField] private GameObject mapUI;

    [Header("메인 UI")]
    [SerializeField] private GameObject mainUI;

    [Header("공용 UI")]
    [SerializeField] private GameObject baseUI;

    [Header("출구 혼잡도 UI")]
    [SerializeField] private Image exitCongestionImage;
    [SerializeField] private Sprite[] exitSprites;

    [Header("버튼을 누르면 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnStation;
    [Space(10)][SerializeField] private UnityEvent OnExit;
    [Space(10)][SerializeField] private UnityEvent OnFood;

    private Dictionary<GameObject, StationDetailButtonData> allButtons = new();     // 모든 버튼들
    private Dictionary<string, StationDetailButtonData> buttonPaths = new();        // 버튼 경로들
    private Stack<GameObject> openedUI = new();                                     // 열린 UI

    private int? playerID = null;                                                   // 플레이어 ID
    #endregion

    private void Awake()
    {
        // 초기화
        InitButtons();
    }

    private void OnEnable()
    {
        // 메인 UI 열기
        ChangeUI(mainUI);
    }

    // 플레이어 ID 설정 함수
    public void SetPlayerID(int? id) => playerID = id;

    // 버튼 선택 함수
    public void SelectButton(SelectEnterEventArgs args)
    {
        // 플레이어 ID와 버튼을 누른 컨트롤러의 ID가 다르다면
        if (playerID != args.interactorObject.transform.GetComponent<PlayerID>().ID)
        {
            Debug.Log($"{args.interactorObject.transform.GetComponent<PlayerID>().ID}와 {playerID}는 다릅니다.");
            // 종료
            return;
        }

        // 누른 버튼에 버튼 데이터가 없다면
        if (!allButtons.TryGetValue(args.interactableObject.transform.gameObject, out var data))
        {
            Debug.Log($"{args.interactableObject.transform.gameObject.name}은 버튼 데이터가 없습니다.");
            // 종료
            return;
        }

        // 버튼 종류에 따라서
        switch (data.ButtonType)
        {
            // 역 혼잡도 버튼이라면
            case StationDetailButtonType.Station:
                // 실행될 함수가 있다면 실행하기
                OnStation?.Invoke();
                break;
            // 출구 혼잡도 UI의 출구 버튼이라면
            case >= StationDetailButtonType.Exit_1 and <= StationDetailButtonType.Exit_4:
                OnExit?.Invoke();
                break;
            // 역 주변 맛집 UI의 출구 버튼이라면
            case StationDetailButtonType.Food:
                OnFood?.Invoke();
                break;
        }

        // UI 변경 동기화 요청하기
        transform.GetComponentInParent<StationInfo>().RequestSync(StationInfo.UI_NAME_DETAIL, data.ButtonPath);
    }

    // 네트워크를 통해 모든 컴퓨터에서 실행될 함수
    public void ExecuteNetworkAction(string buttonPath)
    {
        // 버튼 경로가 없다면
        if (!buttonPaths.TryGetValue(buttonPath, out var data))
        {
            Debug.Log($"{buttonPath}가 등록되어 있지 않습니다.");
            // 종료
            return;
        }

        // 버튼 종류에 따라서
        switch (data.ButtonType)
        {
            // 출구 혼잡도 UI의 출구 버튼이라면
            case >= StationDetailButtonType.Exit_1 and <= StationDetailButtonType.Exit_4:
                // 출구 혼잡도 이미지 변경
                ChangeExitCongestionImage((int)data.ButtonType - (int)StationDetailButtonType.Exit_1);
                break;
            // 뒤로가기 버튼이라면
            case StationDetailButtonType.Back:
                // 뒤로가기 버튼 기능 실행
                SelectBackButton();
                // 종료
                return;
            // 홈 버튼이라면
            case StationDetailButtonType.Home:
                // 홈 버튼 기능 실행
                SelectHomeButton();
                // 종료
                return;
        }

        // UI 변경
        ChangeUI(data.TargetUI);
    }

    // 버튼들 초기화 함수
    private void InitButtons()
    {
        // 버튼 데이터 컴포넌트를 가지고 있는 자식들을 전부 가져오기
        var datas = transform.GetComponentsInChildren<StationDetailButtonData>(true);

        // 버튼 데이터 컴포넌트가 있다면
        if (datas != null)
            // 자식들의 수만큼
            foreach (var data in datas)
            {
                // 버튼 경로 설정
                data.SetButtonPath(transform);

                // 해당 버튼이 없다면
                if (!allButtons.ContainsKey(data.gameObject))
                    // 버튼 저장
                    allButtons.Add(data.gameObject, data);

                // 해당 버튼의 경로가 없다면
                if (!buttonPaths.ContainsKey(data.ButtonPath))
                    // 버튼 경로 저장
                    buttonPaths.Add(data.ButtonPath, data);
            }
    }

    // UI 변경 함수
    private void ChangeUI(GameObject newUI)
    {
        // 새로운 UI가 없다면
        if (newUI == null)
            // 종료
            return;

        // 열린 UI가 있다면
        if (openedUI.Count > 0)
            // 현재 UI 닫기
            openedUI.Peek().SetActive(false);

        // 새로운 UI 열기
        newUI.SetActive(true);
        // 열린 UI에 추가
        openedUI.Push(newUI);
        // 공용 UI 재설정
        RefreshBaseUI();
    }
    
    // UI 닫기 함수
    private void CloseUI(bool showPrevious = false)
    {
        // 열린 UI가 없다면
        if (openedUI.Count <= 0)
            // 종료
            return;

        // UI 닫기
        openedUI.Pop().SetActive(false);
        // 공용 UI 재설정
        RefreshBaseUI();

        // 이전 UI를 보여줘야 한다면
        if (showPrevious)
            // UI 열기
            openedUI.Peek().SetActive(true);
    }

    // 공용 UI 재설정 함수
    private void RefreshBaseUI()
    {
        // 공용 UI가 없다면
        if (baseUI == null)
            // 종료
            return;

        // 메인 UI만 열려있는 상황이 아니라면 공용 UI 열기
        baseUI.SetActive(openedUI.Count > 1);
    }

    // 출구 혼잡도 이미지 변경 함수
    private void ChangeExitCongestionImage(int index) => exitCongestionImage.sprite = exitSprites[index];

    // 돌아가기 버튼 선택 함수
    private void SelectBackButton() => CloseUI(true);

    // 홈 버튼 선택 함수
    private void SelectHomeButton()
    {
        // 열린 UI 개수만큼
        while (openedUI.Count > 0)
            // UI 닫기
            openedUI.Pop().SetActive(false);

        // 상세 정보 UI 닫기
        gameObject.SetActive(false);
        // 노선도 UI 열기
        mapUI.SetActive(true);
    }
}