using UnityEngine;

public class SubwayPassenserLevel : MonoBehaviour
{
    [Header("지하철 설정")]
    [Tooltip("최대 탑승객 인원 수")]
    [SerializeField] private int maxpassenger;      //최대 탑승객
    [Tooltip("지하철 칸 개수")]
    [SerializeField] private int maxSubwaySection;  //지하철 칸 개수

    private int[] passengerCount;

    private void Awake()
    {
        passengerCount = new int[maxSubwaySection];
    }

    //객실마다 인원수 재설정
    public int[] ResetCount()
    {
        for (int i = 0; i < maxSubwaySection; i++)
        {
            passengerCount[i] = Random.Range(1, maxpassenger);
        }
        return passengerCount;
    }
}
