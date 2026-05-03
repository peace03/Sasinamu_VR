using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [Header("회전 속도")]
    [SerializeField][Range(0f, 100f)] private float rotationSpeed = 10f;

    private Transform target;

    private void LateUpdate()
    {
        // 타겟이 없다면
        if (target == null)
            // 종료
            return;

        // 플레이어가 바라보는 방향 구하기
        Vector3 dir = target.position - transform.position;
        // 높이 제거
        dir.y = 0f;

        // 방향이 다르다면
        if (dir != Vector3.zero)
        {
            // 회전 값 구하기
            Quaternion newRot = Quaternion.LookRotation(-dir);
            // 회전하기
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
        }
    }

    // 초기화 함수
    public void Init()
    {
        // 타겟이 없다면
        if (target == null)
            // 초기화
            target = Camera.main.transform;
    }
}