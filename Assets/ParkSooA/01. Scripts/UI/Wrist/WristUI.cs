using UnityEngine;

public class WristUI : MonoBehaviour
{
    [SerializeField] private GameObject questUI;

    private Transform target;

    private void Awake()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        if (Vector3.Dot(transform.right, target.forward) <= -0.7f
            && Vector3.Distance(transform.position, target.position) <= 0.5f)
            questUI.SetActive(true);
        else
            questUI.SetActive(false);
    }
}