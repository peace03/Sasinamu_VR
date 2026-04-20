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

    private void Start()
    {
        // 이 캐릭터가 '내 컴퓨터'에서 스폰된 내 아바타가 아니라면 (즉, 클론이라면)
        if (!photonView.IsMine)
        {
            Debug.Log("문제있는가?");
            // 1. 눈과 귀를 제거합니다 (시점/오디오 탈취 방지)
            if (mainCamera != null) mainCamera.enabled = false;
            if (audioListener != null) audioListener.enabled = false;

            // 2. HMD 입력 기반의 고개 회전을 막습니다
            if (trackedPoseDriver != null) trackedPoseDriver.enabled = false;

            // 3. 조이스틱 이동 입력과 물리 엔진 충돌을 막습니다
            if (playerMove != null) playerMove.enabled = false;
            if (moveProvider != null) moveProvider.enabled = false;
            if (turnProvider != null) turnProvider.enabled = false;
            if (characterController != null) characterController.enabled = false;

            // 4. 양손의 입력과 레이저 상호작용을 막습니다
            foreach (var controller in xrControllers)
            {
                if (controller != null) controller.enabled = false;
            }
            foreach (var interactor in rayInteractors)
            {
                if (interactor != null) interactor.enabled = false;
            }
        }
    }
}