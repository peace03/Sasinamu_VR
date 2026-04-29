using UnityEngine;

public class ButtonData : MonoBehaviour
{
    [Header("버튼을 누르면 열리는 UI")]
    [SerializeField] protected GameObject targetUI;
    public GameObject TargetUI => targetUI;

    private string buttonPath = null;       // 버튼 경로
    public string ButtonPath => buttonPath;

    // 버튼 경로 설정 함수
    public void SetButtonPath(Transform limitParent)
    {
        // 경로를 저장할 변수
        string path = gameObject.name;
        // 경로를 추적할 변수
        Transform current = transform;

        // 비어있는 경로가 아니고 제한 경로에 도달할 때까지
        while (current.parent != null && current.parent != limitParent)
        {
            // 경로 추적
            current = current.parent;
            // 경로 저장
            path = current.name + "/" + path;
        }

        // 버튼 경로 저장
        buttonPath = path;
    }
}