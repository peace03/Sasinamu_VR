using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NPC_PoolManager : MonoBehaviourPunCallbacks, IPunPrefabPool
{
    public static NPC_PoolManager Instance;

    [SerializeField] private SubwayScheduler subwayScheduler;
    [SerializeField] private Transform subway;
    [SerializeField] private NPC_Brain npcPrefab;
    [SerializeField] List<Transform> gatePos;

    private IObjectPool<NPC_Brain> _pool;

    private void Awake()
    {
        Instance = this;

        _pool = new ObjectPool<NPC_Brain>(
            createFunc: CreateNPC,
            actionOnGet: OnGetNPC,
            actionOnRelease: OnReleaseNPC,
            actionOnDestroy: OnDestroyNPC,
            collectionCheck: true,
            maxSize: 10);

        //Photon엔진에게 "NPC 소환은 내가 만든 풀을 거쳐라"선언
        PhotonNetwork.PrefabPool = this;
    }

    public override void OnJoinedRoom()
    {
        //방장만 최초 10마리 소환
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 10; i++) SpawnNPC();
        }
    }

    //Photon 네트워크 풀링 필수 구현부
    //PhotonNetwork.Instantiate()가 호출될 때, 엔진이 내부적으로 이 함수를 실행
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //필터링: 요청받은 이름이 NPC 프리팹의 이름이 아니라면? (ex: Player)
        if (prefabId != npcPrefab.name)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>(prefabId), position, rotation);
            obj.SetActive(false);
            return obj;
        }

        //NPC 요청인 경우: 풀에서 가져옴
        NPC_Brain npc = _pool.Get();
        npc.GetComponent<NPC_BT>().OnSpawn(this.transform, position);
        npc.OnSpawn(subwayScheduler);

        //Photon엔진에 반환하기 직전, 반드시 오브젝트를 비활성화 상태로 만들어야함
        //엔진이 이 객체에 네트워크 ID를 부여한 뒤 알아서 활성화 해줄 것임
        npc.gameObject.SetActive(false);
        return npc.gameObject;
    }

    public void Destroy(GameObject gameObject)
    {
        //PhotonNetwork.Destroy 호출 시 엔진이 파괴 대신 반납 처리함
        if (gameObject.TryGetComponent(out NPC_Brain npc))
        {
            _pool.Release(npc);
        }
        else
        {
            //NPC가 아니라면 (무한루프 방지)
            UnityEngine.Object.Destroy(gameObject);
        }
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
        //스폰 권한도 방장만 가짐
        if (!PhotonNetwork.IsMasterClient) return;
        Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
        //유니티 Instantiate 대신 방에 소속된 네트워크 오브젝트 생성 명령
        PhotonNetwork.InstantiateRoomObject(npcPrefab.name, spawnPos, Quaternion.identity);
    }


}
