using UnityEngine;

public class PlayHumanVFX : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private CharacterController controller;
    private float velocity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        velocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
        anim.SetFloat("Speed", velocity);
    }
}
