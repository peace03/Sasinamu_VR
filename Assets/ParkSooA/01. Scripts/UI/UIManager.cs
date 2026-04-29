using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI들")]
    [SerializeField] private List<GameObject> playerUI;

    private PhotonView pv;

    private void Awake()
    {
        // 초기화
        foreach (var ui in playerUI)
            ui.SetActive(false);
    }

    private void Start()
    {
        pv = transform.root.GetComponentInChildren<PhotonView>();

        if (pv != null && pv.IsMine)
            foreach (var ui in playerUI)
                ui.SetActive(true);
    }
}