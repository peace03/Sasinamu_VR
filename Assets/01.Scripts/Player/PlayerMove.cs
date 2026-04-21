using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private ActionBasedContinuousMoveProvider ObjMove;
    [SerializeField] private ActionBasedContinuousTurnProvider ObjTurn;

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
