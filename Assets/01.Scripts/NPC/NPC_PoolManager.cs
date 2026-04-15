using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NPC_PoolManager : MonoBehaviour
{
    public static NPC_PoolManager Instance;

    [SerializeField] private SubwayScheduler subwayScheduler;
    [SerializeField] private Transform subway;
    [SerializeField] private NPC_Brain npcPrefab;
    [SerializeField] List<Transform> gatePos;

    private IObjectPool<NPC_Brain> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<NPC_Brain>(
            createFunc: CreateNPC,
            actionOnGet: OnGetNPC,
            actionOnRelease: OnReleaseNPC,
            actionOnDestroy: OnDestroyNPC,
            collectionCheck: true,
            maxSize: 20);
    }

    private void Start()
    {
        for (int i = 0; i < 10; i++) SpawnNPC();
    }

    private NPC_Brain CreateNPC()
    {
        NPC_Brain npc = Instantiate(npcPrefab, transform);
        npc.SetPoolManager(_pool);
        npc.GetComponent<NPC_BT>().OnCreate(gatePos, subway);
        return npc;
    }

    private void OnGetNPC(NPC_Brain npc) { npc.gameObject.SetActive(true); }
    private void OnReleaseNPC(NPC_Brain npc) { npc.gameObject.SetActive(false); }
    private void OnDestroyNPC(NPC_Brain npc) { Destroy(npc.gameObject); }

    public void SpawnNPC()
    {
        NPC_Brain npc = _pool.Get();
        Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
        npc.GetComponent<NPC_BT>().OnSpawn(this.transform, spawnPos);
        npc.OnSpawn(subwayScheduler);
    }
}
