using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI들")]
    [SerializeField] private PlatformWristUI wristUI;
    [SerializeField] private GameObject introUI;
    [SerializeField] private GameObject sceneChangeUI;

    [Header("방향 화살표")]
    [SerializeField] private ArrowPointer pointer;

    public void Init()
    {
        wristUI.gameObject.SetActive(true);
        wristUI.Init();
        sceneChangeUI.SetActive(true);
        sceneChangeUI.GetComponentInChildren<SmoothFollow>().Init();
        introUI.SetActive(true);
        introUI.GetComponentInChildren<SmoothFollow>().Init();
        introUI.GetComponentInChildren<IntroPopupUI>().Init();
        pointer.Init(wristUI.CurrentProgress);
        pointer.gameObject.SetActive(true);
    }
}