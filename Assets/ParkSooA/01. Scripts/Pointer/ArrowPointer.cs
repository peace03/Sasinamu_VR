using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private GuideProgress guideProgress;        // 플레이어 가이드 진행 상황

    private int arrowPointerIndex = 0;          // 방향 화살표 인덱스

    private void OnEnable()
    {
        // 현재 가이드 진행 상황에 따라 방향 화살표 인덱스 설정
        arrowPointerIndex = guideProgress.currentStep == GuideState.Move ? 1 : 0;
        // 초기화
        SetPositionAndRotation();
    }

    private void LateUpdate()
    {
        // 위치와 각도 설정
        SetPositionAndRotation();
    }

    // 초기화 함수
    public void Init(GuideProgress progress) => guideProgress = progress;

    // 방향 화살표 관리 함수
    public void ArrowPointerHandler(bool isShow) => gameObject.SetActive(isShow);

    // 위치와 각도 설정 함수
    private void SetPositionAndRotation()
    {
        // 위치 고정
        transform.position = ArrowOffset.Instance.Positions[arrowPointerIndex];
        // 각도 고정
        transform.rotation = ArrowOffset.Instance.Rotations[arrowPointerIndex];
    }
}