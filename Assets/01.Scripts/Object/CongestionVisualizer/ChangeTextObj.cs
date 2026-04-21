using UnityEngine;

public class ChangeTextObj : MonoBehaviour
{
    [Header("탑승 인원수 별 임계치")]
    private int congestion; //혼잡
    private int normal;     //보통
    private int clear;      //여유

    private GameObject congestionObj;   //혼잡 오브젝트
    private GameObject normalObj;       //보통 오브젝트
    private GameObject clearObj;        //여유 오브젝트
    private GameObject standByObj;      //대기 오브젝트
    private ChangeMaterialObj QuadObj;  //쿼드 오브젝트

    public void Init(int congestion, int normal, int clear)
    {
        //탑승 인원수별 임계치 초기화
        this.congestion = congestion;
        this.normal = normal;
        this.clear = clear;

        //프리펩 연결
        congestionObj = transform.GetChild(0).gameObject;
        normalObj = transform.GetChild(1).gameObject;
        clearObj = transform.GetChild(2).gameObject;
        standByObj = transform.GetChild(3).gameObject;
        QuadObj = transform.GetChild(4).GetComponent<ChangeMaterialObj>();
        QuadObj.Init(congestion, normal, clear);

        //프리펩 비활성화
        SetActiveFalse();
    }

    //탑승객 수 받기
    public void SetPassengerCount(int passengerCount)
    {
        ChooseText(passengerCount);
    }
    //대기 상태
    public void SetStandBy()
    {
        SetActiveFalse();
        QuadObj.SetStandByColor();
        standByObj.SetActive(true);
    }

    //오브젝트 선택
    private void ChooseText(int passengerCount)
    {
        SetActiveFalse();
        //혼잡도별 타일 색상 변경
        QuadObj.SetMaterialColor(passengerCount);
        //혼잡도별 텍스트 오브젝트 활성화
        if (passengerCount >= congestion) congestionObj.SetActive(true);
        else if (passengerCount >= normal) normalObj.SetActive(true);
        else if (passengerCount >= clear) clearObj.SetActive(true);
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
