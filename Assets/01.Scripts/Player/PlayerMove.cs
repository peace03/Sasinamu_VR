using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private ActionBasedContinuousMoveProvider ObjMove;
    [SerializeField] private ActionBasedContinuousTurnProvider ObjTurn;

    private Animator anim;
    private CharacterController cc;

    private Vector3 velocity;   //속도(속력으로 바꿔줘야함)

    private void Awake()
    {
        anim = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
        cc = transform.GetChild(0).GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        EventBus<OnEnterStandbyStart>.OnEvent += StopObj;
        EventBus<OnEnterStandbyEnd>.OnEvent += FreeObj;
    }
    private void OnDisable()
    {
        EventBus<OnEnterStandbyStart>.OnEvent -= StopObj;
        EventBus<OnEnterStandbyEnd>.OnEvent -= FreeObj;
    }

    private void Update()
    {
        //조이스틱 위치 읽기
        Vector2 stickInput = ObjMove.leftHandMoveAction.action.ReadValue<Vector2>();
        //조이스틱 안움직이면 Idle
        if (stickInput.magnitude <= 0.2) velocity = Vector3.zero;
        //조이스틱 움직이면 run
        else velocity = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        anim.SetFloat("Speed", velocity.magnitude);
        Debug.Log(velocity.magnitude);
    }

    public void StopObj(OnEnterStandbyStart _)
    {
        ObjMove.enabled = false;
        ObjTurn.enabled = false;
        Debug.Log("StopObj 실행");
    }
    public void FreeObj(OnEnterStandbyEnd _)
    {
        ObjMove.enabled = true;
        ObjTurn.enabled = true;
        Debug.Log("FreeObj 실행");
    }
}
