using UnityEngine;

public enum SubwayState
{
    congestion,
    normal,
    clear,
    standBy
}

//오브젝트 자체를 바꾸는 스크립트
public class ChangeObj : MonoBehaviour
{
    private GameObject congestionObj;
    private GameObject normalObj;
    private GameObject clearObj;
    private GameObject standByObj;

    private void Awake()
    {
        congestionObj = transform.GetChild(0).gameObject;
        normalObj = transform.GetChild(1).gameObject;
        clearObj = transform.GetChild(2).gameObject;
        standByObj = transform.GetChild(3).gameObject;
    }

    public void UpdateObj(SubwayState state)
    {
        //전부 비활성화
        congestionObj.SetActive(false);
        normalObj.SetActive(false);
        clearObj.SetActive(false);
        standByObj.SetActive(false);

        //해당 오브젝트만 활성화
        if (state == SubwayState.congestion) congestionObj.SetActive(true);
        else if (state == SubwayState.normal) normalObj.SetActive(true);
        else if (state == SubwayState.clear) clearObj.SetActive(true);
        else standByObj.SetActive(true);
    }
}
