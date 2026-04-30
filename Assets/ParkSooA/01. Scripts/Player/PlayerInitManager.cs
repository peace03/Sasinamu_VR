using Photon.Pun;
using UnityEngine;

public class PlayerInitManager : MonoBehaviour
{
    [Header("초기화가 필요한 스크립트들")]
    [SerializeField] private PlayerID[] hands;

    public void TotalInit()
    {
        foreach (var hand in hands)
            hand.Init();

        transform.GetComponent<PlayerUIManager>().Init();
    }
}