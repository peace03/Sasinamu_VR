using UnityEngine;

public class ArrowOffset : MonoBehaviour
{
    public static ArrowOffset Instance;     // 싱글톤

    [Header("화살표 위치")]
    [SerializeField] private Vector3[] positions;
    [Header("화살표 각도")]
    [SerializeField] private Quaternion[] rotations;
    public Vector3[] Positions => positions;
    public Quaternion[] Rotations => rotations;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}