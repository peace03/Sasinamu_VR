using Photon.Pun;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    private int? ownerID = null;        // 아이디
    public int? OwnerID => ownerID;

    private void OnEnable()
    {
        // 아이디 받아오기
        ownerID = transform.root.GetComponentInChildren<PhotonView>()?.Owner?.ActorNumber;
    }

    private void OnDisable()
    {
        // 아이디 초기화
        ownerID = null;
    }
}