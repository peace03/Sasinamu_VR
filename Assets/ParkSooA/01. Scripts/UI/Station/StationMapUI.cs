using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class StationMapUI : MonoBehaviour
{
    [Header("역 버튼")]
    [SerializeField] private Button stationButton;

    [Header("상세정보 UI")]
    [SerializeField] private GameObject detailUI;

    [Header("버튼을 눌렀을 때 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnClick;

    private Dictionary<GameObject, ButtonData> allButtons = new();      // 모든 버튼들
    private Dictionary<string, ButtonData> buttonPaths = new();         // 버튼 경로들

    private int? playerID = null;                                       // 플레이어 ID

    private void Awake()
    {
        // 초기화
        gameObject.SetActive(true);
        InitButtons();
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

        Debug.Log($"{args.interactorObject.transform.GetComponent<PlayerID>().ID} / {playerID}");

        // 누른 버튼에 버튼 데이터가 없다면
        if (!allButtons.TryGetValue(args.interactableObject.transform.gameObject, out var data))
        {
            Debug.Log($"{args.interactableObject.transform.gameObject.name}은 버튼 데이터가 없습니다.");
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

                // 해당 버튼이 없다면
                if (!allButtons.ContainsKey(data.gameObject))
                    // 버튼 저장
                    allButtons.Add(data.gameObject, data);

                // 해당 버튼의 경로가 없다면
                if (!buttonPaths.ContainsKey(data.ButtonPath))
                    // 버튼 경로 저장
                    buttonPaths.Add(data.ButtonPath, data);

                // UI가 비어있지 않다면
                if (data.TargetUI != null)
                    // UI 닫기
                    data.TargetUI.SetActive(false);
            }
    }
}