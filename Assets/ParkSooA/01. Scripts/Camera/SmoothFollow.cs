using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Vector3 offsetPosition = new(0f, -0.1f, 0f);
    [SerializeField] private Vector3 offsetRotation = Vector3.zero;

    [Header("카메라와의 거리")]
    [SerializeField][Range(0.16f, 1f)] private float distance = 0.8f;

    [Header("UI가 카메라를 따라오는 이동 시간(작을수록 빠름)")]
    [SerializeField][Range(0f, 2f)] private float movingDuration = 0.5f;

    [Header("UI가 카메라를 따라오는 회전 속도(작을수록 느림)")]
    [SerializeField][Range(0f, 100f)] private float rotationSpeed = 5f;

    [Header("카메라")]
    [SerializeField] private Transform target;

    private Vector3 moveVelocity = Vector3.zero;            // 움직였던 속도

    public void Init()
    {
        if (target == null)
            // 초기화
            target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // 메인 카메라가 없다면
        if (target == null)
            // 종료
            return;

        // 새로운 위치 정하기(카메라의 위치 + 카메라가 바라보고 있는 앞뒤 방향 * 카메라와의 거리
        //                 + 카메라가 바라보고 있는 좌우 방향 * UI의 좌우 좌표 + 카메라가 바라보고 있는 상하 방향 * UI의 상하 좌표)
        var newPos = target.position + target.forward * distance + target.right * offsetPosition.x
                                                                                + target.up * offsetPosition.y;
        // UI를 카메라 위치에 맞춰서 스무스(천천히 -> 빠르게 -> 천천히)하게 움직이기
        transform.position = Vector3.SmoothDamp(transform.position, newPos + target.forward * distance,
                                                                                ref moveVelocity, movingDuration);
        // 새로운 각도 정하기
        var newRot = target.rotation * Quaternion.Euler(offsetRotation);
        // UI를 카메라 각도에 맞춰서 회전하기 
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
    }
}