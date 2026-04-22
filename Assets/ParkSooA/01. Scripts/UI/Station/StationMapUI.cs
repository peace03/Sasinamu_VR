using UnityEngine;
using UnityEngine.UI;

public class StationMapUI : MonoBehaviour
{
    [Header("역 버튼")]
    [SerializeField] private Button stationButton;

    [Header("상세정보 UI")]
    [SerializeField] private GameObject detailUI;

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
    }
}