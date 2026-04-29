using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [Header("카메라로부터의 거리")]
    [SerializeField][Range(0.26f, 1f)] private float distance = 0.8f;

    [Header("UI가 카메라를 따라오는 이동 시간(작을수록 빠름)")]
    [SerializeField][Range(0f, 2f)] private float movingDuration = 0.5f;

    [Header("UI가 카메라를 따라오는 회전 속도(작을수록 느림)")]
    [SerializeField][Range(0f, 100f)] private float rotationSpeed = 5f;

    private Transform target;                               // 타겟(메인 카메라)

    private Vector3 moveVelocity = Vector3.zero;            // 움직였던 속도

    private void Awake()
    {
        // 초기화
        target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // 메인 카메라가 없다면
        if (target == null)
            // 종료
            return;

        // UI를 카메라 위치에 맞춰서 스무스(천천히 -> 빠르게 -> 천천히)하게 움직이기
        transform.position = Vector3.SmoothDamp(transform.position,
            target.position + target.forward * distance, ref moveVelocity, movingDuration);
        // UI를 카메라 각도에 맞춰서 회전하기 
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation,
                                                            rotationSpeed * Time.deltaTime);
    }
}