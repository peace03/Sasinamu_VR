using UnityEngine;

public class InitialLocaiton : MonoBehaviour
{
    private void Start()
    {
        //위치 초기화
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
}
