using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerInvoke : MonoBehaviour
{
    [SerializeField] private UnityEvent OnColiderEnter;

    private void OnTriggerEnter(Collider other)
    {
        OnColiderEnter?.Invoke();
    }
}
