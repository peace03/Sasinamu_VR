using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        Debug.Log("1. 서버 접속 시도 중...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("2. 접속 성공! 방 생성/참가 시도...");
        // 로비를 거치지 않고 곧바로 "TestRoom"이라는 이름의 방으로 들어갑니다.
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("3. 방 입장 성공! 플레이어 소환!");

        // 지하철 씬의 적당한 좌표(예: 개찰구 앞)를 직접 입력하세요.
        Vector3 spawnPos = new Vector3(0f, 0f, 0f);

        // 🚨 주의: Player 프리팹이 반드시 'Resources' 폴더 안에 있어야 합니다.
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}