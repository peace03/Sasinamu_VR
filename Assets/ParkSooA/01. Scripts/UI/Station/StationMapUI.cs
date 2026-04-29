using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StationMapUI : MonoBehaviour
{
    [Header("역 버튼")]
    [SerializeField] private Button stationButton;

    [Header("상세정보 UI")]
    [SerializeField] private GameObject detailUI;

    [Header("버튼을 눌렀을 때 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnClick;

    private Dictionary<string, ButtonData> buttonPaths = new();         // 버튼 경로들

    private void Awake()
    {
        // 초기화
        gameObject.SetActive(true);
        InitButtons();
    }

    // 버튼 선택 함수
    public void SelectButton(ButtonData data)
    {
        // 노선도 UI가 닫혀있다면
        if (!gameObject.activeSelf)
            // 종료
            return;

        // 지정된 플레이어의 컨트롤러가 UI를 가리키고 있지 않다면
        if(!transform.GetComponentInParent<StationInfo>().IsPlayerUIHovering())
        {
            Debug.Log("지정된 플레이어가 UI를 가리키고 있지 않습니다.");
            // 종료
            return;
        }

        // 실행될 함수가 있다면 실행하기
        OnClick?.Invoke();
        // UI 변경 동기화 요청하기
        transform.GetComponentInParent<StationInfo>().RequestSync(StationInfo.UI_NAME_MAP, data.ButtonPath);
    }

    // 네트워크를 통해 모든 컴퓨터에서 실행될 함수
    public void ExecuteNetworkAction(string buttonPath)
    {
        // 노선도 UI 닫기
        gameObject.SetActive(false);
        // 상세정보 UI 열기
        detailUI.SetActive(true);
    }

    // 버튼들 초기화 함수
    private void InitButtons()
    {
        // 버튼 데이터 컴포넌트를 가지고 있는 자식들을 전부 가져오기
        var datas = transform.GetComponentsInChildren<ButtonData>(true);

        // 버튼 데이터 컴포넌트가 있다면
        if (datas != null)
            // 자식들의 수만큼
            foreach (var data in datas)
            {
                // 버튼 경로 설정
                data.SetButtonPath(transform);

                // 해당 버튼의 경로가 없다면
                if (!buttonPaths.ContainsKey(data.ButtonPath))
                    // 버튼 경로 저장
                    buttonPaths.Add(data.ButtonPath, data);

                // UI가 비어있지 않다면
                if (data.TargetUI != null)
                    // UI 닫기
                    data.TargetUI.SetActive(false);

                // 버튼 컴포넌트가 있다면
                if (data.TryGetComponent<Button>(out var button))
                {
                    // 기존에 있던 버튼의 클릭 기능을 모두 삭제
                    button.onClick.RemoveAllListeners();
                    // 버튼 클릭 기능 추가
                    button.onClick.AddListener(() => SelectButton(data));
                }
            }
    }
}