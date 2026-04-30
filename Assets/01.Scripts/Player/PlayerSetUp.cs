using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.XR; // TrackedPoseDriver용
using UnityEngine.XR.Interaction.Toolkit; // XR 컴포넌트용

public class PlayerSetup : MonoBehaviourPun
{
    [Header("시각 및 청각 제어")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioListener audioListener;

    [Header("HMD 트래킹 제어")]
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;

    [Header("이동 및 물리 제어")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider turnProvider;
    [SerializeField] private CharacterController characterController;

    [Header("양손 XR 컨트롤러 제어")]
    [SerializeField] private XRBaseController[] xrControllers; // Left, Right Hand
    [SerializeField] private XRRayInteractor[] rayInteractors;

    [Header("에디터 테스트용 시뮬레이터")]
    [SerializeField] private GameObject xrDeviceSimulator;

    [Header("UI 매니저들")]
    [SerializeField] private PlayerInitManager initManager;
    [SerializeField] private PlayerUIManager uiManager;

    private void Start()
    {
        Debug.Log($"아바타 스폰됨. ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}");
        // 이 캐릭터가 '내 컴퓨터'에서 스폰된 내 아바타가 아니라면 (즉, 클론이라면)
        if (photonView.IsMine)
        {
            Debug.Log("문제있는가?");
            // 1. 눈과 귀를 제거합니다 (시점/오디오 탈취 방지)
            if (mainCamera != null) mainCamera.enabled = true;
            if (audioListener != null) audioListener.enabled = true;

            // 2. HMD 입력 기반의 고개 회전을 막습니다
            if (trackedPoseDriver != null) trackedPoseDriver.enabled = true;

            // 3. 조이스틱 이동 입력과 물리 엔진 충돌을 막습니다
            if (playerMove != null) playerMove.enabled = true;
            if (moveProvider != null) moveProvider.enabled = true;
            if (turnProvider != null) turnProvider.enabled = true;
            if (characterController != null) characterController.enabled = true;

            // 4. 양손의 입력과 레이저 상호작용을 막습니다
            foreach (var controller in xrControllers)
            {
                if (controller != null) controller.enabled = true;
            }
            foreach (var interactor in rayInteractors)
            {
                if (interactor != null) interactor.enabled = true;
            }

            // 남의 아바타라면 시뮬레이터도 꺼버립니다.
            if (xrDeviceSimulator != null) xrDeviceSimulator.SetActive(true);

            if (initManager != null)
            {
                initManager.enabled = true;
                if (uiManager != null) uiManager.enabled = true;
                initManager.TotalInit();
            }
        }
        else
        {
            // 내 아바타라면 시뮬레이터가 켜진 상태를 유지하여 키보드 입력을 받습니다.
            if (xrDeviceSimulator != null) xrDeviceSimulator.SetActive(true);
        }
        //플레이어 움직임 정지 (시작시 UI 보는 용도)
        EventBus<OnSelfInstantiate>.Publish(default);
    }
}