using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerID : MonoBehaviour
{
    public bool IsUIHovering => isUIHovering;
    public int? ID => id;

    private XRRayInteractor interactor;     // 인터렉터

    private bool isUIHovering;              // UI 가리킴 여부
    private int? id = null;                 // 아이디

    private void Awake()
    {
        interactor = transform.GetComponent<XRRayInteractor>();
    }

    private void OnEnable()
    {
        // 아이디 받아오기
        id = transform.root.GetComponentInChildren<PhotonView>()?.Owner?.ActorNumber;
    }

    private void Update()
    {
        // 인터렉터가 비어있지 않고 UI에 닿고 있는 결과 값에 따라서
        if (interactor != null && interactor.TryGetCurrentUIRaycastResult(out var result))
            // UI 가리킴 여부 반영
            isUIHovering = result.isValid;
        // 아니라면
        else
            // UI 가리키고 있지 않음
            isUIHovering = false;
    }

    private void OnDisable()
    {
        // 아이디 초기화
        id = null;
    }
}