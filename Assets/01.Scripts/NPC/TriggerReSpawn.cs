using Photon.Pun;
using UnityEngine;

public class TriggerReSpawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //충돌 판정 및 리스폰 명령은 방장만 내림
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("리스폰 트리거 발동");
        if (other.TryGetComponent(out NPC_Brain npc_Brain))
        {
            Debug.Log("리스폰 NPC 확인 완료");
            npc_Brain.ReleaseSelf();
            NPC_PoolManager.Instance.SpawnNPC();
        }
    }
}
