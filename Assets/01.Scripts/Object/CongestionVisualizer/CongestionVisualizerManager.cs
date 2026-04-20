using UnityEngine;
using System.Collections.Generic;

//혼잡도 오브젝트 매니저
public class CongestionVisualizerManager : MonoBehaviour
{
    [SerializeField] private List<ChangeTextObj> congestionVisualizers;
    [Header("탑승 인원수 별 임계치")]
    [Tooltip("혼잡")]
    [SerializeField] private int congestion;
    [Tooltip("보통")]
    [SerializeField] private int normal;
    [Tooltip("여유")]
    [SerializeField] private int clear;

    private void Awake()
    {
        for (int i = 0; i < congestionVisualizers.Count; i++)
        {
            congestionVisualizers[i].Init(congestion, normal, clear);
        }
    }

    //탑승객 수 받기 UnityEvent 연결
    public void SetPassengerCount(int[] passengerCount)
    {
        for (int i = 0; i < congestionVisualizers.Count; i++)
        {
            congestionVisualizers[i].SetPassengerCount(passengerCount[i]);
        }
    }

    //대기 상태 UnityEvent 연결
    public void SetStandBy()
    {
        foreach(var obj in congestionVisualizers)
        {
            obj.SetStandBy();
        }
    }

}
