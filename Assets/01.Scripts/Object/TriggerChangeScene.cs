using UnityEngine;
using UnityEngine.Events;

public class TriggerChangeScene : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            OnTrigger?.Invoke();
    }
}
