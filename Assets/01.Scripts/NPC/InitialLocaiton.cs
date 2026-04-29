using System;
using UnityEngine;

public class InitialLocaiton : MonoBehaviour
{
    [SerializeField] private Transform XROrigin;

    private void Start()
    {
        //위치 초기화
        transform.position = new Vector3(transform.position.x, -2.2f, transform.position.z);
    }
    private void FixedUpdate()
    {
        if (XROrigin.position.y > -2.2f) XROrigin.position = new Vector3(XROrigin.position.x, -2.2f, XROrigin.position.z);
    }
}
