using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WristUI : MonoBehaviour
{
    [Header("현재 씬 종류")]
    [SerializeField] private LoadScene curScene;

    [Header("팝업 UI")]
    [SerializeField] private PopupUI popupUI;

    [Header("UI가 켜지는 시야 각도")]
    [SerializeField][Range(0f, 1f)] private float viewThreshold = 0.7f;

    [Header("UI가 켜지는 거리")]
    [SerializeField][Range(0f, 1f)] private float minDistance = 0.35f;
    [SerializeField][Range(0f, 1f)] private float maxDistance = 0.65f;

    private Transform target;                   // 메인 카메라
    private XRRayInteractor leftHandRay;        // 왼쪽 컨트롤러 레이
    private WristUIBillBoard billBoard;         // 빌보드

    private bool canOpenUI = false;             // UI 열기 가능 여부
    private bool checkLeftHand;                 // 왼쪽 손 확인 여부
    
    public Transform LeftHand => leftHandRay.transform;

    public bool CanRotate => canOpenUI && checkLeftHand;

    private void Awake()
    {
        // 현재 씬이 시작 씬이라면
        if (curScene == LoadScene.StartScene)
            // 초기화
            Init();
    }

    // 초기화 함수
    public void Init()
    {
        target = Camera.main.transform;
        leftHandRay = GetComponentInParent<XRRayInteractor>();
        billBoard = GetComponentInChildren<WristUIBillBoard>();
        billBoard?.Init(this);
    }

    private void Update()
    {
        // 메인 카메라가 없거나, UI 열기가 가능하지 않다면
        if (target == null || !canOpenUI)
            // 종료
            return;

        // 손목과의 거리, 각도 확인하기
        checkLeftHand = CheckDistanceAndRotation();

        // 왼쪽 컨트롤러에 레이가 있고, 상태를 바꿀 필요가 있다면
        if (leftHandRay != null && leftHandRay.enabled == checkLeftHand)
            // 손목과의 거리, 각도에 따라서 레이 상태 바꾸기
            leftHandRay.enabled = !checkLeftHand;

        // 팝업 UI가 있다면
        if (popupUI != null)
            // 거리와 각도에 따른 결과로 팝업 UI 열거나 닫기
            popupUI.PopupUIHandler(checkLeftHand);
    }

    // 거리와 각도 확인 함수
    private bool CheckDistanceAndRotation()
    {
        // 각도 저장
        float dot = Vector3.Dot(transform.right, target.forward);
        // 거리 저장
        float dis = Vector3.Distance(transform.position, target.position);
        // 거리와 각도가 조건에 맞는지 확인
        return dot >= viewThreshold && dis >= minDistance && dis <= maxDistance;
    }

    // UI 열기 가능 여부 설정 함수
    public void SetCanOpenUI(bool value)
    {
        canOpenUI = value;

        // UI를 못 열게 할거라면
        if (!value)
        {
            // 팝업 UI가 있다면
            if (popupUI != null)
                // 팝업 UI 닫기
                popupUI.PopupUIHandler(false);

            // 왼쪽 컨트롤러가 있다면
            if (leftHandRay != null)
                // 레이 켜기
                leftHandRay.enabled = true;
        }
    }
}