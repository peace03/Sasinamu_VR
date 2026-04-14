using UnityEngine;

public class ChangeMaterialObj : MonoBehaviour
{
    [SerializeField] private Color congestionObj;
    [SerializeField] private Color normalObj;
    [SerializeField] private Color clearObj;
    [SerializeField] private Color standByObj;

    public void SetMaterialColor(int passengerCount)
    {
        if (passengerCount >= 10) GetComponent<Renderer>().material.color = congestionObj;
        else if (passengerCount >= 5) GetComponent<Renderer>().material.color = normalObj;
        else GetComponent<Renderer>().material.color = clearObj;
    }

    public void SetStandByColor()
    {
        GetComponent<Renderer>().material.color = standByObj;
    }
}