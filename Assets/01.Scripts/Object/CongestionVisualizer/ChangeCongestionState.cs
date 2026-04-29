using UnityEngine;

//혼잡도에 따라 하위 오브젝트들을 컨트롤하는 스크립트
public class ChangeCongestionState : MonoBehaviour
{
    [Header("FloorEmission Material")]
    [SerializeField] private Material congestionMat_Floor;
    [SerializeField] private Material normalMat_Floor;
    [SerializeField] private Material clearMat_Floor;
    [SerializeField] private Material standByMat_Floor;

    [Header("Light Material")]
    [SerializeField] private Material congestionMat_Light;
    [SerializeField] private Material normalMat_Light;
    [SerializeField] private Material clearMat_Light;
    [SerializeField] private Material standByMat_Light;

    [Header("TextCornDoor Color")]
    [SerializeField] private Color congestionColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color clearColor;
    [SerializeField] private Color standByColor;

    //탑승 인원수 별 임계치
    private int congestion; //혼잡
    private int normal;     //보통
    private int clear;      //여유

    //머테리얼 바꿀 오브젝트들
    private ChangeMaterial floorEmissionObj; //FloorEmission 오브젝트
    private LightObjsHandler lightObj;         //Light 오브젝트

    //오브젝트를 변경할 부모 오브젝트
    private ChangeObj texts;

    //머테리얼 색상 바꿀 오브젝트
    private ChangeMatColor textCorn;

    public void Init(int congestion, int normal, int clear)
    {
        //탑승 인원수별 임계치 초기화
        this.congestion = congestion;
        this.normal = normal;
        this.clear = clear;

        //FloorEmission 오브젝트 연결
        floorEmissionObj = transform.GetChild(0).GetComponent<ChangeMaterial>();
        //FloorEmissionObj.Init(congestion, normal, clear);

        //Light 오브젝트 연결
        lightObj = transform.GetChild(1).GetComponent<LightObjsHandler>();

        //Texts 빈 오브젝트 연결
        texts = transform.GetChild(2).GetComponent<ChangeObj>();

        //TextCorn_Door 오브젝트 연결
        textCorn = transform.GetChild(3).GetComponent<ChangeMatColor>();

        //프리펩 비활성화
        //SetActiveFalse();
    }

    //탑승객 수 받기
    public void SetPassengerCount(int passengerCount)
    {
        if (passengerCount >= congestion)
        {
            floorEmissionObj.SetMaterial(congestionMat_Floor);
            lightObj.SetMaterials(congestionMat_Light);
            texts.UpdateObj(SubwayState.congestion);
            textCorn.SetMatColor(congestionColor);
        }
        else if (passengerCount >= normal)
        {
            floorEmissionObj.SetMaterial(normalMat_Floor);
            lightObj.SetMaterials(normalMat_Light);
            texts.UpdateObj(SubwayState.normal);
            textCorn.SetMatColor(normalColor);
        }
        else
        {
            floorEmissionObj.SetMaterial(clearMat_Floor);
            lightObj.SetMaterials(clearMat_Light);
            texts.UpdateObj(SubwayState.clear);
            textCorn.SetMatColor(clearColor);
        }
    }
    //대기 상태
    public void SetStandBy()
    {
        floorEmissionObj.SetMaterial(standByMat_Floor);
        lightObj.SetMaterials(standByMat_Light);
        texts.UpdateObj(SubwayState.standBy);
        textCorn.SetMatColor(standByColor);
    }
}
