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

    private void Awake()
    {
        // 초기화
        gameObject.SetActive(true);
        stationButton.onClick.AddListener(OpenDetailUI);
    }

    // 상세 정보 UI 열기 함수
    private void OpenDetailUI()
    {
        // 노선도 UI 닫기
        gameObject.SetActive(false);
        // 상세 정보 UI 열기
        detailUI.SetActive(true);
        // 실행될 함수가 있다면 실행하기
        OnClick?.Invoke();
    }
}