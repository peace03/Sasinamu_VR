using UnityEngine;

public class WristUI : MonoBehaviour
{
    [Header("팝업 UI")]
    [SerializeField] private PopupUI popupUI;

    private Transform target;       // 메인 카메라

    private void Awake()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        // 시야에 들어오고, 가깝다면
        if (Vector3.Dot(transform.right, target.forward) <= -0.7f
            && Vector3.Distance(transform.position, target.position) <= 0.5f)
            // 팝업 UI 키기
            popupUI.PopupUIHandler(true);
        // 아니라면
        else
            // 팝업 UI 끄기
            popupUI.PopupUIHandler(false);
    }
}