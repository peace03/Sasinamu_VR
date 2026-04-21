using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        Debug.Log("마스터 서버 접속 시도 중...");
        // 설정 파일(App ID)을 기반으로 소켓 연결 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    //마스터 서버 접소에 성공했을 때 엔진이 자동으로 호출하는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 성공! 로비로 진입합니다.");
        PhotonNetwork.JoinLobby();  //대기실(로비)로 이동
    }

    //로비 진입에 성공했을 때 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 진입 성공! 방을 찾거나 생성합니다");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,    //최대 수용 인원
            IsVisible = true,   //다른 사람에게 방이 보이도록 설정
            IsOpen = true       //입장이 가능하도록 설정
        };

        //"SubwayStation"이라는 방이 있으면 들어가고, 없으면 방금 만든 옵션으로 방을 개설함
        PhotonNetwork.JoinOrCreateRoom("SubwayStation", roomOptions, TypedLobby.Default);
    }

    //방 입장에 성고했을 때 호출되는 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공! 플레이어를 네트워크 상에 스폰합니다.");

        // 주의: 첫 번째 파라미터는 Resources 폴더 안에 있는 프리팹의 정확한 '이름'이어야 합니다.
        // 스폰 위치는 일단 역 내부의 안전한 좌표로 임의 설정합니다.
        Vector3 spawnPos = new Vector3(0f, 0f, 1f);
        PhotonNetwork.Instantiate("Player_photon", spawnPos, Quaternion.identity);
        EventBus<OnSelfInstantiate>.Publish(default);
    }
}
