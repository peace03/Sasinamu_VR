using Photon.Pun;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    private int? id = null;        // 아이디
    public int? ID => id;

    private void OnEnable()
    {
        // 아이디 받아오기
        id = transform.root.GetComponentInChildren<PhotonView>()?.Owner?.ActorNumber;
    }

    private void OnDisable()
    {
        // 아이디 초기화
        id = null;
    }
}