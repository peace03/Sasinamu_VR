using UnityEngine;
using Photon.Pun;

public class VRAnimation_Sync : MonoBehaviourPun, IPunObservable
{
    [Header("VR 컨트롤러")]
    [SerializeField] private Transform headCamera;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    //네트워크 임시 메모리
    private Quaternion networkHeadRot;
    private Vector3 networkLeftPos;
    private Quaternion networkLeftRot;
    private Vector3 networkRightPos;
    private Quaternion networkRightRot;

    private void LateUpdate()
    {
        //내 아바타일 때
        //Two Bone IK Constriant가 움직여주고 있음

        //내 아바타가 아닐 때
        if (!photonView.IsMine)
        {
            headCamera.localRotation = Quaternion.Slerp(headCamera.localRotation, networkHeadRot, Time.deltaTime * 15f);
            leftController.localPosition = Vector3.Lerp(leftController.localPosition, networkLeftPos, Time.deltaTime * 15f);
            leftController.localRotation = Quaternion.Slerp(leftController.localRotation, networkLeftRot, Time.deltaTime * 15);
            rightController.localPosition = Vector3.Lerp(rightController.localPosition, networkRightPos, Time.deltaTime * 15f);
            rightController.localRotation = Quaternion.Slerp(rightController.localRotation, networkRightRot, Time.deltaTime * 15);
        }
    }

    //네트워크 직렬화(데이터 송수신)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   //내 데이터일 때
        {
            stream.SendNext(headCamera.localRotation);
            stream.SendNext(leftController.localPosition);
            stream.SendNext(leftController.localRotation);
            stream.SendNext(rightController.localPosition);
            stream.SendNext(rightController.localRotation);
        }
        else    //내 데이터가 아닐 때
        {
            networkHeadRot = (Quaternion)stream.ReceiveNext();
            networkLeftPos = (Vector3)stream.ReceiveNext();
            networkLeftRot = (Quaternion)stream.ReceiveNext();
            networkRightPos = (Vector3)stream.ReceiveNext();
            networkRightRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
