using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WristUI : MonoBehaviour
{
    [Header("팝업 UI")]
    [SerializeField] private PopupUI popupUI;

    [Header("UI가 켜지는 시야 각도")]
    [SerializeField][Range(0f, 1f)] private float viewThreshold = 0.7f;

    [Header("UI가 켜지는 거리")]
    [SerializeField][Range(0f, 1f)] private float minDistance = 0.35f;
    [SerializeField][Range(0f, 1f)] private float maxDistance = 0.65f;

    private Transform target;                   // 메인 카메라
    private XRRayInteractor leftHandRay;        // 왼쪽 컨트롤러 레이

    private bool canOpenUI = false;             // UI 열기 가능 여부

    private void Awake()
    {
        target = Camera.main.transform;
        leftHandRay = GetComponentInParent<XRRayInteractor>();
    }

    private void Update()
    {
        // 메인 카메라가 없거나, UI 열기가 가능하지 않다면
        if (target == null || !canOpenUI)
            // 종료
            return;

        // 손목과의 거리, 각도 확인하기
        bool leftHandCheck = CheckDistanceAndRotation();

        // 왼쪽 컨트롤러에 레이가 있고, 상태를 바꿀 필요가 있다면
        if (leftHandRay != null && leftHandRay.enabled == leftHandCheck)
            // 손목과의 거리, 각도에 따라서 레이 상태 바꾸기
            leftHandRay.enabled = !leftHandCheck;

        // 거리와 각도에 따른 결과로 팝업 UI 열거나 닫기
        popupUI.PopupUIHandler(leftHandCheck);
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
    public void SetCanOpenUI(bool value) => canOpenUI = value;
}