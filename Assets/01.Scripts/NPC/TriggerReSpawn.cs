using UnityEngine;
using UnityEngine.Events;

public class TriggerReSpawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("리스폰 트리거 발동");
        if (other.TryGetComponent(out NPC_Brain npc_Brain))
        {
            Debug.Log("리스폰 NPC 확인 완료");
            npc_Brain.ReleaseSelf();
            NPC_PoolManager.Instance.SpawnNPC();
        }
    }
}
