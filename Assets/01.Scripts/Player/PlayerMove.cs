using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private ActionBasedContinuousMoveProvider ObjMove;
    [SerializeField] private ActionBasedContinuousTurnProvider ObjTurn;

    public void StopObjMove()
    {
        ObjMove.enabled = false;
    }
    public void StopObjTurn()
    {
        ObjTurn.enabled = false;
    }
    public void FreeObjMove()
    {
        ObjMove.enabled = true;
    }
    public void FreeObjTurn()
    {
        ObjTurn.enabled = true;
    }
}
