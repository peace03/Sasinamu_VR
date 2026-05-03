using UnityEngine;

public class WristUIBillBoard : MonoBehaviour
{
    [Header("회전 속도")]
    [SerializeField][Range(0f, 100f)] private float rotationSpeed = 15f;

    private WristUI wristUI;        // 손목 UI
    private Transform target;       // 타겟

    private void Update()
    {
        // 타겟이 없거나, 회전이 불가능한 상태라면
        if (target == null || !wristUI.CanRotate)
            // 종료
            return;

        // 방향 구하기
        Vector3 dir = transform.position - target.position;
        // 카메라의 위쪽 방향(수평 유지)으로 회전 값 구하기
        Quaternion newRot = Quaternion.LookRotation(dir, target.up);
        // 회전하기
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
    }

    // 초기화 함수
    public void Init(WristUI wristUI)
    {
        // 초기화
        this.wristUI = wristUI;

        if (target == null)
            target = Camera.main.transform;
    }
}