using UnityEngine;

public class SubwayDoorHandler : MonoBehaviour
{
    public enum DoorState
    {
        Opening,    //열리는 중
        Closing,    //닫히는 중
        stay        //대기
    }

    private Vector3 moveLocal; //이동할 로컬 좌표 거리
    private float speed;

    //현재 로컬좌표
    private Transform leftDoor;
    private Transform rightDoor;

    //원본 로컬좌표
    private Vector3 leftOriginPos;
    private Vector3 rightOriginPos;

    //지하철 문 행동 상태
    private DoorState doorState = DoorState.stay;

    public void Init(Vector3 moveLocal, float speed)
    {
        //문 수치 초기화
        this.moveLocal = moveLocal;
        this.speed = speed;

        //제어할 문
        leftDoor = transform.GetChild(0).transform;
        rightDoor = transform.GetChild(1).transform;
        //원본 로컬좌표
        leftOriginPos = leftDoor.localPosition;
        rightOriginPos = rightDoor.localPosition;
    }

    private void Update()
    {
        switch (doorState)
        {
            case DoorState.Opening:
                if (leftDoor.localPosition == leftOriginPos - moveLocal) doorState = DoorState.stay;
                MoveDoor(leftDoor.localPosition, leftOriginPos - moveLocal, rightDoor.localPosition, rightOriginPos + moveLocal);
                break;
            case DoorState.Closing:
                if (leftDoor.localPosition == leftOriginPos) doorState = DoorState.stay;
                MoveDoor(leftDoor.localPosition, leftOriginPos, rightDoor.localPosition, rightOriginPos);
                break;
            case DoorState.stay:
                break;
        }
    }

    public void OpenDoor()
    {
        doorState = DoorState.Opening;
    }
    public void CloseDoor()
    {
        doorState = DoorState.Closing;
    }

    public void MoveDoor(Vector3 leftStartPos, Vector3 leftEndPos, Vector3 rightStartPos, Vector3 rightEndPos)
    {
        leftDoor.localPosition = Vector3.Lerp(leftStartPos, leftEndPos, Time.deltaTime * speed);
        rightDoor.localPosition = Vector3.Lerp(rightStartPos, rightEndPos, Time.deltaTime * speed);
    }
}
