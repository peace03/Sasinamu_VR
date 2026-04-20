using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    private AudioSource target;
    private void Awake()
    {
        target = GetComponent<AudioSource>();
    }
    public void Call()
    {
        target.Play();
    }
}
