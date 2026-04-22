using UnityEngine;
using UnityEngine.Events;

public class ArrivalTrigger : MonoBehaviour
{
    [Header("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayer;

    [Header("플레이어가 도착하면 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnEnter;

    [Header("플레이어가 나가면 실행될 함수")]
    [Space(10)][SerializeField] private UnityEvent OnExit;

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어가 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0)
            // 종료
            return;

        // 실행될 함수가 있다면 실행하기
        OnEnter?.Invoke();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // 나간 물체가 플레이어가 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0)
            // 종료
            return;

        // 실행될 함수가 있다면 실행하기
        OnExit?.Invoke();
    }
}