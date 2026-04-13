using UnityEngine;

public class ChangeTextObj : MonoBehaviour
{
    [Tooltip("혼잡도 표시할 객차 번호")]
    [SerializeField] private int sectionNum;
    [Header("탑승 인원수 별 임계치")]
    [Tooltip("혼잡")]
    [SerializeField] private int congestion;
    [Tooltip("보통")]
    [SerializeField] private int normal;
    [Tooltip("여유")]
    [SerializeField] private int clear;

    private GameObject congestionObj;   //프리펩
    private GameObject normalObj;       //프리펩
    private GameObject clearObj;        //프리펩
    private GameObject standByObj;        //프리펩

    private void Awake()
    {
        //프리펩 연결
        congestionObj = transform.GetChild(0).gameObject;
        normalObj = transform.GetChild(1).gameObject;
        clearObj = transform.GetChild(2).gameObject;
        standByObj = transform.GetChild(3).gameObject;

        //프리펩 비활성화
        SetActiveFalse();
    }

    //탑승객 수 받기 UnityEvent 연결
    public void SetPassengerCount(int[] passengerCount)
    {
        ChooseText(passengerCount[sectionNum-1]);
    }
    //대기 상태 UnityEvent 연결
    public void SetStandBy()
    {
        SetActiveFalse();
        standByObj.SetActive(true);
    }

    //오브젝트 선택
    private void ChooseText(int passengerCount)
    {
        SetActiveFalse();

        //혼잡도별 텍스트 오브젝트 활성화
        if (passengerCount >= 10) congestionObj.SetActive(true);
        else if (passengerCount >= 5) normalObj.SetActive(true);
        else clearObj.SetActive(true);
    }

    //프리펩 비활성화
    private void SetActiveFalse()
    {
        congestionObj.SetActive(false);
        normalObj.SetActive(false);
        clearObj.SetActive(false);
        standByObj.SetActive(false);
    }
}
