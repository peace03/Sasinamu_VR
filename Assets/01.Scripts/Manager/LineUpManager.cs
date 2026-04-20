using UnityEngine;

public class LineUpManager : MonoBehaviour
{
    [SerializeField] private int gateCount = 4; //게이트 개수
    public float spaceBetweenNPCs = 1f;       //줄 간격

    private int[] gateLineCounts;   //각 게이트의 현재 줄서있는 인원수

    private void Awake()
    {
        gateLineCounts = new int[gateCount];
    }

    //NPC 줄 설 때 단 1번만 호출, 좌표 반환
    public Vector3 GetStaticLineUpPos(int gateIndex, Vector3 gatePos, Vector3 gateForward)
    {
        //현재 대기 인원 확인
        int myTurn = gateLineCounts[gateIndex];
        //대기 인원 추가
        gateLineCounts[gateIndex]++;
        //좌표 반환
        return gatePos - (gateForward * (myTurn * spaceBetweenNPCs));
    }

    //지하철 문 열릴 때 줄 초기화
    public void ResetAllLine()
    {
        for(int i = 0; i < gateLineCounts.Length; i++)
        {
            gateLineCounts[i] = 0;
        }
    }
}
