using UnityEngine;

public class WristUI : MonoBehaviour
{
    [Header("팝업 UI")]
    [SerializeField] private PopupUI popupUI;

    [Header("UI가 켜지는 시야 각도")]
    [SerializeField][Range(0f, 1f)] private float viewThreshold = 0.7f;

    [Header("UI가 켜지는 거리")]
    [SerializeField][Range(0f, 1f)] private float minDistance = 0.35f;
    [SerializeField][Range(0f, 1f)] private float maxDistance = 0.65f;

    private Transform target;       // 메인 카메라

    private void Awake()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        // 메인 카메라가 없다면
        if (target == null)
            // 종료
            return;

        // 거리와 각도에 따른 결과로 팝업 UI 열거나 닫기
        popupUI.PopupUIHandler(CheckDistanceAndRotation());
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
}