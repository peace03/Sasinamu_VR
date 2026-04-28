using UnityEngine;

public class ButtonData : MonoBehaviour
{
    [Header("버튼을 누르면 열리는 UI")]
    [SerializeField] protected GameObject targetUI;
    public GameObject TargetUI => targetUI;
}