using UnityEngine;

public class ChangeMaterialObj : MonoBehaviour
{
    [SerializeField] private Color congestionObj;
    [SerializeField] private Color normalObj;
    [SerializeField] private Color clearObj;
    [SerializeField] private Color standByObj;

    private int congestion;
    private int normal;
    private int clear;

    public void Init(int congestion, int normal, int clear)
    {
        this.congestion = congestion;
        this.normal = normal;
        this.clear = clear;
    }

    public void SetMaterialColor(int passengerCount)
    {
        if (passengerCount >= congestion) GetComponent<Renderer>().material.color = congestionObj;
        else if (passengerCount >= normal) GetComponent<Renderer>().material.color = normalObj;
        else if (passengerCount >= clear) GetComponent<Renderer>().material.color = clearObj;
    }

    public void SetStandByColor()
    {
        GetComponent<Renderer>().material.color = standByObj;
    }
}