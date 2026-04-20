using UnityEngine;
using System.Collections.Generic;

public class SubwayDoorHandlerManager : MonoBehaviour
{
    [SerializeField] private List<SubwayDoorHandler> DoorHandlers;
    [Header("설정")]
    [SerializeField] private Vector3 moveLocal; //이동할 로컬 좌표 거리
    [SerializeField] private float speed;

    private void Awake()
    {
        //초기화
        foreach (var door in DoorHandlers) { door.Init(moveLocal, speed); }
    }

    public void OpenDoor() { foreach (var door in DoorHandlers) door.OpenDoor(); }
    public void CloseDoor() { foreach(var door in DoorHandlers) door.CloseDoor(); }
}
